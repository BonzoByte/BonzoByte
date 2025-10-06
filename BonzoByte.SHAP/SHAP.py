# BonzoByte – FULL(94) vs FIXED CORE40, sigmoid vs isotonic: usporedba + perm importance
import warnings
warnings.filterwarnings("ignore", category=UserWarning)

import pyodbc
import pandas as pd
import numpy as np
from sklearn.model_selection import GroupShuffleSplit
from sklearn.metrics import roc_auc_score, brier_score_loss
from sklearn.inspection import permutation_importance
from sklearn.ensemble import HistGradientBoostingClassifier
from sklearn.calibration import CalibratedClassifierCV

# ======= KONFIG =======
FAST_SCAN   = True
MAX_ROWS    = None
N_PI_MAX    = 4000
N_REPEATS   = 8
SAVE_FILES  = True

# ======= FIXED CORE40 (iz tvog zadnjeg run-a) =======
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

# ======= KONEKCIJA =======
conn = pyodbc.connect(
    "Driver={SQL Server};"
    "Server=Stanko;"
    "Database=BonzoByte;"
    "Trusted_Connection=yes;"
    "Connection Timeout=180000000;"
    "TrustServerCertificate=Yes;"
)

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
            rows.append((b+1, n, np.nan, np.nan))
            continue
        conf = float(p[m].mean())
        acc  = float(y_true[m].mean())
        ece += (n / N) * abs(acc - conf)
        rows.append((b+1, n, conf, acc))
    return pd.DataFrame(rows, columns=["bin","n","mean_pred","emp_rate"]), ece

def make_X(df):
    meta_cols = ['MatchTPId','LeftPlayerTPId','RightPlayerTPId','Orientation','FeatureVersion','SnapshotAt','Split']
    y = df['y'].astype(int).copy()
    X = df.drop(columns=['y'] + [c for c in meta_cols if c in df.columns], errors='ignore')

    # CORE_TS_H2H: dopuštene + zabrane
    DENY    = ('vw_NN_StreakDominance_perMatch__','vw_NN_Form_perMatch__','vw_NN_Recency_perMatch__')
    ALLOWED = ('vw_NN_H2H_perMatch__','vw_NN_TS_perMatch__')
    X = X[[c for c in X.columns if c.startswith(ALLOWED) and not c.startswith(DENY)]].copy()

    # numeričko + pragmatična imputacija
    X = X.select_dtypes(include=[np.number]).copy()
    miss_cols   = [c for c in X.columns if c.endswith('_isMissing')]
    uncert_cols = [c for c in X.columns if '_uncert_' in c]
    models_cols = [c for c in X.columns if c.endswith('modelsPresent')]
    prob_like   = [c for c in X.columns if (('__p' in c) or ('pBlend' in c) or c.endswith('_01') or ('_Delta_' in c) or ('_Diff_' in c))]
    prob_like   = [c for c in prob_like if not c.endswith('_isMissing')]
    if miss_cols:   X[miss_cols]   = X[miss_cols].fillna(1)
    if uncert_cols: X[uncert_cols] = X[uncert_cols].fillna(1)
    if models_cols: X[models_cols] = X[models_cols].fillna(0)
    if prob_like:   X[prob_like]   = X[prob_like].fillna(0.5)
    X = X.fillna(X.median(numeric_only=True))
    return X, y

def split_xy(df, X, y):
    use_time_split = 'Split' in df.columns and df['Split'].isin(['TRAIN','VAL']).any()
    if use_time_split:
        mtr = (df['Split']=='TRAIN'); mva = (df['Split']=='VAL')
        if mtr.sum()==0 or mva.sum()==0 or y[mtr].nunique()<2 or y[mva].nunique()<2:
            use_time_split = False
    if use_time_split:
        return X[mtr], X[mva], y[mtr], y[mva]
    else:
        if 'MatchTPId' not in df.columns:
            raise RuntimeError("Za group split treba kolona MatchTPId.")
        gss = GroupShuffleSplit(n_splits=1, test_size=0.2, random_state=42)
        tr, te = next(gss.split(X, y, groups=df['MatchTPId']))
        return X.iloc[tr], X.iloc[te], y.iloc[tr], y.iloc[te]

def make_base():
    if FAST_SCAN:
        return HistGradientBoostingClassifier(
            learning_rate=0.08, max_iter=300, max_depth=6,
            min_samples_leaf=20, l2_regularization=1.0, random_state=42
        )
    else:
        return HistGradientBoostingClassifier(
            learning_rate=0.05, max_iter=1200, max_depth=8,
            min_samples_leaf=10, l2_regularization=0.5,
            early_stopping=True, scoring="neg_brier_score", random_state=42
        )

def fit_calibrated(X_train, y_train, method):
    base = make_base()
    try:
        cal = CalibratedClassifierCV(estimator=base, method=method, cv=5)  # sklearn >=1.2
    except TypeError:
        cal = CalibratedClassifierCV(base_estimator=base, method=method, cv=5)  # starije verzije
    cal.fit(X_train, y_train)
    return cal

def eval_model(clf, X_test, y_test, name="model"):
    proba = clf.predict_proba(X_test)[:,1]
    auc   = roc_auc_score(y_test, proba)
    brier = brier_score_loss(y_test, proba)
    bins, ece = calib_report(y_test, proba, bins=12)
    print(f"[{name}] AUC: {auc:.4f} | Brier: {brier:.4f} | ECE: {ece:.4f}")
    return dict(proba=proba, auc=auc, brier=brier, bins=bins, ece=ece)

