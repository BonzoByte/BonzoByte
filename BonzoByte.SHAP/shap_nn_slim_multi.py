import warnings; warnings.filterwarnings("ignore", category=UserWarning)

import pyodbc, numpy as np, pandas as pd
from pathlib import Path

from sklearn.ensemble import HistGradientBoostingClassifier
from sklearn.calibration import CalibratedClassifierCV
from sklearn.metrics import roc_auc_score, brier_score_loss
import shap, matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt

# ========= PODESIVO =========
RANDOM_STATE  = 42
MAX_ROWS      = None       # npr. 500_000 za brži test-run
N_SHAP        = 2000
DB_CONN_STR   = (
    "Driver={SQL Server};"
    "Server=Stanko;"
    "Database=BonzoByte;"
    "Trusted_Connection=yes;"
    "Connection Timeout=180000000;"
    "TrustServerCertificate=Yes;"
)

# Koje viewove vozimo (tag, sql_view, r_group)
# r_group ∈ {"R0","R1"} -> određuje drop H2H kolona
VIEWS = [
    ("R0_ALL",  "dbo.vw_NN_Slim_R0_ALL",  "R0"),
    ("R1_ALL",  "dbo.vw_NN_Slim_R1_ALL",  "R1"),
    ("R0_CLAY", "dbo.vw_NN_Slim_R0_CLAY", "R0"),
    ("R1_CLAY", "dbo.vw_NN_Slim_R1_CLAY", "R1"),
    ("R0_HARD", "dbo.vw_NN_Slim_R0_HARD", "R0"),
    ("R1_HARD", "dbo.vw_NN_Slim_R1_HARD", "R1"),
    ("R0_GRASS","dbo.vw_NN_Slim_R0_GRASS","R0"),
    ("R1_GRASS","dbo.vw_NN_Slim_R1_GRASS","R1"),
]

OUT_ROOT = Path("./outputs_slim_multi")
OUT_ROOT.mkdir(parents=True, exist_ok=True)

# Meta kolone i drop-lista s previše rupa
META = {
    'y':           'FavWon',
    'fold':        'Fold',
    'w':           'SampleWeightRecency',
    'drop_also':   ['MatchTPId','IsTrain','IsVal','IsTest','SurfaceEffectiveId']  # imamo IsSurf1..4
}
DROP_HARD = {'BmiDiff','BmiGap','WeightDiff','HeightDiff','ProYearsDiff'}

# H2H kolone koje maknemo za R0 (bez H2H evidencije)
H2H_FEATURES = {
    'H2H_conflict_weighted','H2HWinRateSmoothedDiff','H2HSmoothedWeighted',
    'dWP_H2HM_M_abs','dZ_H2HM_M_abs','H2HMatches'
}

# Winsor heavy-tail kolone (ako postoje)
WINSOR_COLS_ALL = ['RestDaysFavMinusOpp','DaysSinceWinDiff','DaysSinceLossDiff','StreakDiff','H2HMatches']


# ========= UTIL =========
def load_df(view_sql: str):
    conn = pyodbc.connect(DB_CONN_STR)
    df = pd.read_sql(f"SELECT * FROM {view_sql}", conn)
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
        m = (idx == b); n = int(m.sum())
        if n == 0:
            rows.append((b+1, n, np.nan, np.nan)); continue
        conf = float(p[m].mean()); acc = float(y_true[m].mean())
        ece += (n / N) * abs(acc - conf)
        rows.append((b+1, n, conf, acc))
    return pd.DataFrame(rows, columns=["bin","n","mean_pred","emp_rate"]), ece

def prep_splits(df: pd.DataFrame, drop_h2h: bool):
    # meta
    y    = df[META['y']].astype(int).copy()
    w    = df[META['w']].astype(float).clip(lower=1e-6, upper=1.0).copy()
    fold = df[META['fold']].astype(str)

    # kandidat featurea = sve numeric osim meta/drop
    drop_cols = set([META['y'], META['fold'], META['w']]) | set(META['drop_also']) | DROP_HARD
    num = df.select_dtypes(include=[np.number]).columns.tolist()
    feats = [c for c in num if c not in drop_cols]

    if drop_h2h:
        feats = [c for c in feats if c not in H2H_FEATURES]

    X = df[feats].copy()

    # train/val/test maske
    mtr = (fold=='train'); mva = (fold=='val'); mte = (fold=='test')

    # medijani iz traina
    med = X[mtr].median(numeric_only=True)

    # winsor samo po postojećim kol.
    wins_cols = [c for c in WINSOR_COLS_ALL if c in X.columns]
    if wins_cols:
        q_lo = X[mtr][wins_cols].quantile(0.01).to_dict()
        q_hi = X[mtr][wins_cols].quantile(0.99).to_dict()
    else:
        q_lo, q_hi = {}, {}

    def prep(block: pd.DataFrame):
        Z = block.copy()
        zero_like = [c for c in Z.columns if any(k in c for k in
                      ['IsSurf','FavHome','FavIsHome','PlaysMismatch','PrizeMissing',
                       'WP_', 'WinRate', 'SetWinRate', 'GameWinRate',
                       'EffSurf', 'RecencySlope','SeasonMonth','H2H_conflict','H2HWinRateSmoothedDiff','H2HSmoothedWeighted'])]
        if zero_like:
            Z[zero_like] = Z[zero_like].fillna(0.0)
        Z = Z.fillna(med)
        for c in wins_cols:
            Z[c] = Z[c].clip(q_lo[c], q_hi[c])
        return Z

    Xtr, Xva, Xte = prep(X[mtr]), prep(X[mva]), prep(X[mte])
    ytr, yva, yte = y[mtr], y[mva], y[mte]
    wtr           = w[mtr]
    return Xtr, Xva, Xte, ytr, yva, yte, wtr, feats

