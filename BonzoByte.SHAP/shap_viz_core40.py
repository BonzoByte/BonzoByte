# shap_viz_core40.py
import warnings; warnings.filterwarnings("ignore", category=UserWarning)
import pyodbc, numpy as np, pandas as pd
import matplotlib
matplotlib.use("Agg")  # bez GUI-a
import matplotlib.pyplot as plt
import shap

from sklearn.model_selection import GroupShuffleSplit
from sklearn.ensemble import HistGradientBoostingClassifier

# ======= PODESI PO ŽELJI =======
VIEW_NAME   = "dbo.vw_NN_MatchData_sample_1pct"  # zamijeni svojim "full" pogledom kad budeš htio
MAX_ROWS    = None        # npr. 250000 za brže
N_SHAP      = 2000        # uzorak za SHAP (1500–3000 je ok)
RANDOM_STATE = 42

# CORE40 (zamrznuto)
CORE40 = [
    "vw_NN_H2H_perMatch__H2H_glb_Delta_01",
    "vw_NN_H2H_perMatch__pH2H_glb_01",
    "vw_NN_H2H_perMatch__H2H_SM_Delta_01",
    "vw_NN_H2H_perMatch__pH2H_M_01",
    "vw_NN_H2H_perMatch__H2H_glb_pBlend_01",
    "vw_NN_TS_perMatch__TS_uncert_GSM_01",
    "vw_NN_TS_perMatch__TS_uncert_SM_01",
    "vw_NN_H2H_perMatch__H2H_GSMS_Delta_01",
    "vw_NN_H2H_perMatch__H2H_M_Delta_01",
    "vw_NN_H2H_perMatch__H2H_GSM_pBlend_01",
    "vw_NN_H2H_perMatch__pH2H_SM_01",
    "vw_NN_TS_perMatch__L_TSprob_GSM_01",
    "vw_NN_H2H_perMatch__pH2H_GSM_01",
    "vw_NN_TS_perMatch__TS_uncert_M_01",
    "vw_NN_TS_perMatch__L_TS_gap_SM_01",
    "vw_NN_TS_perMatch__TS_uncert_GSMS_S_01",
    "vw_NN_H2H_perMatch__H2H_GSM_Delta_01",
    "vw_NN_TS_perMatch__L_TSprob_SM_01",
    "vw_NN_H2H_perMatch__H2H_MS_Delta_01",
    "vw_NN_H2H_perMatch__H2H_SMS_Delta_01",
    "vw_NN_TS_perMatch__L_TSprob_MS_S_01",
    "vw_NN_H2H_perMatch__H2H_MS_AbsDelta_01",
    "vw_NN_H2H_perMatch__pH2H_GSMS_01",
    "vw_NN_H2H_perMatch__H2H_SM_pBlend_01",
    "vw_NN_H2H_perMatch__H2H_glb_AbsDelta_01",
    "vw_NN_H2H_perMatch__H2H_sfc_Delta_01",
    "vw_NN_H2H_perMatch__H2H_M_pBlend_01",
    "vw_NN_H2H_perMatch__pH2H_MS_01",
    "vw_NN_TS_perMatch__L_TSprob_SMS_S_01",
    "vw_NN_TS_perMatch__L_TS_gap_GSM_01",
    "vw_NN_H2H_perMatch__pH2H_SMS_01",
    "vw_NN_TS_perMatch__TS_uncert_SMS_S_01",
    "vw_NN_H2H_perMatch__H2H_MS_Extreme_01",
    "vw_NN_H2H_perMatch__pH2H_sfc_01",
    "vw_NN_H2H_perMatch__H2H_SM_AbsDelta_01",
    "vw_NN_TS_perMatch__L_TSprob_M_01",
    "vw_NN_TS_perMatch__L_TS_meanProb_01",
    "vw_NN_H2H_perMatch__H2H_GSMS_Extreme_01",
    "vw_NN_H2H_perMatch__H2H_sfc_pBlend_01",
    "vw_NN_H2H_perMatch__H2H_M_Extreme_01",
]

META = ['MatchTPId','LeftPlayerTPId','RightPlayerTPId','Orientation','FeatureVersion','SnapshotAt','Split']
DENY = ('vw_NN_StreakDominance_perMatch__','vw_NN_Form_perMatch__','vw_NN_Recency_perMatch__')

def load_df():
    conn = pyodbc.connect(
        "Driver={SQL Server};Server=Stanko;Database=BonzoByte;Trusted_Connection=yes;"
        "Connection Timeout=180000000;TrustServerCertificate=Yes;")
    df = pd.read_sql(f"SELECT * FROM {VIEW_NAME}", conn)
    conn.close()
    return df

