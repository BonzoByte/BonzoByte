CREATE VIEW dbo.vw_NN_Recency_perMatch
AS
/* =========================
   Parametri (podesivo)
   ========================= */
WITH param AS (
    SELECT 
        -- Half-life (dani): Core + Experimental
        CAST(14.0 AS float) AS H_WIN_CORE,  -- Win recency (core)
        CAST(21.0 AS float) AS H_LOSS_CORE, -- Loss recency (core)
        CAST(7.0  AS float) AS H_EXP_FAST,  -- Experimental brzi decay
        CAST(28.0 AS float) AS H_EXP_SLOW,  -- Experimental sporiji decay

        -- Linear CAP (dani) za eksperimentalnu varijantu
        CAST(60.0 AS float) AS CAP_LIN
),
/* =========================
   Sirovi brojevi dana; surface-pick S1..S4
   ========================= */
base AS (
    SELECT
        m.MatchTPId,
        m.SurfaceId,

        -- Global (bez podloge)
        CAST(m.Player1DaysSinceLastWin  AS float) AS P1_Days_Win,
        CAST(m.Player2DaysSinceLastWin  AS float) AS P2_Days_Win,
        CAST(m.Player1DaysSinceLastLoss AS float) AS P1_Days_Loss,
        CAST(m.Player2DaysSinceLastLoss AS float) AS P2_Days_Loss,

        -- Surface-specifično: uzmi baš za SurfaceId meča
        CAST(CASE m.SurfaceId
            WHEN 1 THEN m.Player1DaysSinceLastWinS1
            WHEN 2 THEN m.Player1DaysSinceLastWinS2
            WHEN 3 THEN m.Player1DaysSinceLastWinS3
            WHEN 4 THEN m.Player1DaysSinceLastWinS4
        END AS float) AS P1_Days_Win_S,

        CAST(CASE m.SurfaceId
            WHEN 1 THEN m.Player2DaysSinceLastWinS1
            WHEN 2 THEN m.Player2DaysSinceLastWinS2
            WHEN 3 THEN m.Player2DaysSinceLastWinS3
            WHEN 4 THEN m.Player2DaysSinceLastWinS4
        END AS float) AS P2_Days_Win_S,

        CAST(CASE m.SurfaceId
            WHEN 1 THEN m.Player1DaysSinceLastLossS1
            WHEN 2 THEN m.Player1DaysSinceLastLossS2
            WHEN 3 THEN m.Player1DaysSinceLastLossS3
            WHEN 4 THEN m.Player1DaysSinceLastLossS4
        END AS float) AS P1_Days_Loss_S,

        CAST(CASE m.SurfaceId
            WHEN 1 THEN m.Player2DaysSinceLastLossS1
            WHEN 2 THEN m.Player2DaysSinceLastLossS2
            WHEN 3 THEN m.Player2DaysSinceLastLossS3
            WHEN 4 THEN m.Player2DaysSinceLastLossS4
        END AS float) AS P2_Days_Loss_S
    FROM dbo.[Match] m
),
/* =========================
   Core i eksperimentalne mape u 0–1
   ========================= */
