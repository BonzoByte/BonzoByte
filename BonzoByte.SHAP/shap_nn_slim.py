# shap_nn_slim.py  — SHAP baseline na dbo.NN_Features_Slim
import warnings; warnings.filterwarnings("ignore", category=UserWarning)

import pyodbc, numpy as np, pandas as pd
from pathlib import Path

from sklearn.ensemble import HistGradientBoostingClassifier
from sklearn.calibration import CalibratedClassifierCV
from sklearn.metrics import roc_auc_score, brier_score_loss
import shap, matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt

# ======= PODESIVO =======
MAX_ROWS      = None          # npr. 500_000 za brži prvi run
N_SHAP        = 2000
RANDOM_STATE  = 42
DB_CONN_STR   = (
    "Driver={SQL Server};"
    "Server=Stanko;"
    "Database=BonzoByte;"
    "Trusted_Connection=yes;"
    "Connection Timeout=180000000;"
    "TrustServerCertificate=Yes;"
)

OUT_DIR = Path(".")
OUT_DIR.mkdir(parents=True, exist_ok=True)

META = {
    'y':           'FavWon',
    'fold':        'Fold',
    'w':           'SampleWeightRecency',
    'drop_also':   ['MatchTPId','IsTrain','IsVal','IsTest','SurfaceEffectiveId']  # imamo IsSurf1..4
}

# kolone s previše rupa/outliera — makni za 1. rundu (po tvom profilu null-ova)
DROP_HARD = {'BmiDiff','BmiGap','WeightDiff','HeightDiff','ProYearsDiff'}

# winsor & imputacija — stabilizira heavy-tail metrike
WINSOR_COLS = ['RestDaysFavMinusOpp','DaysSinceWinDiff','DaysSinceLossDiff','StreakDiff','H2HMatches']

def load_df():
    conn = pyodbc.connect(DB_CONN_STR)
    df = pd.read_sql("SELECT * FROM dbo.NN_Features_Slim", conn)
    conn.close()
    if MAX_ROWS and len(df) > MAX_ROWS:
        df = df.sample(n=MAX_ROWS, random_state=RANDOM_STATE).reset_index(drop=True)
    return df

def calib_report(y_true, p, bins=12):
    y_true = np.asarray(y_true).astype(int)
    p = np.asarray(p).clip(1e-6, 1-1e-6)
    edges = np.linspace(0.0, 1.0, bins+1)
    idx = np.digitize(p, edges[1:-1], right=False)
    rows, ece = [], 0.0
    N = len(p)
    for b in range(bins):
        m = (idx == b)
        n = int(m.sum())
        if n == 0:
            rows.append((b+1, n, np.nan, np.nan)); continue
        conf = float(p[m].mean())
        acc  = float(y_true[m].mean())
        ece += (n / N) * abs(acc - conf)
        rows.append((b+1, n, conf, acc))
    return pd.DataFrame(rows, columns=["bin","n","mean_pred","emp_rate"]), ece

def split_sets(df):
    y = df[META['y']].astype(int).copy()
    w = df[META['w']].astype(float).clip(lower=1e-6, upper=1.0).copy()
    fold = df[META['fold']].astype(str)

    # candidate features = numeric minus meta/drop
    drop_cols = set([META['y'], META['fold'], META['w']]) | set(META['drop_also']) | DROP_HARD
    num = df.select_dtypes(include=[np.number]).columns.tolist()
    feats = [c for c in num if c not in drop_cols]

    X = df[feats].copy()

    # minimalna imputacija:
    # - sve NaN → 0 za rate/dummyje je ok za drvo; ostalo median (po trainu kasnije)
    #   pa ćemo preciznije odraditi na trainu i aplicirati na val/test
    mtr = (fold=='train'); mva = (fold=='val'); mte = (fold=='test')

    # train medians
    med = X[mtr].median(numeric_only=True)

    # winsor granice iz traina
    q_lo = X[mtr][WINSOR_COLS].quantile(0.01).to_dict()
    q_hi = X[mtr][WINSOR_COLS].quantile(0.99).to_dict()

    def prep(block):
        Z = block.copy()
        # NaN -> 0 tamo gdje je dummy/prob/rate po nazivu
        zero_like = [c for c in Z.columns if any(k in c for k in
                      ['IsSurf','FavHome','FavIsHome','PlaysMismatch','PrizeMissing',
                       'WP_', 'WinRate', 'SetWinRate', 'GameWinRate',
                       'EffSurf', 'RecencySlope','SeasonMonth','H2H_conflict','H2HWinRateSmoothedDiff','H2HSmoothedWeighted'])]
        if zero_like:
            Z[zero_like] = Z[zero_like].fillna(0.0)

        # ostalo median (učimo median iz traina)
        Z = Z.fillna(med)

        # winsor + stabilizacija heavy-tail
        for c in WINSOR_COLS:
            if c in Z.columns:
                Z[c] = Z[c].clip(q_lo[c], q_hi[c])
        return Z

    Xtr, Xva, Xte = prep(X[mtr]), prep(X[mva]), prep(X[mte])
    ytr, yva, yte = y[mtr], y[mva], y[mte]
    wtr           = w[mtr]

    return Xtr, Xva, Xte, ytr, yva, yte, wtr, feats

