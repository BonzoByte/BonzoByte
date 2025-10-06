CREATE VIEW dbo.vw_NN_StreakDominance_perMatch
AS
/* =========================================
   Parametri (možeš prilagoditi)
   ========================================= */
WITH param AS (
    SELECT
        -- Streak saturacija (Half-life i linearna cap)
        CAST(3.0   AS float) AS K_STREAK_HL,   -- half-life u "broju mečeva" streaka
        CAST(6.0   AS float) AS CAP_STREAK,    -- linearni cap za |streak| (0..CAP → 0..1)

        -- Smoothing i capovi za Sets/Games
        CAST(1.0   AS float) AS A_SET,         -- Laplace α za set WR
        CAST(1.0   AS float) AS A_GAME,        -- Laplace α za game WR
        CAST(200.0 AS float) AS CAP_SETS,      -- cap volumena setova
        CAST(1500.0 AS float) AS CAP_GAMES     -- cap volumena gemova
),
/* =========================================
   Sirovi ulazi (cast u float) + surface-pick za streak
   ========================================= */
base AS (
    SELECT
        m.MatchTPId,
        m.SurfaceId,

        -- Global streak
        CAST(m.Player1Streak AS float) AS P1_Streak,
        CAST(m.Player2Streak AS float) AS P2_Streak,

        -- Surface streak: uzmi baš za SurfaceId meča
        CAST(CASE m.SurfaceId
            WHEN 1 THEN m.Player1StreakS1
            WHEN 2 THEN m.Player1StreakS2
            WHEN 3 THEN m.Player1StreakS3
            WHEN 4 THEN m.Player1StreakS4
        END AS float) AS P1_Streak_S,

        CAST(CASE m.SurfaceId
            WHEN 1 THEN m.Player2StreakS1
            WHEN 2 THEN m.Player2StreakS2
            WHEN 3 THEN m.Player2StreakS3
            WHEN 4 THEN m.Player2StreakS4
        END AS float) AS P2_Streak_S,

        -- Sets / Games totals (global, pre-match snapshot)
        CAST(COALESCE(m.P1SetsWon ,0) AS float) AS P1_SW,
        CAST(COALESCE(m.P1SetsLoss,0) AS float) AS P1_SL,
        CAST(COALESCE(m.P2SetsWon ,0) AS float) AS P2_SW,
        CAST(COALESCE(m.P2SetsLoss,0) AS float) AS P2_SL,

        CAST(COALESCE(m.P1GamesWon ,0) AS float) AS P1_GW,
        CAST(COALESCE(m.P1GamesLoss,0) AS float) AS P1_GL,
        CAST(COALESCE(m.P2GamesWon ,0) AS float) AS P2_GW,
        CAST(COALESCE(m.P2GamesLoss,0) AS float) AS P2_GL
    FROM dbo.[Match] m
),
/* =========================================
   Streak → sign, magnitude, tilt (HL i linearno)
   ========================================= */