calc AS (
    SELECT
      b.MatchTPId,

      -- ====== CORE (half-life) ======
      -- Win recency (0–1); NULL -> 0 (uz flag niže)
      CASE WHEN b.P1_Days_Win  IS NULL THEN 0
           WHEN b.P1_Days_Win  <= 0   THEN 1
           ELSE POWER(0.5, b.P1_Days_Win  / p.H_WIN_CORE) END AS P1_WinRecency_01,
      CASE WHEN b.P2_Days_Win  IS NULL THEN 0
           WHEN b.P2_Days_Win  <= 0   THEN 1
           ELSE POWER(0.5, b.P2_Days_Win  / p.H_WIN_CORE) END AS P2_WinRecency_01,

      CASE WHEN b.P1_Days_Loss IS NULL THEN 0
           WHEN b.P1_Days_Loss <= 0   THEN 1
           ELSE POWER(0.5, b.P1_Days_Loss / p.H_LOSS_CORE) END AS P1_LossRecency_01,
      CASE WHEN b.P2_Days_Loss IS NULL THEN 0
           WHEN b.P2_Days_Loss <= 0   THEN 1
           ELSE POWER(0.5, b.P2_Days_Loss / p.H_LOSS_CORE) END AS P2_LossRecency_01,

      -- Surface varijante (samo za SurfaceId meča)
      CASE WHEN b.P1_Days_Win_S  IS NULL THEN 0
           WHEN b.P1_Days_Win_S  <= 0   THEN 1
           ELSE POWER(0.5, b.P1_Days_Win_S  / p.H_WIN_CORE) END AS P1_WinRecency_S_01,
      CASE WHEN b.P2_Days_Win_S  IS NULL THEN 0
           WHEN b.P2_Days_Win_S  <= 0   THEN 1
           ELSE POWER(0.5, b.P2_Days_Win_S  / p.H_WIN_CORE) END AS P2_WinRecency_S_01,

      CASE WHEN b.P1_Days_Loss_S IS NULL THEN 0
           WHEN b.P1_Days_Loss_S <= 0   THEN 1
           ELSE POWER(0.5, b.P1_Days_Loss_S / p.H_LOSS_CORE) END AS P1_LossRecency_S_01,
      CASE WHEN b.P2_Days_Loss_S IS NULL THEN 0
           WHEN b.P2_Days_Loss_S <= 0   THEN 1
           ELSE POWER(0.5, b.P2_Days_Loss_S / p.H_LOSS_CORE) END AS P2_LossRecency_S_01,

      -- Match recency = max(win, loss)
      NULLIF(0,0) AS _placeholder -- (hack da zadržimo CROSS JOIN uredan)

    FROM base b
    CROSS JOIN param p
),
core AS (
    SELECT
      c.MatchTPId,

      -- Core metrika
      c.P1_WinRecency_01, c.P2_WinRecency_01,
      c.P1_LossRecency_01, c.P2_LossRecency_01,

      c.P1_WinRecency_S_01, c.P2_WinRecency_S_01,
      c.P1_LossRecency_S_01, c.P2_LossRecency_S_01,

      -- Match recency (global)
      CASE WHEN c.P1_WinRecency_01=0 AND c.P1_LossRecency_01=0 THEN 0
           ELSE CASE WHEN c.P1_WinRecency_01 >= c.P1_LossRecency_01 THEN c.P1_WinRecency_01 ELSE c.P1_LossRecency_01 END END AS P1_MatchRecency_01,
      CASE WHEN c.P2_WinRecency_01=0 AND c.P2_LossRecency_01=0 THEN 0
           ELSE CASE WHEN c.P2_WinRecency_01 >= c.P2_LossRecency_01 THEN c.P2_WinRecency_01 ELSE c.P2_LossRecency_01 END END AS P2_MatchRecency_01,

      -- Match recency (surface)
      CASE WHEN c.P1_WinRecency_S_01=0 AND c.P1_LossRecency_S_01=0 THEN 0
           ELSE CASE WHEN c.P1_WinRecency_S_01 >= c.P1_LossRecency_S_01 THEN c.P1_WinRecency_S_01 ELSE c.P1_LossRecency_S_01 END END AS P1_MatchRecency_S_01,
      CASE WHEN c.P2_WinRecency_S_01=0 AND c.P2_LossRecency_S_01=0 THEN 0
           ELSE CASE WHEN c.P2_WinRecency_S_01 >= c.P2_LossRecency_S_01 THEN c.P2_WinRecency_S_01 ELSE c.P2_LossRecency_S_01 END END AS P2_MatchRecency_S_01,

      -- Momentum tilt = ((win - loss)+1)/2
      ((c.P1_WinRecency_01 - c.P1_LossRecency_01) + 1.0)/2.0 AS P1_Tilt_01,
      ((c.P2_WinRecency_01 - c.P2_LossRecency_01) + 1.0)/2.0 AS P2_Tilt_01,
      ((c.P1_WinRecency_S_01 - c.P1_LossRecency_S_01) + 1.0)/2.0 AS P1_Tilt_S_01,
      ((c.P2_WinRecency_S_01 - c.P2_LossRecency_S_01) + 1.0)/2.0 AS P2_Tilt_S_01
    FROM calc c
),
/* =========================
   Experimental: dodatni half-life i linearni CAP
   ========================= */
