# score_core40_iso.py
import warnings; warnings.filterwarnings("ignore", category=UserWarning)

import json, joblib, pyodbc
import pandas as pd
from datetime import datetime, timezone

MODEL_PATH = "bb_core40_iso_model.joblib"
COLS_PATH  = "bb_core40_columns.json"
VIEW_TO_SCORE = "dbo.vw_NN_MatchData_sample_1pct"   # promijeni na svoj toScore pogled
OUT_TABLE     = "dbo.NN_Core40_Scores"
MODEL_TAG     = "CORE40_ISO_v1"

META_COLS = ['MatchTPId','LeftPlayerTPId','RightPlayerTPId','Orientation','FeatureVersion','SnapshotAt','Split']

def ensure_table(conn):
    ddl = f"""
    IF OBJECT_ID('{OUT_TABLE}', 'U') IS NULL
    BEGIN
        CREATE TABLE {OUT_TABLE}(
            MatchTPId BIGINT NOT NULL,
            p1_win_prob FLOAT NOT NULL,
            ScoredAt DATETIME2 NOT NULL CONSTRAINT DF_{OUT_TABLE.replace('.','_')}_ScoredAt DEFAULT SYSUTCDATETIME(),
            ModelTag NVARCHAR(50) NOT NULL
        );
        CREATE INDEX IX_{OUT_TABLE.replace('.','_')}_MatchTPId ON {OUT_TABLE}(MatchTPId);
    END
    """
    cur = conn.cursor()
    cur.execute(ddl)
    conn.commit()

def main():
    pipe = joblib.load(MODEL_PATH)
    with open(COLS_PATH,"r",encoding="utf-8") as f:
        used_cols = json.load(f)

    conn = pyodbc.connect(
        "Driver={SQL Server};Server=Stanko;Database=BonzoByte;Trusted_Connection=yes;Connection Timeout=180000000;TrustServerCertificate=Yes;"
    )
    ensure_table(conn)

    df = pd.read_sql(f"SELECT * FROM {VIEW_TO_SCORE}", conn)
    # makni y ako postoji
    if 'y' in df.columns:
        df = df.drop(columns=['y'])

    # spremi id-eve za upis
    if 'MatchTPId' not in df.columns:
        raise RuntimeError("Očekujem kolonu MatchTPId za upis rezultata.")
    ids = df['MatchTPId'].values

    X = df.drop(columns=[c for c in META_COLS if c in df.columns], errors='ignore')

    # osiguraj da postoje potrebne kolone (ako nedostaju, bit će ignorirane jer ih CorePrep ionako filtrira)
    missing = [c for c in used_cols if c not in X.columns]
    if missing:
        print(f"[WARN] Nedostaju neke CORE kolone ({len(missing)}), npr: {missing[:5]} — model će raditi s preostalim.")

    p = pipe.predict_proba(X)[:,1]

    # upis u SQL
    now = datetime.now(timezone.utc).strftime("%Y-%m-%d %H:%M:%S")
    rows = list(zip(ids, p, [now]*len(p), [MODEL_TAG]*len(p)))
    cur = conn.cursor()
    cur.fast_executemany = True
    cur.executemany(f"INSERT INTO {OUT_TABLE} (MatchTPId, p1_win_prob, ScoredAt, ModelTag) VALUES (?, ?, ?, ?)", rows)
    conn.commit()
    print(f"Scored {len(rows)} rows into {OUT_TABLE}.")
    conn.close()

if __name__ == "__main__":
    main()