streak AS (
    SELECT
        b.MatchTPId,

        -- ----- Sign (0 porazni ← 0.5 neutralno → 1 pobjednički) -----
        CASE WHEN b.P1_Streak   IS NULL THEN 0.5
             WHEN b.P1_Streak   > 0    THEN 1.0
             WHEN b.P1_Streak   < 0    THEN 0.0
             ELSE 0.5 END AS P1_StreakSign_01,
        CASE WHEN b.P2_Streak   IS NULL THEN 0.5
             WHEN b.P2_Streak   > 0    THEN 1.0
             WHEN b.P2_Streak   < 0    THEN 0.0
             ELSE 0.5 END AS P2_StreakSign_01,

        CASE WHEN b.P1_Streak_S IS NULL THEN 0.5
             WHEN b.P1_Streak_S > 0    THEN 1.0
             WHEN b.P1_Streak_S < 0    THEN 0.0
             ELSE 0.5 END AS P1_StreakSign_S_01,
        CASE WHEN b.P2_Streak_S IS NULL THEN 0.5
             WHEN b.P2_Streak_S > 0    THEN 1.0
             WHEN b.P2_Streak_S < 0    THEN 0.0
             ELSE 0.5 END AS P2_StreakSign_S_01,

        -- ----- Magnitude (HL) -----
        CASE WHEN b.P1_Streak IS NULL THEN 0.0
             ELSE 1.0 - POWER(0.5, ABS(b.P1_Streak)/p.K_STREAK_HL) END AS P1_StreakMagHL_01,
        CASE WHEN b.P2_Streak IS NULL THEN 0.0
             ELSE 1.0 - POWER(0.5, ABS(b.P2_Streak)/p.K_STREAK_HL) END AS P2_StreakMagHL_01,

        CASE WHEN b.P1_Streak_S IS NULL THEN 0.0
             ELSE 1.0 - POWER(0.5, ABS(b.P1_Streak_S)/p.K_STREAK_HL) END AS P1_StreakMagHL_S_01,
        CASE WHEN b.P2_Streak_S IS NULL THEN 0.0
             ELSE 1.0 - POWER(0.5, ABS(b.P2_Streak_S)/p.K_STREAK_HL) END AS P2_StreakMagHL_S_01,

        -- ----- Magnitude (linear, experimental) -----
        CASE WHEN b.P1_Streak IS NULL THEN 0.0
             ELSE CASE WHEN ABS(b.P1_Streak) >= p.CAP_STREAK THEN 1.0 ELSE ABS(b.P1_Streak)/p.CAP_STREAK END END AS P1_StreakMagLIN_01,
        CASE WHEN b.P2_Streak IS NULL THEN 0.0
             ELSE CASE WHEN ABS(b.P2_Streak) >= p.CAP_STREAK THEN 1.0 ELSE ABS(b.P2_Streak)/p.CAP_STREAK END END AS P2_StreakMagLIN_01,

        CASE WHEN b.P1_Streak_S IS NULL THEN 0.0
             ELSE CASE WHEN ABS(b.P1_Streak_S) >= p.CAP_STREAK THEN 1.0 ELSE ABS(b.P1_Streak_S)/p.CAP_STREAK END END AS P1_StreakMagLIN_S_01,
        CASE WHEN b.P2_Streak_S IS NULL THEN 0.0
             ELSE CASE WHEN ABS(b.P2_Streak_S) >= p.CAP_STREAK THEN 1.0 ELSE ABS(b.P2_Streak_S)/p.CAP_STREAK END END AS P2_StreakMagLIN_S_01

    FROM base b
    CROSS JOIN param p
),
streak_tilt AS (
    SELECT
        s.MatchTPId,

        -- ----- Tilt (HL kombinacija sign & magnitude) -----
        ((s.P1_StreakSign_01 - 0.5) * s.P1_StreakMagHL_01) + 0.5 AS P1_StreakTiltHL_01,
        ((s.P2_StreakSign_01 - 0.5) * s.P2_StreakMagHL_01) + 0.5 AS P2_StreakTiltHL_01,
        ((s.P1_StreakSign_S_01 - 0.5) * s.P1_StreakMagHL_S_01) + 0.5 AS P1_StreakTiltHL_S_01,
        ((s.P2_StreakSign_S_01 - 0.5) * s.P2_StreakMagHL_S_01) + 0.5 AS P2_StreakTiltHL_S_01,

        -- ----- Tilt (linear, experimental) -----
        ((s.P1_StreakSign_01 - 0.5) * s.P1_StreakMagLIN_01) + 0.5 AS P1_StreakTiltLIN_01,
        ((s.P2_StreakSign_01 - 0.5) * s.P2_StreakMagLIN_01) + 0.5 AS P2_StreakTiltLIN_01,
        ((s.P1_StreakSign_S_01 - 0.5) * s.P1_StreakMagLIN_S_01) + 0.5 AS P1_StreakTiltLIN_S_01,
        ((s.P2_StreakSign_S_01 - 0.5) * s.P2_StreakMagLIN_S_01) + 0.5 AS P2_StreakTiltLIN_S_01,

        -- ----- Delta (surface - global), mapirano u [0,1] -----
        ((s.P1_StreakMagHL_S_01 - s.P1_StreakMagHL_01) + 1.0)/2.0 AS P1_StreakMagHL_Delta_01,
        ((s.P2_StreakMagHL_S_01 - s.P2_StreakMagHL_01) + 1.0)/2.0 AS P2_StreakMagHL_Delta_01,
        (( ((s.P1_StreakSign_S_01 - 0.5) * s.P1_StreakMagHL_S_01) + 0.5 )
         - ((s.P1_StreakSign_01   - 0.5) * s.P1_StreakMagHL_01   + 0.5 ) + 1.0)/2.0 AS P1_StreakTiltHL_Delta_01,
        (( ((s.P2_StreakSign_S_01 - 0.5) * s.P2_StreakMagHL_S_01) + 0.5 )
         - ((s.P2_StreakSign_01   - 0.5) * s.P2_StreakMagHL_01   + 0.5 ) + 1.0)/2.0 AS P2_StreakTiltHL_Delta_01

    FROM streak s
),
/* =========================================
   Sets/Games → WR, VOL, dominance, gap/efficiency
   ========================================= */
