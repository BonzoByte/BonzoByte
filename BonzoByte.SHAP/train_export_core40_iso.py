# train_export_core40_iso.py
import warnings; warnings.filterwarnings("ignore", category=UserWarning)

import json, joblib, pyodbc
import pandas as pd
import numpy as np
from sklearn.ensemble import HistGradientBoostingClassifier
from sklearn.calibration import CalibratedClassifierCV
from sklearn.pipeline import Pipeline
from sklearn.base import BaseEstimator, TransformerMixin

# ===== FIXED CORE40 (iz tvoje datoteke) =====
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

DENY_PREFIX = (
    'vw_NN_StreakDominance_perMatch__',
    'vw_NN_Form_perMatch__',
    'vw_NN_Recency_perMatch__',
)

META_COLS = ['MatchTPId','LeftPlayerTPId','RightPlayerTPId','Orientation','FeatureVersion','SnapshotAt','Split']

class CorePrep(BaseEstimator, TransformerMixin):
    def __init__(self, core_cols, deny_prefix=DENY_PREFIX):
        self.core_cols = list(core_cols)
        self.deny_prefix = deny_prefix
        self.used_cols_ = None

    def fit(self, X, y=None):
        cols = [c for c in self.core_cols if c in X.columns and not any(c.startswith(p) for p in self.deny_prefix)]
        self.used_cols_ = cols
        return self

    def transform(self, X):
        X = X[self.used_cols_].copy()
        X = X.select_dtypes(include=[np.number])
        miss = [c for c in X.columns if c.endswith('_isMissing')]
        uncert = [c for c in X.columns if '_uncert_' in c]
        models = [c for c in X.columns if c.endswith('modelsPresent')]
        prob_like = [c for c in X.columns if (('__p' in c) or ('pBlend' in c) or c.endswith('_01') or ('_Delta_' in c) or ('_Diff_' in c))]
        prob_like = [c for c in prob_like if not c.endswith('_isMissing')]
        if miss:   X[miss]   = X[miss].fillna(1)
        if uncert: X[uncert] = X[uncert].fillna(1)
        if models: X[models] = X[models].fillna(0)
        if prob_like: X[prob_like] = X[prob_like].fillna(0.5)
        X = X.fillna(X.median(numeric_only=True))
        return X

def load_df(conn, view_name="dbo.vw_NN_MatchData_sample_1pct"):
    df = pd.read_sql(f"SELECT * FROM {view_name}", conn)
    return df

def build_pipeline():
    base = HistGradientBoostingClassifier(
        learning_rate=0.08, max_iter=300, max_depth=6,
        min_samples_leaf=20, l2_regularization=1.0, random_state=42
    )
    try:
        cal = CalibratedClassifierCV(estimator=base, method="isotonic", cv=5)
    except TypeError:
        cal = CalibratedClassifierCV(base_estimator=base, method="isotonic", cv=5)
    pipe = Pipeline(steps=[
        ("coreprep", CorePrep(CORE40)),
        ("clf", cal)
    ])
    return pipe

def main():
    conn = pyodbc.connect(
        "Driver={SQL Server};Server=Stanko;Database=BonzoByte;Trusted_Connection=yes;Connection Timeout=180000000;TrustServerCertificate=Yes;"
    )
    df = load_df(conn)
    y = df['y'].astype(int).copy()
    X = df.drop(columns=['y'] + [c for c in META_COLS if c in df.columns], errors='ignore')

    pipe = build_pipeline()
    pipe.fit(X, y)

    joblib.dump(pipe, "bb_core40_iso_model.joblib")
    # spremi i kolone koje pipeline zaista koristi (redoslijed)
    used_cols = pipe.named_steps["coreprep"].used_cols_
    with open("bb_core40_columns.json","w",encoding="utf-8") as f:
        json.dump(used_cols, f, ensure_ascii=False, indent=2)

    print(f"Saved model: bb_core40_iso_model.joblib")
    print(f"Saved columns: bb_core40_columns.json")
    conn.close()

if __name__ == "__main__":
    main()