def make_base():
    return HistGradientBoostingClassifier(
        learning_rate=0.08, max_iter=300, max_depth=6,
        min_samples_leaf=20, l2_regularization=1.0, random_state=RANDOM_STATE
    )

def auto_calibrate(base, Xva, yva):
    """
    Auto izbor: isotonic ako ima dovoljno validacije (>=200 po klasi),
    sigmoid ako je srednje (>=100 po klasi), inače bez kalibracije.
    """
    n = len(yva)
    pos = int(yva.sum()); neg = n - pos
    if n >= 1000 and pos >= 200 and neg >= 200:
        cal = CalibratedClassifierCV(estimator=base, method="isotonic", cv='prefit')
        cal.fit(Xva, yva); return cal, "isotonic(prefit)"
    elif n >= 400 and pos >= 100 and neg >= 100:
        cal = CalibratedClassifierCV(estimator=base, method="sigmoid", cv='prefit')
        cal.fit(Xva, yva); return cal, "sigmoid(prefit)"
    else:
        # premalo za stabilnu kalibraciju – vraćamo bazni model
        return base, "none"

def eval_and_shap(model, Xte, yte, out_dir: Path, tag: str, n_shap=N_SHAP):
    out_dir.mkdir(parents=True, exist_ok=True)

    proba = model.predict_proba(Xte)[:,1] if hasattr(model, "predict_proba") else model.predict(Xte)
    auc   = roc_auc_score(yte, proba)
    brier = brier_score_loss(yte, proba)
    bins, ece = calib_report(yte, proba, bins=12)
    print(f"[{tag}] Test AUC: {auc:.4f} | Brier: {brier:.4f} | ECE: {ece:.4f}")

    bins.to_csv(out_dir / "calibration_bins_test.csv", index=False)
    pd.DataFrame([{"AUC":auc, "Brier":brier, "ECE":ece}]).to_csv(out_dir/"metrics_test.csv", index=False)

    # SHAP sample
    n = min(n_shap, len(Xte))
    Xs = Xte.sample(n=n, random_state=RANDOM_STATE) if n < len(Xte) else Xte

    # uzmi “base” za TreeExplainer ako je CalibratedCV
    base = model
    if hasattr(model, "base_estimator"):
        base = model.base_estimator
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

    # vizuali
    shap.summary_plot(shap_matrix, Xs, max_display=40, show=False)
    plt.title(f"SHAP beeswarm – {tag}")
    plt.tight_layout(); plt.savefig(out_dir/"shap_beeswarm.png", dpi=160); plt.close()

    shap.summary_plot(shap_matrix, Xs, plot_type="bar", max_display=40, show=False)
    plt.title(f"SHAP mean(|value|) – {tag}")
    plt.tight_layout(); plt.savefig(out_dir/"shap_bar.png", dpi=160); plt.close()

    imp = (pd.DataFrame({"feature": list(Xs.columns),
                         "mean_abs_shap": np.mean(np.abs(shap_matrix), axis=0)})
           .sort_values("mean_abs_shap", ascending=False))
    imp.to_csv(out_dir/"shap_importance.csv", index=False)

    return auc, brier, ece

def run_one(tag: str, view_sql: str, r_group: str):
    print(f"\n=== {tag} :: {view_sql} ({r_group}) ===")
    df = load_df(view_sql)
    print(f"Rows: {len(df):,}")
    assert {'train','val','test'}.issubset(set(df['Fold'].unique())), "Fold mora imati train/val/test."

    drop_h2h = (r_group == "R0")
    Xtr, Xva, Xte, ytr, yva, yte, wtr, feats = prep_splits(df, drop_h2h=drop_h2h)

    print(f"Train: {Xtr.shape} | Val: {Xva.shape} | Test: {Xte.shape}")

    base = make_base()
    base.fit(Xtr, ytr, sample_weight=wtr)

    model, cal_tag = auto_calibrate(base, Xva, yva)
    print(f"Calibration: {cal_tag}")

    out_dir = OUT_ROOT / tag
    auc, brier, ece = eval_and_shap(model, Xte, yte, out_dir, tag=tag, n_shap=N_SHAP)
    return dict(tag=tag, rows=len(df), auc=auc, brier=brier, ece=ece, calib=cal_tag)

def main():
    rows = []
    for tag, view_sql, rgrp in VIEWS:
        try:
            r = run_one(tag, view_sql, rgrp)
            rows.append(r)
        except Exception as e:
            print(f"[WARN] {tag} failed: {e}")
    if rows:
        pd.DataFrame(rows).to_csv(OUT_ROOT / "summary.csv", index=False)
        print("\nSaved summary →", OUT_ROOT / "summary.csv")

if __name__ == "__main__":
    main()