exp AS (
    SELECT
      b.MatchTPId,

      -- H=7 i H=28 (global)
      CASE WHEN b.P1_Days_Win  IS NULL THEN 0 WHEN b.P1_Days_Win  <= 0 THEN 1 ELSE POWER(0.5, b.P1_Days_Win  / p.H_EXP_FAST) END AS P1_WinRecency_H7_01,
      CASE WHEN b.P2_Days_Win  IS NULL THEN 0 WHEN b.P2_Days_Win  <= 0 THEN 1 ELSE POWER(0.5, b.P2_Days_Win  / p.H_EXP_FAST) END AS P2_WinRecency_H7_01,
      CASE WHEN b.P1_Days_Loss IS NULL THEN 0 WHEN b.P1_Days_Loss <= 0 THEN 1 ELSE POWER(0.5, b.P1_Days_Loss / p.H_EXP_FAST) END AS P1_LossRecency_H7_01,
      CASE WHEN b.P2_Days_Loss IS NULL THEN 0 WHEN b.P2_Days_Loss <= 0 THEN 1 ELSE POWER(0.5, b.P2_Days_Loss / p.H_EXP_FAST) END AS P2_LossRecency_H7_01,

      CASE WHEN b.P1_Days_Win  IS NULL THEN 0 WHEN b.P1_Days_Win  <= 0 THEN 1 ELSE POWER(0.5, b.P1_Days_Win  / p.H_EXP_SLOW) END AS P1_WinRecency_H28_01,
      CASE WHEN b.P2_Days_Win  IS NULL THEN 0 WHEN b.P2_Days_Win  <= 0 THEN 1 ELSE POWER(0.5, b.P2_Days_Win  / p.H_EXP_SLOW) END AS P2_WinRecency_H28_01,
      CASE WHEN b.P1_Days_Loss IS NULL THEN 0 WHEN b.P1_Days_Loss <= 0 THEN 1 ELSE POWER(0.5, b.P1_Days_Loss / p.H_EXP_SLOW) END AS P1_LossRecency_H28_01,
      CASE WHEN b.P2_Days_Loss IS NULL THEN 0 WHEN b.P2_Days_Loss <= 0 THEN 1 ELSE POWER(0.5, b.P2_Days_Loss / p.H_EXP_SLOW) END AS P2_LossRecency_H28_01,

      -- H=7 i H=28 (surface)
      CASE WHEN b.P1_Days_Win_S  IS NULL THEN 0 WHEN b.P1_Days_Win_S  <= 0 THEN 1 ELSE POWER(0.5, b.P1_Days_Win_S  / p.H_EXP_FAST) END AS P1_WinRecency_S_H7_01,
      CASE WHEN b.P2_Days_Win_S  IS NULL THEN 0 WHEN b.P2_Days_Win_S  <= 0 THEN 1 ELSE POWER(0.5, b.P2_Days_Win_S  / p.H_EXP_FAST) END AS P2_WinRecency_S_H7_01,
      CASE WHEN b.P1_Days_Loss_S IS NULL THEN 0 WHEN b.P1_Days_Loss_S <= 0 THEN 1 ELSE POWER(0.5, b.P1_Days_Loss_S / p.H_EXP_FAST) END AS P1_LossRecency_S_H7_01,
      CASE WHEN b.P2_Days_Loss_S IS NULL THEN 0 WHEN b.P2_Days_Loss_S <= 0 THEN 1 ELSE POWER(0.5, b.P2_Days_Loss_S / p.H_EXP_FAST) END AS P2_LossRecency_S_H7_01,

      CASE WHEN b.P1_Days_Win_S  IS NULL THEN 0 WHEN b.P1_Days_Win_S  <= 0 THEN 1 ELSE POWER(0.5, b.P1_Days_Win_S  / p.H_EXP_SLOW) END AS P1_WinRecency_S_H28_01,
      CASE WHEN b.P2_Days_Win_S  IS NULL THEN 0 WHEN b.P2_Days_Win_S  <= 0 THEN 1 ELSE POWER(0.5, b.P2_Days_Win_S  / p.H_EXP_SLOW) END AS P2_WinRecency_S_H28_01,
      CASE WHEN b.P1_Days_Loss_S IS NULL THEN 0 WHEN b.P1_Days_Loss_S <= 0 THEN 1 ELSE POWER(0.5, b.P1_Days_Loss_S / p.H_EXP_SLOW) END AS P1_LossRecency_S_H28_01,
      CASE WHEN b.P2_Days_Loss_S IS NULL THEN 0 WHEN b.P2_Days_Loss_S <= 0 THEN 1 ELSE POWER(0.5, b.P2_Days_Loss_S / p.H_EXP_SLOW) END AS P2_LossRecency_S_H28_01,

      -- Linear CAP (global + surface)
      CASE WHEN b.P1_Days_Win  IS NULL THEN 0 ELSE 1 - CASE WHEN b.P1_Days_Win  / p.CAP_LIN > 1 THEN 1 ELSE (b.P1_Days_Win  / p.CAP_LIN) END END AS P1_WinRecency_Lin60_01,
      CASE WHEN b.P2_Days_Win  IS NULL THEN 0 ELSE 1 - CASE WHEN b.P2_Days_Win  / p.CAP_LIN > 1 THEN 1 ELSE (b.P2_Days_Win  / p.CAP_LIN) END END AS P2_WinRecency_Lin60_01,
      CASE WHEN b.P1_Days_Loss IS NULL THEN 0 ELSE 1 - CASE WHEN b.P1_Days_Loss / p.CAP_LIN > 1 THEN 1 ELSE (b.P1_Days_Loss / p.CAP_LIN) END END AS P1_LossRecency_Lin60_01,
      CASE WHEN b.P2_Days_Loss IS NULL THEN 0 ELSE 1 - CASE WHEN b.P2_Days_Loss / p.CAP_LIN > 1 THEN 1 ELSE (b.P2_Days_Loss / p.CAP_LIN) END END AS P2_LossRecency_Lin60_01,

      CASE WHEN b.P1_Days_Win_S  IS NULL THEN 0 ELSE 1 - CASE WHEN b.P1_Days_Win_S  / p.CAP_LIN > 1 THEN 1 ELSE (b.P1_Days_Win_S  / p.CAP_LIN) END END AS P1_WinRecency_S_Lin60_01,
      CASE WHEN b.P2_Days_Win_S  IS NULL THEN 0 ELSE 1 - CASE WHEN b.P2_Days_Win_S  / p.CAP_LIN > 1 THEN 1 ELSE (b.P2_Days_Win_S  / p.CAP_LIN) END END AS P2_WinRecency_S_Lin60_01,
      CASE WHEN b.P1_Days_Loss_S IS NULL THEN 0 ELSE 1 - CASE WHEN b.P1_Days_Loss_S / p.CAP_LIN > 1 THEN 1 ELSE (b.P1_Days_Loss_S / p.CAP_LIN) END END AS P1_LossRecency_S_Lin60_01,
      CASE WHEN b.P2_Days_Loss_S IS NULL THEN 0 ELSE 1 - CASE WHEN b.P2_Days_Loss_S / p.CAP_LIN > 1 THEN 1 ELSE (b.P2_Days_Loss_S / p.CAP_LIN) END END AS P2_LossRecency_S_Lin60_01

    FROM base b
    CROSS JOIN param p
),
/* =========================
   Delta (Surface - Global) i Diff (P1 vs P2)
   ========================= */