sg AS (
    SELECT
        b.MatchTPId,

        -- Denominatori
        (b.P1_SW + b.P1_SL) AS P1_S_TOT,
        (b.P2_SW + b.P2_SL) AS P2_S_TOT,
        (b.P1_GW + b.P1_GL) AS P1_G_TOT,
        (b.P2_GW + b.P2_GL) AS P2_G_TOT,

        -- Win rate (Laplace)
        CASE WHEN (b.P1_SW + b.P1_SL)=0 THEN 0.5 ELSE (b.P1_SW + p.A_SET)  / (b.P1_SW + b.P1_SL + 2*p.A_SET)  END AS P1_SetWR_01,
        CASE WHEN (b.P2_SW + b.P2_SL)=0 THEN 0.5 ELSE (b.P2_SW + p.A_SET)  / (b.P2_SW + b.P2_SL + 2*p.A_SET)  END AS P2_SetWR_01,
        CASE WHEN (b.P1_GW + b.P1_GL)=0 THEN 0.5 ELSE (b.P1_GW + p.A_GAME) / (b.P1_GW + b.P1_GL + 2*p.A_GAME) END AS P1_GameWR_01,
        CASE WHEN (b.P2_GW + b.P2_GL)=0 THEN 0.5 ELSE (b.P2_GW + p.A_GAME) / (b.P2_GW + b.P2_GL + 2*p.A_GAME) END AS P2_GameWR_01,

        -- Volume caps
        CASE WHEN (b.P1_SW + b.P1_SL)=0 THEN 0 ELSE CASE WHEN (b.P1_SW + b.P1_SL) >= p.CAP_SETS  THEN 1 ELSE (b.P1_SW + b.P1_SL)/p.CAP_SETS  END END AS P1_SetVOL_01,
        CASE WHEN (b.P2_SW + b.P2_SL)=0 THEN 0 ELSE CASE WHEN (b.P2_SW + b.P2_SL) >= p.CAP_SETS  THEN 1 ELSE (b.P2_SW + b.P2_SL)/p.CAP_SETS  END END AS P2_SetVOL_01,
        CASE WHEN (b.P1_GW + b.P1_GL)=0 THEN 0 ELSE CASE WHEN (b.P1_GW + b.P1_GL) >= p.CAP_GAMES THEN 1 ELSE (b.P1_GW + b.P1_GL)/p.CAP_GAMES END END AS P1_GameVOL_01,
        CASE WHEN (b.P2_GW + b.P2_GL)=0 THEN 0 ELSE CASE WHEN (b.P2_GW + b.P2_GL) >= p.CAP_GAMES THEN 1 ELSE (b.P2_GW + b.P2_GL)/p.CAP_GAMES END END AS P2_GameVOL_01,

        -- Dominance (margin normalized)
        CASE WHEN (b.P1_SW + b.P1_SL)=0 THEN 0.5 ELSE ((b.P1_SW - b.P1_SL)/NULLIF(b.P1_SW + b.P1_SL,0) + 1.0)/2.0 END AS P1_SetDom_01,
        CASE WHEN (b.P2_SW + b.P2_SL)=0 THEN 0.5 ELSE ((b.P2_SW - b.P2_SL)/NULLIF(b.P2_SW + b.P2_SL,0) + 1.0)/2.0 END AS P2_SetDom_01,
        CASE WHEN (b.P1_GW + b.P1_GL)=0 THEN 0.5 ELSE ((b.P1_GW - b.P1_GL)/NULLIF(b.P1_GW + b.P1_GL,0) + 1.0)/2.0 END AS P1_GameDom_01,
        CASE WHEN (b.P2_GW + b.P2_GL)=0 THEN 0.5 ELSE ((b.P2_GW - b.P2_GL)/NULLIF(b.P2_GW + b.P2_GL,0) + 1.0)/2.0 END AS P2_GameDom_01

    FROM base b
    CROSS JOIN param p
),
sg_meta AS (
    SELECT
        s.MatchTPId,

        -- Gap Set vs Game (sign + abs)
        ((s.P1_SetWR_01  - s.P1_GameWR_01) + 1.0)/2.0 AS P1_SetVsGameGap_01,
        ((s.P2_SetWR_01  - s.P2_GameWR_01) + 1.0)/2.0 AS P2_SetVsGameGap_01,
        ABS(s.P1_SetWR_01 - s.P1_GameWR_01)           AS P1_SetVsGameGapAbs_01,
        ABS(s.P2_SetWR_01 - s.P2_GameWR_01)           AS P2_SetVsGameGapAbs_01,

        -- "Efficiency": dominance × volume
        (s.P1_SetDom_01  * s.P1_SetVOL_01)  AS P1_SetDomEff_01,
        (s.P2_SetDom_01  * s.P2_SetVOL_01)  AS P2_SetDomEff_01,
        (s.P1_GameDom_01 * s.P1_GameVOL_01) AS P1_GameDomEff_01,
        (s.P2_GameDom_01 * s.P2_GameVOL_01) AS P2_GameDomEff_01
    FROM sg s
),
/* =========================================
   Flagovi (missing)
   ========================================= */