def prep_xy(df):
    if MAX_ROWS and len(df)>MAX_ROWS:
        df = df.sample(n=MAX_ROWS, random_state=RANDOM_STATE).reset_index(drop=True)
    y = df['y'].astype(int).copy()
    X = df.drop(columns=['y']+[c for c in META if c in df.columns], errors='ignore')
    # uzmi CORE40 (i osiguraj da nije u DENY)
    cols = [c for c in CORE40 if (c in X.columns and not any(c.startswith(p) for p in DENY))]
    X = X[cols].copy().select_dtypes(include=[np.number])

    # IMPUTACIJA (identična kao prije)
    miss   = [c for c in X.columns if c.endswith('_isMissing')]
    uncert = [c for c in X.columns if '_uncert_' in c]
    models = [c for c in X.columns if c.endswith('modelsPresent')]
    prob_like = [c for c in X.columns if (('__p' in c) or ('pBlend' in c) or c.endswith('_01') or ('_Delta_' in c) or ('_Diff_' in c))]
    prob_like = [c for c in prob_like if not c.endswith('_isMissing')]
    if miss:   X[miss]   = X[miss].fillna(1)
    if uncert: X[uncert] = X[uncert].fillna(1)
    if models: X[models] = X[models].fillna(0)
    if prob_like: X[prob_like] = X[prob_like].fillna(0.5)
    X = X.fillna(X.median(numeric_only=True))

    # Split bez leaka
    use_time = ('Split' in df.columns) and df['Split'].isin(['TRAIN','VAL']).any()
    if use_time:
        mtr = (df['Split']=='TRAIN'); mva = (df['Split']=='VAL')
        if (mtr.sum()==0) or (mva.sum()==0) or (y[mtr].nunique()<2) or (y[mva].nunique()<2):
            use_time = False

    if use_time:
        Xtr, ytr = X[mtr], y[mtr]
        Xte, yte = X[mva], y[mva]
    else:
        if 'MatchTPId' not in df.columns:
            raise RuntimeError("Za group split treba kolona MatchTPId.")
        gss = GroupShuffleSplit(n_splits=1, test_size=0.2, random_state=RANDOM_STATE)
        tr, te = next(gss.split(X, y, groups=df['MatchTPId']))
        Xtr, Xte, ytr, yte = X.iloc[tr], X.iloc[te], y.iloc[tr], y.iloc[te]

    return Xtr, Xte, ytr, yte

def main():
    print("Loading…")
    df = load_df()
    Xtr, Xte, ytr, yte = prep_xy(df)
    print(f"Train: {Xtr.shape} | Test: {Xte.shape}")

    # Brzi base model (bez kalibracije) – zgodan za SHAP
    clf = HistGradientBoostingClassifier(
        learning_rate=0.08, max_iter=300, max_depth=6,
        min_samples_leaf=20, l2_regularization=1.0, random_state=RANDOM_STATE
    )
    clf.fit(Xtr, ytr)

    # SHAP na uzorku testa
    n = min(N_SHAP, len(Xte))
    Xs = Xte.sample(n=n, random_state=RANDOM_STATE) if n < len(Xte) else Xte
    print(f"SHAP sample: {Xs.shape}")

    explainer = shap.TreeExplainer(clf)
    sv = explainer.shap_values(Xs)

    # normalize output shape
    if isinstance(sv, list):
        shap_matrix = sv[1] if len(sv)>1 else sv[0]
    elif hasattr(sv, "values"):
        shap_matrix = sv.values
    else:
        shap_matrix = np.asarray(sv)
    if shap_matrix.ndim == 3:
        shap_matrix = shap_matrix[:, :, 0]

    # ---- Beeswarm ----
    shap.summary_plot(shap_matrix, Xs, max_display=40, show=False)
    plt.title("SHAP beeswarm – CORE40")
    plt.tight_layout()
    plt.savefig("shap_beeswarm_core40.png", dpi=160)
    plt.close()

    # ---- Bar (mean |SHAP|) ----
    shap.summary_plot(shap_matrix, Xs, plot_type="bar", max_display=40, show=False)
    plt.title("SHAP mean(|value|) – CORE40")
    plt.tight_layout()
    plt.savefig("shap_bar_core40.png", dpi=160)
    plt.close()

    # CSV rang važnosti
    mean_abs = np.mean(np.abs(shap_matrix), axis=0)
    imp = (pd.DataFrame({"feature": list(Xs.columns), "mean_abs_shap": mean_abs})
           .sort_values("mean_abs_shap", ascending=False))
    imp.to_csv("shap_importance_core40.csv", index=False)

    print("Saved: shap_beeswarm_core40.png, shap_bar_core40.png, shap_importance_core40.csv")

if __name__ == "__main__":
    main()