meta AS (
    SELECT
      co.*,

      -- Delta (surface - global) u [0,1]
      ((co.P1_WinRecency_S_01  - co.P1_WinRecency_01)  + 1.0)/2.0 AS P1_WinRecency_Delta_01,
      ((co.P2_WinRecency_S_01  - co.P2_WinRecency_01)  + 1.0)/2.0 AS P2_WinRecency_Delta_01,
      ((co.P1_LossRecency_S_01 - co.P1_LossRecency_01) + 1.0)/2.0 AS P1_LossRecency_Delta_01,
      ((co.P2_LossRecency_S_01 - co.P2_LossRecency_01) + 1.0)/2.0 AS P2_LossRecency_Delta_01,

      -- Diff (P1 vs P2) za core set (global + surface)
      ((co.P1_WinRecency_01      - co.P2_WinRecency_01)      + 1.0)/2.0 AS WinRecency_Diff_01,
      ((co.P1_LossRecency_01     - co.P2_LossRecency_01)     + 1.0)/2.0 AS LossRecency_Diff_01,
      ((co.P1_MatchRecency_01    - co.P2_MatchRecency_01)    + 1.0)/2.0 AS MatchRecency_Diff_01,
      ((co.P1_Tilt_01            - co.P2_Tilt_01)            + 1.0)/2.0 AS Tilt_Diff_01,

      ((co.P1_WinRecency_S_01    - co.P2_WinRecency_S_01)    + 1.0)/2.0 AS WinRecency_S_Diff_01,
      ((co.P1_LossRecency_S_01   - co.P2_LossRecency_S_01)   + 1.0)/2.0 AS LossRecency_S_Diff_01,
      ((co.P1_MatchRecency_S_01  - co.P2_MatchRecency_S_01)  + 1.0)/2.0 AS MatchRecency_S_Diff_01,
      ((co.P1_Tilt_S_01          - co.P2_Tilt_S_01)          + 1.0)/2.0 AS Tilt_S_Diff_01

    FROM core co
),
/* =========================
   Missing flagovi
   ========================= */