flags AS (
    SELECT
        b.MatchTPId,

        -- Streak missing (NULL)
        CASE WHEN b.P1_Streak   IS NULL THEN 1 ELSE 0 END AS P1_Streak_isMissing,
        CASE WHEN b.P2_Streak   IS NULL THEN 1 ELSE 0 END AS P2_Streak_isMissing,
        CASE WHEN b.P1_Streak_S IS NULL THEN 1 ELSE 0 END AS P1_Streak_S_isMissing,
        CASE WHEN b.P2_Streak_S IS NULL THEN 1 ELSE 0 END AS P2_Streak_S_isMissing,

        -- Sets/Games missing (nema volumena)
        CASE WHEN (b.P1_SW + b.P1_SL)=0 THEN 1 ELSE 0 END AS P1_Set_isMissing,
        CASE WHEN (b.P2_SW + b.P2_SL)=0 THEN 1 ELSE 0 END AS P2_Set_isMissing,
        CASE WHEN (b.P1_GW + b.P1_GL)=0 THEN 1 ELSE 0 END AS P1_Game_isMissing,
        CASE WHEN (b.P2_GW + b.P2_GL)=0 THEN 1 ELSE 0 END AS P2_Game_isMissing
    FROM base b
)

/* =========================================
   FINAL: feature set
   ========================================= */