def slice_auc(df, X_test, y_test, proba, tag):
    ix = X_test.index
    cols = {
        "Hard":   'vw_NN_Tournament_features__Surf_Hard',
        "Clay":   'vw_NN_Tournament_features__Surf_Clay',
        "Grass":  'vw_NN_Tournament_features__Surf_Grass',
        "Unknown":'vw_NN_Tournament_features__Surf_Unknown'
    }
    have = {k:v for k,v in cols.items() if v in df.columns}
    outs = []
    for name, col in have.items():
        m = (df.loc[ix, col]==1)
        n = int(m.sum())
        if n>=50:
            outs.append((tag, name, n, roc_auc_score(y_test[m], proba[m])))
        else:
            outs.append((tag, name, n, np.nan))
    return outs

try:
    # ====== load & prep ======
    df = pd.read_sql("SELECT * FROM dbo.vw_NN_MatchData_sample_1pct", conn)
    if MAX_ROWS and len(df) > MAX_ROWS:
        df = df.sample(n=MAX_ROWS, random_state=42).reset_index(drop=True)

    X, y = make_X(df)
    print(f"FEATURES: {X.shape[1]}")
    X_train, X_test, y_train, y_test = split_xy(df, X, y)
    print(f"Train: {X_train.shape} | Test: {X_test.shape}")

    # ====== FULL(94): oba kalibratora ======
    clf_full_sig = fit_calibrated(X_train, y_train, "sigmoid")
    r_full_sig   = eval_model(clf_full_sig, X_test, y_test, "FULL + SIGMOID")

    clf_full_iso = fit_calibrated(X_train, y_train, "isotonic")
    r_full_iso   = eval_model(clf_full_iso, X_test, y_test, "FULL + ISOTONIC")

    # Odaberi bolji FULL po (Brier, -AUC) za perm importance
    best_clf, best_tag = (clf_full_sig, "FULL+SIGMOID") if (r_full_sig["brier"], -r_full_sig["auc"]) < (r_full_iso["brier"], -r_full_iso["auc"]) else (clf_full_iso, "FULL+ISOTONIC")
    print(f">> Permutation importance on: {best_tag}")

    # ====== perm importance na FULL ======
    n_pi = min(N_PI_MAX, len(X_test))
    X_pi = X_test.sample(n=n_pi, random_state=42) if n_pi < len(X_test) else X_test
    y_pi = y_test.loc[X_pi.index]
    pi = permutation_importance(best_clf, X_pi, y_pi, n_repeats=N_REPEATS, random_state=42, scoring='roc_auc', n_jobs=-1)
    pi_df = (pd.DataFrame({"feature": X_pi.columns, "perm_imp_mean": pi.importances_mean, "perm_imp_std": pi.importances_std})
             .sort_values("perm_imp_mean", ascending=False).reset_index(drop=True))
    print("\nTop 20 permutation importances:")
    print(pi_df.head(20).to_string(index=False))
    if SAVE_FILES:
        pi_df.to_csv("perm_importances_CORE_TS_H2H.csv", index=False)
        print("Saved: perm_importances_CORE_TS_H2H.csv")

    # ====== FIXED CORE40 ======
    missing = [c for c in CORE40 if c not in X.columns]
    if missing:
        print(f"\n[WARN] {len(missing)} CORE40 kolona nedostaje u DF i bit će ignorirano (npr. {missing[:5]})")
    core_feats = [c for c in CORE40 if c in X.columns]
    Xtr_c, Xte_c = X_train[core_feats].copy(), X_test[core_feats].copy()

    clf_c_sig = fit_calibrated(Xtr_c, y_train, "sigmoid")
    r_c_sig   = eval_model(clf_c_sig, Xte_c, y_test, "CORE40 + SIGMOID")

    clf_c_iso = fit_calibrated(Xtr_c, y_train, "isotonic")
    r_c_iso   = eval_model(clf_c_iso, Xte_c, y_test, "CORE40 + ISOTONIC")

    # ====== AUC po podlozi ======
    rows = []
    rows += slice_auc(df, X_test, y_test, r_full_sig["proba"], "FULL+SIG")
    rows += slice_auc(df, X_test, y_test, r_full_iso["proba"], "FULL+ISO")
    rows += slice_auc(df, Xte_c,  y_test, r_c_sig["proba"],   "CORE40+SIG")
    rows += slice_auc(df, Xte_c,  y_test, r_c_iso["proba"],   "CORE40+ISO")
    surf_df = pd.DataFrame(rows, columns=["model","surface","n","AUC"])
    print("\nAUC by surface:")
    print(surf_df.to_string(index=False))

    # ====== Sažetak ======
    summary = pd.DataFrame([
        ["FULL","SIGMOID", r_full_sig["auc"], r_full_sig["brier"], r_full_sig["ece"]],
        ["FULL","ISOTONIC",r_full_iso["auc"], r_full_iso["brier"], r_full_iso["ece"]],
        ["CORE40","SIGMOID",r_c_sig["auc"],   r_c_sig["brier"],   r_c_sig["ece"]],
        ["CORE40","ISOTONIC",r_c_iso["auc"], r_c_iso["brier"],   r_c_iso["ece"]],
    ], columns=["Set","Calib","AUC","Brier","ECE"])
    print("\n=== SUMMARY ===")
    print(summary.to_string(index=False))

    if SAVE_FILES:
        with open("core_features_FIXED_CORE40.txt","w",encoding="utf-8") as f:
            for c in core_feats: f.write(c+"\n")
        print("\nSaved: core_features_FIXED_CORE40.txt")

finally:
    conn.close()