flags AS (
    SELECT
      b.MatchTPId,

      CASE WHEN b.P1_Days_Win  IS NULL THEN 1 ELSE 0 END AS P1_WinRecency_isMissing,
      CASE WHEN b.P2_Days_Win  IS NULL THEN 1 ELSE 0 END AS P2_WinRecency_isMissing,
      CASE WHEN b.P1_Days_Loss IS NULL THEN 1 ELSE 0 END AS P1_LossRecency_isMissing,
      CASE WHEN b.P2_Days_Loss IS NULL THEN 1 ELSE 0 END AS P2_LossRecency_isMissing,

      CASE WHEN b.P1_Days_Win_S  IS NULL THEN 1 ELSE 0 END AS P1_WinRecency_S_isMissing,
      CASE WHEN b.P2_Days_Win_S  IS NULL THEN 1 ELSE 0 END AS P2_WinRecency_S_isMissing,
      CASE WHEN b.P1_Days_Loss_S IS NULL THEN 1 ELSE 0 END AS P1_LossRecency_S_isMissing,
      CASE WHEN b.P2_Days_Loss_S IS NULL THEN 1 ELSE 0 END AS P2_LossRecency_S_isMissing
    FROM base b
)
SELECT
  m.MatchTPId,

  /* ===== CORE (global) ===== */
  m.P1_WinRecency_01,  m.P2_WinRecency_01,
  m.P1_LossRecency_01, m.P2_LossRecency_01,

  m.P1_MatchRecency_01, m.P2_MatchRecency_01,
  m.P1_Tilt_01, m.P2_Tilt_01,

  /* ===== CORE (surface) ===== */
  m.P1_WinRecency_S_01,  m.P2_WinRecency_S_01,
  m.P1_LossRecency_S_01, m.P2_LossRecency_S_01,

  m.P1_MatchRecency_S_01, m.P2_MatchRecency_S_01,
  m.P1_Tilt_S_01, m.P2_Tilt_S_01,

  /* ===== DELTE & DIFF ===== */
  m.P1_WinRecency_Delta_01,  m.P2_WinRecency_Delta_01,
  m.P1_LossRecency_Delta_01, m.P2_LossRecency_Delta_01,

  m.WinRecency_Diff_01,  m.LossRecency_Diff_01,
  m.MatchRecency_Diff_01, m.Tilt_Diff_01,
  m.WinRecency_S_Diff_01, m.LossRecency_S_Diff_01,
  m.MatchRecency_S_Diff_01, m.Tilt_S_Diff_01,

  /* ===== MISSING FLAGOVI ===== */
  f.P1_WinRecency_isMissing,  f.P2_WinRecency_isMissing,
  f.P1_LossRecency_isMissing, f.P2_LossRecency_isMissing,
  f.P1_WinRecency_S_isMissing,  f.P2_WinRecency_S_isMissing,
  f.P1_LossRecency_S_isMissing, f.P2_LossRecency_S_isMissing,

  /* ===== EXPERIMENTAL (bez diffova, da ne eksplodiraju dimenzije) ===== */
  e.P1_WinRecency_H7_01,   e.P2_WinRecency_H7_01,
  e.P1_LossRecency_H7_01,  e.P2_LossRecency_H7_01,
  e.P1_WinRecency_H28_01,  e.P2_WinRecency_H28_01,
  e.P1_LossRecency_H28_01, e.P2_LossRecency_H28_01,

  e.P1_WinRecency_S_H7_01,   e.P2_WinRecency_S_H7_01,
  e.P1_LossRecency_S_H7_01,  e.P2_LossRecency_S_H7_01,
  e.P1_WinRecency_S_H28_01,  e.P2_WinRecency_S_H28_01,
  e.P1_LossRecency_S_H28_01, e.P2_LossRecency_S_H28_01,

  e.P1_WinRecency_Lin60_01,   e.P2_WinRecency_Lin60_01,
  e.P1_LossRecency_Lin60_01,  e.P2_LossRecency_Lin60_01,
  e.P1_WinRecency_S_Lin60_01, e.P2_WinRecency_S_Lin60_01,
  e.P1_LossRecency_S_Lin60_01,e.P2_LossRecency_S_Lin60_01

FROM meta m
JOIN flags f ON f.MatchTPId = m.MatchTPId
JOIN exp   e ON e.MatchTPId = m.MatchTPId;
GO