SELECT
    b.MatchTPId,

    /* ---------- Streak (CORE: HL varijanta) ---------- */
    s.P1_StreakSign_01,    s.P2_StreakSign_01,
    s.P1_StreakMagHL_01,   s.P2_StreakMagHL_01,
    t.P1_StreakTiltHL_01,  t.P2_StreakTiltHL_01,

    s.P1_StreakSign_S_01,  s.P2_StreakSign_S_01,
    s.P1_StreakMagHL_S_01, s.P2_StreakMagHL_S_01,
    t.P1_StreakTiltHL_S_01,t.P2_StreakTiltHL_S_01,

    t.P1_StreakMagHL_Delta_01, t.P2_StreakMagHL_Delta_01,
    t.P1_StreakTiltHL_Delta_01,t.P2_StreakTiltHL_Delta_01,

    -- Diff (HL)
    ((t.P1_StreakTiltHL_01   - t.P2_StreakTiltHL_01  ) + 1.0)/2.0 AS StreakTiltHL_Diff_01,
    ((t.P1_StreakTiltHL_S_01 - t.P2_StreakTiltHL_S_01) + 1.0)/2.0 AS StreakTiltHL_S_Diff_01,
    ((s.P1_StreakMagHL_01    - s.P2_StreakMagHL_01   ) + 1.0)/2.0 AS StreakMagHL_Diff_01,

    /* ---------- Streak (EXPERIMENTAL: linear) ---------- */
    s.P1_StreakMagLIN_01,   s.P2_StreakMagLIN_01,
    s.P1_StreakMagLIN_S_01, s.P2_StreakMagLIN_S_01,
    t.P1_StreakTiltLIN_01,  t.P2_StreakTiltLIN_01,
    t.P1_StreakTiltLIN_S_01,t.P2_StreakTiltLIN_S_01,

    /* ---------- Sets/Games (CORE) ---------- */
    sg.P1_SetWR_01,  sg.P2_SetWR_01,
    sg.P1_GameWR_01, sg.P2_GameWR_01,
    sg.P1_SetVOL_01, sg.P2_SetVOL_01,
    sg.P1_GameVOL_01,sg.P2_GameVOL_01,
    sg.P1_SetDom_01, sg.P2_SetDom_01,
    sg.P1_GameDom_01,sg.P2_GameDom_01,

    -- Diff (core)
    ((sg.P1_SetWR_01   - sg.P2_SetWR_01  ) + 1.0)/2.0 AS SetWR_Diff_01,
    ((sg.P1_GameWR_01  - sg.P2_GameWR_01 ) + 1.0)/2.0 AS GameWR_Diff_01,
    ((sg.P1_SetDom_01  - sg.P2_SetDom_01 ) + 1.0)/2.0 AS SetDom_Diff_01,
    ((sg.P1_GameDom_01 - sg.P2_GameDom_01) + 1.0)/2.0 AS GameDom_Diff_01,

    /* ---------- Sets/Games (EXPERIMENTAL) ---------- */
    sm.P1_SetVsGameGap_01,    sm.P2_SetVsGameGap_01,
    sm.P1_SetVsGameGapAbs_01, sm.P2_SetVsGameGapAbs_01,
    sm.P1_SetDomEff_01,       sm.P2_SetDomEff_01,
    sm.P1_GameDomEff_01,      sm.P2_GameDomEff_01,

    ((sm.P1_SetDomEff_01  - sm.P2_SetDomEff_01 ) + 1.0)/2.0 AS SetDomEff_Diff_01,
    ((sm.P1_GameDomEff_01 - sm.P2_GameDomEff_01) + 1.0)/2.0 AS GameDomEff_Diff_01,
    ((sm.P1_SetVsGameGap_01 - sm.P2_SetVsGameGap_01) + 1.0)/2.0 AS SetVsGameGap_Diff_01,

    /* ---------- Flagovi ---------- */
    f.P1_Streak_isMissing, f.P2_Streak_isMissing,
    f.P1_Streak_S_isMissing, f.P2_Streak_S_isMissing,
    f.P1_Set_isMissing, f.P2_Set_isMissing,
    f.P1_Game_isMissing, f.P2_Game_isMissing

FROM base b
JOIN streak       s  ON s.MatchTPId = b.MatchTPId
JOIN streak_tilt  t  ON t.MatchTPId = b.MatchTPId
JOIN sg           sg ON sg.MatchTPId = b.MatchTPId
JOIN sg_meta      sm ON sm.MatchTPId = b.MatchTPId
JOIN flags        f  ON f.MatchTPId = b.MatchTPId
CROSS JOIN param p;  -- (rezervirano ako želiš “emitirati” parametre)
GO