def train_and_calibrate(Xtr, ytr, wtr, Xva, yva):
    base = HistGradientBoostingClassifier(
        learning_rate=0.08, max_iter=300, max_depth=6,
        min_samples_leaf=20, l2_regularization=1.0, random_state=RANDOM_STATE
    )
    base.fit(Xtr, ytr, sample_weight=wtr)

    # isotonic kalibracija na VAL (bez leaka)
    cal = CalibratedClassifierCV(estimator=base, method="isotonic", cv='prefit')
    cal.fit(Xva, yva)
    return base, cal

def eval_and_shap(model, Xte, yte, feats, tag="HGB+ISO", n_shap=N_SHAP):
    proba = model.predict_proba(Xte)[:,1]
    auc   = roc_auc_score(yte, proba)
    brier = brier_score_loss(yte, proba)
    bins, ece = calib_report(yte, proba, bins=12)
    print(f"[{tag}] Test AUC: {auc:.4f} | Brier: {brier:.4f} | ECE: {ece:.4f}")

    bins.to_csv(OUT_DIR / "calibration_bins_test.csv", index=False)

    # SHAP na uzorku
    n = min(n_shap, len(Xte))
    Xs = Xte.sample(n=n, random_state=RANDOM_STATE) if n < len(Xte) else Xte

    # Explainer: koristi bazni estimator iz CalibratedCV
    base = model.base_estimator if hasattr(model, "base_estimator") else model
    if hasattr(model, "estimator"):  # sklearn >=1.5
        base = model.estimator

    explainer = shap.TreeExplainer(base)
    sv = explainer.shap_values(Xs)
    if isinstance(sv, list):
        shap_matrix = sv[1] if len(sv)>1 else sv[0]
    elif hasattr(sv, "values"):
        shap_matrix = sv.values
    else:
        shap_matrix = np.asarray(sv)
    if shap_matrix.ndim == 3:
        shap_matrix = shap_matrix[:, :, 0]

    # Beeswarm
    shap.summary_plot(shap_matrix, Xs, max_display=40, show=False)
    plt.title("SHAP beeswarm – NN_Features_Slim")
    plt.tight_layout(); plt.savefig(OUT_DIR/"shap_beeswarm_slim.png", dpi=160); plt.close()

    # Bar
    shap.summary_plot(shap_matrix, Xs, plot_type="bar", max_display=40, show=False)
    plt.title("SHAP mean(|value|) – NN_Features_Slim")
    plt.tight_layout(); plt.savefig(OUT_DIR/"shap_bar_slim.png", dpi=160); plt.close()

    mean_abs = np.mean(np.abs(shap_matrix), axis=0)
    imp = (pd.DataFrame({"feature": list(Xs.columns), "mean_abs_shap": mean_abs})
           .sort_values("mean_abs_shap", ascending=False))
    imp.to_csv(OUT_DIR/"shap_importance_slim.csv", index=False)

    # Sažetak metrika
    pd.DataFrame([{"AUC":auc, "Brier":brier, "ECE":ece}]).to_csv(OUT_DIR/"metrics_test.csv", index=False)
    return auc, brier, ece

def main():
    print("Loading from dbo.NN_Features_Slim …")
    df = load_df()
    print(f"Rows: {len(df):,}")

    # sanity: očekujem foldove
    assert {'train','val','test'}.issubset(set(df['Fold'].unique())), "Fold mora imati train/val/test."

    Xtr, Xva, Xte, ytr, yva, yte, wtr, feats = split_sets(df)
    print(f"Train: {Xtr.shape} | Val: {Xva.shape} | Test: {Xte.shape}")

    base, cal = train_and_calibrate(Xtr, ytr, wtr, Xva, yva)
    eval_and_shap(cal, Xte, yte, feats, tag="HGB+ISO", n_shap=N_SHAP)

if __name__ == "__main__":
    main()