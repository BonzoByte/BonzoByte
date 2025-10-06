CREATE VIEW dbo.vw_NN_Form_perMatch
AS
/* =========================
   Parametri (podesivo)
   ========================= */
WITH param AS (
    SELECT 
        -- Smoothing alpha (veće za kratke prozore)
        CAST(1.0 AS float) AS A_FM_T,  CAST(1.0 AS float) AS A_FM_Y,  CAST(2.0 AS float) AS A_FM_M,  CAST(2.0 AS float) AS A_FM_W,
        CAST(1.0 AS float) AS A_FS_T,  CAST(1.0 AS float) AS A_FS_Y,  CAST(2.0 AS float) AS A_FS_M,  CAST(2.0 AS float) AS A_FS_W,
        CAST(1.0 AS float) AS A_FG_T,  CAST(1.0 AS float) AS A_FG_Y,  CAST(2.0 AS float) AS A_FG_M,  CAST(2.0 AS float) AS A_FG_W,

        -- Volume caps (exposure) – siguran default; kasnije "zamrzni" iz TRAIN-a
        CAST(200.0 AS float) AS C_FM_T, CAST(60.0 AS float) AS C_FM_Y, CAST(15.0 AS float) AS C_FM_M, CAST(5.0 AS float) AS C_FM_W,
        CAST(400.0 AS float) AS C_FS_T, CAST(120.0 AS float) AS C_FS_Y, CAST(30.0 AS float) AS C_FS_M, CAST(10.0 AS float) AS C_FS_W,
        CAST(2000.0 AS float) AS C_FG_T,CAST(600.0 AS float) AS C_FG_Y, CAST(150.0 AS float) AS C_FG_M, CAST(50.0 AS float) AS C_FG_W
),
/* =========================
   Sirovi brojači (W/L), cast u float i surface-pick S1..S4
   ========================= */
base AS (
    SELECT
        m.MatchTPId,
        m.SurfaceId,

        -- ===== MATCH (agnostično) P1
        CAST(COALESCE(m.Player1WinsTotal,0)       AS float) AS P1_FM_W_T,
        CAST(COALESCE(m.Player1LossesTotal,0)     AS float) AS P1_FM_L_T,
        CAST(COALESCE(m.Player1WinsLastYear,0)    AS float) AS P1_FM_W_Y,
        CAST(COALESCE(m.Player1LossesLastYear,0)  AS float) AS P1_FM_L_Y,
        CAST(COALESCE(m.Player1WinsLastMonth,0)   AS float) AS P1_FM_W_M,
        CAST(COALESCE(m.Player1LossesLastMonth,0) AS float) AS P1_FM_L_M,
        CAST(COALESCE(m.Player1WinsLastWeek,0)    AS float) AS P1_FM_W_W,
        CAST(COALESCE(m.Player1LossesLastWeek,0)  AS float) AS P1_FM_L_W,

        -- ===== MATCH (agnostično) P2
        CAST(COALESCE(m.Player2WinsTotal,0)       AS float) AS P2_FM_W_T,
        CAST(COALESCE(m.Player2LossesTotal,0)     AS float) AS P2_FM_L_T,
        CAST(COALESCE(m.Player2WinsLastYear,0)    AS float) AS P2_FM_W_Y,
        CAST(COALESCE(m.Player2LossesLastYear,0)  AS float) AS P2_FM_L_Y,
        CAST(COALESCE(m.Player2WinsLastMonth,0)   AS float) AS P2_FM_W_M,
        CAST(COALESCE(m.Player2LossesLastMonth,0) AS float) AS P2_FM_L_M,
        CAST(COALESCE(m.Player2WinsLastWeek,0)    AS float) AS P2_FM_W_W,
        CAST(COALESCE(m.Player2LossesLastWeek,0)  AS float) AS P2_FM_L_W,

        -- ===== MATCH (surface) P1
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsTotalS4,0) END AS float) AS P1_FM_S_W_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesTotalS4,0) END AS float) AS P1_FM_S_L_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsLastYearS4,0) END AS float) AS P1_FM_S_W_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesLastYearS4,0) END AS float) AS P1_FM_S_L_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsLastMonthS4,0) END AS float) AS P1_FM_S_W_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesLastMonthS4,0) END AS float) AS P1_FM_S_L_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsLastWeekS4,0) END AS float) AS P1_FM_S_W_W,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesLastWeekS4,0) END AS float) AS P1_FM_S_L_W,

        -- ===== MATCH (surface) P2
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsTotalS4,0) END AS float) AS P2_FM_S_W_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesTotalS4,0) END AS float) AS P2_FM_S_L_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsLastYearS4,0) END AS float) AS P2_FM_S_W_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesLastYearS4,0) END AS float) AS P2_FM_S_L_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsLastMonthS4,0) END AS float) AS P2_FM_S_W_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesLastMonthS4,0) END AS float) AS P2_FM_S_L_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsLastWeekS4,0) END AS float) AS P2_FM_S_W_W,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesLastWeekS4,0) END AS float) AS P2_FM_S_L_W,

        /* ===== SETS (agnostično) P1/P2 */
        CAST(COALESCE(m.Player1WinsSetsTotal,0)       AS float) AS P1_FS_W_T,
        CAST(COALESCE(m.Player1LossesSetsTotal,0)     AS float) AS P1_FS_L_T,
        CAST(COALESCE(m.Player1WinsSetsLastYear,0)    AS float) AS P1_FS_W_Y,
        CAST(COALESCE(m.Player1LossesSetsLastYear,0)  AS float) AS P1_FS_L_Y,
        CAST(COALESCE(m.Player1WinsSetsLastMonth,0)   AS float) AS P1_FS_W_M,
        CAST(COALESCE(m.Player1LossesSetsLastMonth,0) AS float) AS P1_FS_L_M,
        CAST(COALESCE(m.Player1WinsSetsLastWeek,0)    AS float) AS P1_FS_W_W,
        CAST(COALESCE(m.Player1LossesSetsLastWeek,0)  AS float) AS P1_FS_L_W,

        CAST(COALESCE(m.Player2WinsSetsTotal,0)       AS float) AS P2_FS_W_T,
        CAST(COALESCE(m.Player2LossesSetsTotal,0)     AS float) AS P2_FS_L_T,
        CAST(COALESCE(m.Player2WinsSetsLastYear,0)    AS float) AS P2_FS_W_Y,
        CAST(COALESCE(m.Player2LossesSetsLastYear,0)  AS float) AS P2_FS_L_Y,
        CAST(COALESCE(m.Player2WinsSetsLastMonth,0)   AS float) AS P2_FS_W_M,
        CAST(COALESCE(m.Player2LossesSetsLastMonth,0) AS float) AS P2_FS_L_M,
        CAST(COALESCE(m.Player2WinsSetsLastWeek,0)    AS float) AS P2_FS_W_W,
        CAST(COALESCE(m.Player2LossesSetsLastWeek,0)  AS float) AS P2_FS_L_W,

        /* ===== SETS (surface) P1/P2 */
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsSetsTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsSetsTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsSetsTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsSetsTotalS4,0) END AS float) AS P1_FS_S_W_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesSetsTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesSetsTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesSetsTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesSetsTotalS4,0) END AS float) AS P1_FS_S_L_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsSetsLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsSetsLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsSetsLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsSetsLastYearS4,0) END AS float) AS P1_FS_S_W_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesSetsLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesSetsLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesSetsLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesSetsLastYearS4,0) END AS float) AS P1_FS_S_L_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsSetsLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsSetsLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsSetsLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsSetsLastMonthS4,0) END AS float) AS P1_FS_S_W_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesSetsLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesSetsLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesSetsLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesSetsLastMonthS4,0) END AS float) AS P1_FS_S_L_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsSetsLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsSetsLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsSetsLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsSetsLastWeekS4,0) END AS float) AS P1_FS_S_W_W,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesSetsLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesSetsLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesSetsLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesSetsLastWeekS4,0) END AS float) AS P1_FS_S_L_W,

        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsSetsTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsSetsTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsSetsTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsSetsTotalS4,0) END AS float) AS P2_FS_S_W_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesSetsTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesSetsTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesSetsTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesSetsTotalS4,0) END AS float) AS P2_FS_S_L_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsSetsLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsSetsLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsSetsLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsSetsLastYearS4,0) END AS float) AS P2_FS_S_W_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesSetsLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesSetsLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesSetsLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesSetsLastYearS4,0) END AS float) AS P2_FS_S_L_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsSetsLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsSetsLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsSetsLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsSetsLastMonthS4,0) END AS float) AS P2_FS_S_W_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesSetsLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesSetsLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesSetsLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesSetsLastMonthS4,0) END AS float) AS P2_FS_S_L_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsSetsLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsSetsLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsSetsLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsSetsLastWeekS4,0) END AS float) AS P2_FS_S_W_W,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesSetsLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesSetsLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesSetsLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesSetsLastWeekS4,0) END AS float) AS P2_FS_S_L_W,

        /* ===== GAMES (agnostično) P1/P2 */
        CAST(COALESCE(m.Player1WinsGamesTotal,0)       AS float) AS P1_FG_W_T,
        CAST(COALESCE(m.Player1LossesGamesTotal,0)     AS float) AS P1_FG_L_T,
        CAST(COALESCE(m.Player1WinsGamesLastYear,0)    AS float) AS P1_FG_W_Y,
        CAST(COALESCE(m.Player1LossesGamesLastYear,0)  AS float) AS P1_FG_L_Y,
        CAST(COALESCE(m.Player1WinsGamesLastMonth,0)   AS float) AS P1_FG_W_M,
        CAST(COALESCE(m.Player1LossesGamesLastMonth,0) AS float) AS P1_FG_L_M,
        CAST(COALESCE(m.Player1WinsGamesLastWeek,0)    AS float) AS P1_FG_W_W,
        CAST(COALESCE(m.Player1LossesGamesLastWeek,0)  AS float) AS P1_FG_L_W,

        CAST(COALESCE(m.Player2WinsGamesTotal,0)       AS float) AS P2_FG_W_T,
        CAST(COALESCE(m.Player2LossesGamesTotal,0)     AS float) AS P2_FG_L_T,
        CAST(COALESCE(m.Player2WinsGamesLastYear,0)    AS float) AS P2_FG_W_Y,
        CAST(COALESCE(m.Player2LossesGamesLastYear,0)  AS float) AS P2_FG_L_Y,
        CAST(COALESCE(m.Player2WinsGamesLastMonth,0)   AS float) AS P2_FG_W_M,
        CAST(COALESCE(m.Player2LossesGamesLastMonth,0) AS float) AS P2_FG_L_M,
        CAST(COALESCE(m.Player2WinsGamesLastWeek,0)    AS float) AS P2_FG_W_W,
        CAST(COALESCE(m.Player2LossesGamesLastWeek,0)  AS float) AS P2_FG_L_W,

        /* ===== GAMES (surface) P1/P2 */
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsGamesTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsGamesTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsGamesTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsGamesTotalS4,0) END AS float) AS P1_FG_S_W_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesGamesTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesGamesTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesGamesTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesGamesTotalS4,0) END AS float) AS P1_FG_S_L_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsGamesLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsGamesLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsGamesLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsGamesLastYearS4,0) END AS float) AS P1_FG_S_W_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesGamesLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesGamesLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesGamesLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesGamesLastYearS4,0) END AS float) AS P1_FG_S_L_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsGamesLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsGamesLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsGamesLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsGamesLastMonthS4,0) END AS float) AS P1_FG_S_W_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesGamesLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesGamesLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesGamesLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesGamesLastMonthS4,0) END AS float) AS P1_FG_S_L_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1WinsGamesLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player1WinsGamesLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player1WinsGamesLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player1WinsGamesLastWeekS4,0) END AS float) AS P1_FG_S_W_W,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player1LossesGamesLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player1LossesGamesLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player1LossesGamesLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player1LossesGamesLastWeekS4,0) END AS float) AS P1_FG_S_L_W,

        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsGamesTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsGamesTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsGamesTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsGamesTotalS4,0) END AS float) AS P2_FG_S_W_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesGamesTotalS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesGamesTotalS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesGamesTotalS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesGamesTotalS4,0) END AS float) AS P2_FG_S_L_T,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsGamesLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsGamesLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsGamesLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsGamesLastYearS4,0) END AS float) AS P2_FG_S_W_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesGamesLastYearS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesGamesLastYearS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesGamesLastYearS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesGamesLastYearS4,0) END AS float) AS P2_FG_S_L_Y,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsGamesLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsGamesLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsGamesLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsGamesLastMonthS4,0) END AS float) AS P2_FG_S_W_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesGamesLastMonthS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesGamesLastMonthS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesGamesLastMonthS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesGamesLastMonthS4,0) END AS float) AS P2_FG_S_L_M,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2WinsGamesLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player2WinsGamesLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player2WinsGamesLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player2WinsGamesLastWeekS4,0) END AS float) AS P2_FG_S_W_W,
        CAST(CASE m.SurfaceId WHEN 1 THEN COALESCE(m.Player2LossesGamesLastWeekS1,0)
                              WHEN 2 THEN COALESCE(m.Player2LossesGamesLastWeekS2,0)
                              WHEN 3 THEN COALESCE(m.Player2LossesGamesLastWeekS3,0)
                              WHEN 4 THEN COALESCE(m.Player2LossesGamesLastWeekS4,0) END AS float) AS P2_FG_S_L_W
    FROM dbo.[Match] m
),
/* =========================
   Izračun WR / VOL / Missing (smoothed), delta i diff
   ========================= */
calc AS (
    SELECT
      b.MatchTPId,
      /* kratice za parametre radi čitljivosti */
      p.A_FM_T, p.A_FM_Y, p.A_FM_M, p.A_FM_W,
      p.A_FS_T, p.A_FS_Y, p.A_FS_M, p.A_FS_W,
      p.A_FG_T, p.A_FG_Y, p.A_FG_M, p.A_FG_W,
      p.C_FM_T, p.C_FM_Y, p.C_FM_M, p.C_FM_W,
      p.C_FS_T, p.C_FS_Y, p.C_FS_M, p.C_FS_W,
      p.C_FG_T, p.C_FG_Y, p.C_FG_M, p.C_FG_W,

      /* ========== MATCH (agnostično) P1 ==========
         WR = (W+α)/(W+L+2α), VOL = min((W+L)/cap,1)  */
      CASE WHEN (b.P1_FM_W_T + b.P1_FM_L_T)=0 THEN 0.5 ELSE (b.P1_FM_W_T + p.A_FM_T)/(b.P1_FM_W_T + b.P1_FM_L_T + 2*p.A_FM_T) END AS P1_FM_WR_T_01,
      CASE WHEN (b.P1_FM_W_Y + b.P1_FM_L_Y)=0 THEN 0.5 ELSE (b.P1_FM_W_Y + p.A_FM_Y)/(b.P1_FM_W_Y + b.P1_FM_L_Y + 2*p.A_FM_Y) END AS P1_FM_WR_Y_01,
      CASE WHEN (b.P1_FM_W_M + b.P1_FM_L_M)=0 THEN 0.5 ELSE (b.P1_FM_W_M + p.A_FM_M)/(b.P1_FM_W_M + b.P1_FM_L_M + 2*p.A_FM_M) END AS P1_FM_WR_M_01,
      CASE WHEN (b.P1_FM_W_W + b.P1_FM_L_W)=0 THEN 0.5 ELSE (b.P1_FM_W_W + p.A_FM_W)/(b.P1_FM_W_W + b.P1_FM_L_W + 2*p.A_FM_W) END AS P1_FM_WR_W_01,

      CASE WHEN (b.P1_FM_W_T + b.P1_FM_L_T)=0 THEN 0 ELSE CASE WHEN (b.P1_FM_W_T + b.P1_FM_L_T) >= p.C_FM_T THEN 1 ELSE (b.P1_FM_W_T + b.P1_FM_L_T)/p.C_FM_T END END AS P1_FM_VOL_T_01,
      CASE WHEN (b.P1_FM_W_Y + b.P1_FM_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P1_FM_W_Y + b.P1_FM_L_Y) >= p.C_FM_Y THEN 1 ELSE (b.P1_FM_W_Y + b.P1_FM_L_Y)/p.C_FM_Y END END AS P1_FM_VOL_Y_01,
      CASE WHEN (b.P1_FM_W_M + b.P1_FM_L_M)=0 THEN 0 ELSE CASE WHEN (b.P1_FM_W_M + b.P1_FM_L_M) >= p.C_FM_M THEN 1 ELSE (b.P1_FM_W_M + b.P1_FM_L_M)/p.C_FM_M END END AS P1_FM_VOL_M_01,
      CASE WHEN (b.P1_FM_W_W + b.P1_FM_L_W)=0 THEN 0 ELSE CASE WHEN (b.P1_FM_W_W + b.P1_FM_L_W) >= p.C_FM_W THEN 1 ELSE (b.P1_FM_W_W + b.P1_FM_L_W)/p.C_FM_W END END AS P1_FM_VOL_W_01,

      CASE WHEN (b.P1_FM_W_T + b.P1_FM_L_T)=0 THEN 1 ELSE 0 END AS P1_FM_WR_T_isMissing,
      CASE WHEN (b.P1_FM_W_Y + b.P1_FM_L_Y)=0 THEN 1 ELSE 0 END AS P1_FM_WR_Y_isMissing,
      CASE WHEN (b.P1_FM_W_M + b.P1_FM_L_M)=0 THEN 1 ELSE 0 END AS P1_FM_WR_M_isMissing,
      CASE WHEN (b.P1_FM_W_W + b.P1_FM_L_W)=0 THEN 1 ELSE 0 END AS P1_FM_WR_W_isMissing,

      /* ========== MATCH (agnostično) P2 ========== */
      CASE WHEN (b.P2_FM_W_T + b.P2_FM_L_T)=0 THEN 0.5 ELSE (b.P2_FM_W_T + p.A_FM_T)/(b.P2_FM_W_T + b.P2_FM_L_T + 2*p.A_FM_T) END AS P2_FM_WR_T_01,
      CASE WHEN (b.P2_FM_W_Y + b.P2_FM_L_Y)=0 THEN 0.5 ELSE (b.P2_FM_W_Y + p.A_FM_Y)/(b.P2_FM_W_Y + b.P2_FM_L_Y + 2*p.A_FM_Y) END AS P2_FM_WR_Y_01,
      CASE WHEN (b.P2_FM_W_M + b.P2_FM_L_M)=0 THEN 0.5 ELSE (b.P2_FM_W_M + p.A_FM_M)/(b.P2_FM_W_M + b.P2_FM_L_M + 2*p.A_FM_M) END AS P2_FM_WR_M_01,
      CASE WHEN (b.P2_FM_W_W + b.P2_FM_L_W)=0 THEN 0.5 ELSE (b.P2_FM_W_W + p.A_FM_W)/(b.P2_FM_W_W + b.P2_FM_L_W + 2*p.A_FM_W) END AS P2_FM_WR_W_01,

      CASE WHEN (b.P2_FM_W_T + b.P2_FM_L_T)=0 THEN 0 ELSE CASE WHEN (b.P2_FM_W_T + b.P2_FM_L_T) >= p.C_FM_T THEN 1 ELSE (b.P2_FM_W_T + b.P2_FM_L_T)/p.C_FM_T END END AS P2_FM_VOL_T_01,
      CASE WHEN (b.P2_FM_W_Y + b.P2_FM_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P2_FM_W_Y + b.P2_FM_L_Y) >= p.C_FM_Y THEN 1 ELSE (b.P2_FM_W_Y + b.P2_FM_L_Y)/p.C_FM_Y END END AS P2_FM_VOL_Y_01,
      CASE WHEN (b.P2_FM_W_M + b.P2_FM_L_M)=0 THEN 0 ELSE CASE WHEN (b.P2_FM_W_M + b.P2_FM_L_M) >= p.C_FM_M THEN 1 ELSE (b.P2_FM_W_M + b.P2_FM_L_M)/p.C_FM_M END END AS P2_FM_VOL_M_01,
      CASE WHEN (b.P2_FM_W_W + b.P2_FM_L_W)=0 THEN 0 ELSE CASE WHEN (b.P2_FM_W_W + b.P2_FM_L_W) >= p.C_FM_W THEN 1 ELSE (b.P2_FM_W_W + b.P2_FM_L_W)/p.C_FM_W END END AS P2_FM_VOL_W_01,

      CASE WHEN (b.P2_FM_W_T + b.P2_FM_L_T)=0 THEN 1 ELSE 0 END AS P2_FM_WR_T_isMissing,
      CASE WHEN (b.P2_FM_W_Y + b.P2_FM_L_Y)=0 THEN 1 ELSE 0 END AS P2_FM_WR_Y_isMissing,
      CASE WHEN (b.P2_FM_W_M + b.P2_FM_L_M)=0 THEN 1 ELSE 0 END AS P2_FM_WR_M_isMissing,
      CASE WHEN (b.P2_FM_W_W + b.P2_FM_L_W)=0 THEN 1 ELSE 0 END AS P2_FM_WR_W_isMissing,

      /* ========== MATCH (surface) P1 ========== */
      CASE WHEN (b.P1_FM_S_W_T + b.P1_FM_S_L_T)=0 THEN 0.5 ELSE (b.P1_FM_S_W_T + p.A_FM_T)/(b.P1_FM_S_W_T + b.P1_FM_S_L_T + 2*p.A_FM_T) END AS P1_FM_WR_T_Surf_01,
      CASE WHEN (b.P1_FM_S_W_Y + b.P1_FM_S_L_Y)=0 THEN 0.5 ELSE (b.P1_FM_S_W_Y + p.A_FM_Y)/(b.P1_FM_S_W_Y + b.P1_FM_S_L_Y + 2*p.A_FM_Y) END AS P1_FM_WR_Y_Surf_01,
      CASE WHEN (b.P1_FM_S_W_M + b.P1_FM_S_L_M)=0 THEN 0.5 ELSE (b.P1_FM_S_W_M + p.A_FM_M)/(b.P1_FM_S_W_M + b.P1_FM_S_L_M + 2*p.A_FM_M) END AS P1_FM_WR_M_Surf_01,
      CASE WHEN (b.P1_FM_S_W_W + b.P1_FM_S_L_W)=0 THEN 0.5 ELSE (b.P1_FM_S_W_W + p.A_FM_W)/(b.P1_FM_S_W_W + b.P1_FM_S_L_W + 2*p.A_FM_W) END AS P1_FM_WR_W_Surf_01,

      CASE WHEN (b.P1_FM_S_W_T + b.P1_FM_S_L_T)=0 THEN 0 ELSE CASE WHEN (b.P1_FM_S_W_T + b.P1_FM_S_L_T) >= p.C_FM_T THEN 1 ELSE (b.P1_FM_S_W_T + b.P1_FM_S_L_T)/p.C_FM_T END END AS P1_FM_VOL_T_Surf_01,
      CASE WHEN (b.P1_FM_S_W_Y + b.P1_FM_S_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P1_FM_S_W_Y + b.P1_FM_S_L_Y) >= p.C_FM_Y THEN 1 ELSE (b.P1_FM_S_W_Y + b.P1_FM_S_L_Y)/p.C_FM_Y END END AS P1_FM_VOL_Y_Surf_01,
      CASE WHEN (b.P1_FM_S_W_M + b.P1_FM_S_L_M)=0 THEN 0 ELSE CASE WHEN (b.P1_FM_S_W_M + b.P1_FM_S_L_M) >= p.C_FM_M THEN 1 ELSE (b.P1_FM_S_W_M + b.P1_FM_S_L_M)/p.C_FM_M END END AS P1_FM_VOL_M_Surf_01,
      CASE WHEN (b.P1_FM_S_W_W + b.P1_FM_S_L_W)=0 THEN 0 ELSE CASE WHEN (b.P1_FM_S_W_W + b.P1_FM_S_L_W) >= p.C_FM_W THEN 1 ELSE (b.P1_FM_S_W_W + b.P1_FM_S_L_W)/p.C_FM_W END END AS P1_FM_VOL_W_Surf_01,

      CASE WHEN (b.P1_FM_S_W_T + b.P1_FM_S_L_T)=0 THEN 1 ELSE 0 END AS P1_FM_WR_T_Surf_isMissing,
      CASE WHEN (b.P1_FM_S_W_Y + b.P1_FM_S_L_Y)=0 THEN 1 ELSE 0 END AS P1_FM_WR_Y_Surf_isMissing,
      CASE WHEN (b.P1_FM_S_W_M + b.P1_FM_S_L_M)=0 THEN 1 ELSE 0 END AS P1_FM_WR_M_Surf_isMissing,
      CASE WHEN (b.P1_FM_S_W_W + b.P1_FM_S_L_W)=0 THEN 1 ELSE 0 END AS P1_FM_WR_W_Surf_isMissing,

      /* ========== MATCH (surface) P2 ========== */
      CASE WHEN (b.P2_FM_S_W_T + b.P2_FM_S_L_T)=0 THEN 0.5 ELSE (b.P2_FM_S_W_T + p.A_FM_T)/(b.P2_FM_S_W_T + b.P2_FM_S_L_T + 2*p.A_FM_T) END AS P2_FM_WR_T_Surf_01,
      CASE WHEN (b.P2_FM_S_W_Y + b.P2_FM_S_L_Y)=0 THEN 0.5 ELSE (b.P2_FM_S_W_Y + p.A_FM_Y)/(b.P2_FM_S_W_Y + b.P2_FM_S_L_Y + 2*p.A_FM_Y) END AS P2_FM_WR_Y_Surf_01,
      CASE WHEN (b.P2_FM_S_W_M + b.P2_FM_S_L_M)=0 THEN 0.5 ELSE (b.P2_FM_S_W_M + p.A_FM_M)/(b.P2_FM_S_W_M + b.P2_FM_S_L_M + 2*p.A_FM_M) END AS P2_FM_WR_M_Surf_01,
      CASE WHEN (b.P2_FM_S_W_W + b.P2_FM_S_L_W)=0 THEN 0.5 ELSE (b.P2_FM_S_W_W + p.A_FM_W)/(b.P2_FM_S_W_W + b.P2_FM_S_L_W + 2*p.A_FM_W) END AS P2_FM_WR_W_Surf_01,

      CASE WHEN (b.P2_FM_S_W_T + b.P2_FM_S_L_T)=0 THEN 0 ELSE CASE WHEN (b.P2_FM_S_W_T + b.P2_FM_S_L_T) >= p.C_FM_T THEN 1 ELSE (b.P2_FM_S_W_T + b.P2_FM_S_L_T)/p.C_FM_T END END AS P2_FM_VOL_T_Surf_01,
      CASE WHEN (b.P2_FM_S_W_Y + b.P2_FM_S_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P2_FM_S_W_Y + b.P2_FM_S_L_Y) >= p.C_FM_Y THEN 1 ELSE (b.P2_FM_S_W_Y + b.P2_FM_S_L_Y)/p.C_FM_Y END END AS P2_FM_VOL_Y_Surf_01,
      CASE WHEN (b.P2_FM_S_W_M + b.P2_FM_S_L_M)=0 THEN 0 ELSE CASE WHEN (b.P2_FM_S_W_M + b.P2_FM_S_L_M) >= p.C_FM_M THEN 1 ELSE (b.P2_FM_S_W_M + b.P2_FM_S_L_M)/p.C_FM_M END END AS P2_FM_VOL_M_Surf_01,
      CASE WHEN (b.P2_FM_S_W_W + b.P2_FM_S_L_W)=0 THEN 0 ELSE CASE WHEN (b.P2_FM_S_W_W + b.P2_FM_S_L_W) >= p.C_FM_W THEN 1 ELSE (b.P2_FM_S_W_W + b.P2_FM_S_L_W)/p.C_FM_W END END AS P2_FM_VOL_W_Surf_01,

      CASE WHEN (b.P2_FM_S_W_T + b.P2_FM_S_L_T)=0 THEN 1 ELSE 0 END AS P2_FM_WR_T_Surf_isMissing,
      CASE WHEN (b.P2_FM_S_W_Y + b.P2_FM_S_L_Y)=0 THEN 1 ELSE 0 END AS P2_FM_WR_Y_Surf_isMissing,
      CASE WHEN (b.P2_FM_S_W_M + b.P2_FM_S_L_M)=0 THEN 1 ELSE 0 END AS P2_FM_WR_M_Surf_isMissing,
      CASE WHEN (b.P2_FM_S_W_W + b.P2_FM_S_L_W)=0 THEN 1 ELSE 0 END AS P2_FM_WR_W_Surf_isMissing,

      /* ======== SETS & GAMES analogno (isti obrasci) ======== */
      -- SETS P1 agnostično
      CASE WHEN (b.P1_FS_W_T + b.P1_FS_L_T)=0 THEN 0.5 ELSE (b.P1_FS_W_T + p.A_FS_T)/(b.P1_FS_W_T + b.P1_FS_L_T + 2*p.A_FS_T) END AS P1_FS_WR_T_01,
      CASE WHEN (b.P1_FS_W_Y + b.P1_FS_L_Y)=0 THEN 0.5 ELSE (b.P1_FS_W_Y + p.A_FS_Y)/(b.P1_FS_W_Y + b.P1_FS_L_Y + 2*p.A_FS_Y) END AS P1_FS_WR_Y_01,
      CASE WHEN (b.P1_FS_W_M + b.P1_FS_L_M)=0 THEN 0.5 ELSE (b.P1_FS_W_M + p.A_FS_M)/(b.P1_FS_W_M + b.P1_FS_L_M + 2*p.A_FS_M) END AS P1_FS_WR_M_01,
      CASE WHEN (b.P1_FS_W_W + b.P1_FS_L_W)=0 THEN 0.5 ELSE (b.P1_FS_W_W + p.A_FS_W)/(b.P1_FS_W_W + b.P1_FS_L_W + 2*p.A_FS_W) END AS P1_FS_WR_W_01,

      CASE WHEN (b.P1_FS_W_T + b.P1_FS_L_T)=0 THEN 0 ELSE CASE WHEN (b.P1_FS_W_T + b.P1_FS_L_T) >= p.C_FS_T THEN 1 ELSE (b.P1_FS_W_T + b.P1_FS_L_T)/p.C_FS_T END END AS P1_FS_VOL_T_01,
      CASE WHEN (b.P1_FS_W_Y + b.P1_FS_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P1_FS_W_Y + b.P1_FS_L_Y) >= p.C_FS_Y THEN 1 ELSE (b.P1_FS_W_Y + b.P1_FS_L_Y)/p.C_FS_Y END END AS P1_FS_VOL_Y_01,
      CASE WHEN (b.P1_FS_W_M + b.P1_FS_L_M)=0 THEN 0 ELSE CASE WHEN (b.P1_FS_W_M + b.P1_FS_L_M) >= p.C_FS_M THEN 1 ELSE (b.P1_FS_W_M + b.P1_FS_L_M)/p.C_FS_M END END AS P1_FS_VOL_M_01,
      CASE WHEN (b.P1_FS_W_W + b.P1_FS_L_W)=0 THEN 0 ELSE CASE WHEN (b.P1_FS_W_W + b.P1_FS_L_W) >= p.C_FS_W THEN 1 ELSE (b.P1_FS_W_W + b.P1_FS_L_W)/p.C_FS_W END END AS P1_FS_VOL_W_01,

      -- SETS P2 agnostično
      CASE WHEN (b.P2_FS_W_T + b.P2_FS_L_T)=0 THEN 0.5 ELSE (b.P2_FS_W_T + p.A_FS_T)/(b.P2_FS_W_T + b.P2_FS_L_T + 2*p.A_FS_T) END AS P2_FS_WR_T_01,
      CASE WHEN (b.P2_FS_W_Y + b.P2_FS_L_Y)=0 THEN 0.5 ELSE (b.P2_FS_W_Y + p.A_FS_Y)/(b.P2_FS_W_Y + b.P2_FS_L_Y + 2*p.A_FS_Y) END AS P2_FS_WR_Y_01,
      CASE WHEN (b.P2_FS_W_M + b.P2_FS_L_M)=0 THEN 0.5 ELSE (b.P2_FS_W_M + p.A_FS_M)/(b.P2_FS_W_M + b.P2_FS_L_M + 2*p.A_FS_M) END AS P2_FS_WR_M_01,
      CASE WHEN (b.P2_FS_W_W + b.P2_FS_L_W)=0 THEN 0.5 ELSE (b.P2_FS_W_W + p.A_FS_W)/(b.P2_FS_W_W + b.P2_FS_L_W + 2*p.A_FS_W) END AS P2_FS_WR_W_01,

      CASE WHEN (b.P2_FS_W_T + b.P2_FS_L_T)=0 THEN 0 ELSE CASE WHEN (b.P2_FS_W_T + b.P2_FS_L_T) >= p.C_FS_T THEN 1 ELSE (b.P2_FS_W_T + b.P2_FS_L_T)/p.C_FS_T END END AS P2_FS_VOL_T_01,
      CASE WHEN (b.P2_FS_W_Y + b.P2_FS_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P2_FS_W_Y + b.P2_FS_L_Y) >= p.C_FS_Y THEN 1 ELSE (b.P2_FS_W_Y + b.P2_FS_L_Y)/p.C_FS_Y END END AS P2_FS_VOL_Y_01,
      CASE WHEN (b.P2_FS_W_M + b.P2_FS_L_M)=0 THEN 0 ELSE CASE WHEN (b.P2_FS_W_M + b.P2_FS_L_M) >= p.C_FS_M THEN 1 ELSE (b.P2_FS_W_M + b.P2_FS_L_M)/p.C_FS_M END END AS P2_FS_VOL_M_01,
      CASE WHEN (b.P2_FS_W_W + b.P2_FS_L_W)=0 THEN 0 ELSE CASE WHEN (b.P2_FS_W_W + b.P2_FS_L_W) >= p.C_FS_W THEN 1 ELSE (b.P2_FS_W_W + b.P2_FS_L_W)/p.C_FS_W END END AS P2_FS_VOL_W_01,

      -- SETS surface P1
      CASE WHEN (b.P1_FS_S_W_T + b.P1_FS_S_L_T)=0 THEN 0.5 ELSE (b.P1_FS_S_W_T + p.A_FS_T)/(b.P1_FS_S_W_T + b.P1_FS_S_L_T + 2*p.A_FS_T) END AS P1_FS_WR_T_Surf_01,
      CASE WHEN (b.P1_FS_S_W_Y + b.P1_FS_S_L_Y)=0 THEN 0.5 ELSE (b.P1_FS_S_W_Y + p.A_FS_Y)/(b.P1_FS_S_W_Y + b.P1_FS_S_L_Y + 2*p.A_FS_Y) END AS P1_FS_WR_Y_Surf_01,
      CASE WHEN (b.P1_FS_S_W_M + b.P1_FS_S_L_M)=0 THEN 0.5 ELSE (b.P1_FS_S_W_M + p.A_FS_M)/(b.P1_FS_S_W_M + b.P1_FS_S_L_M + 2*p.A_FS_M) END AS P1_FS_WR_M_Surf_01,
      CASE WHEN (b.P1_FS_S_W_W + b.P1_FS_S_L_W)=0 THEN 0.5 ELSE (b.P1_FS_S_W_W + p.A_FS_W)/(b.P1_FS_S_W_W + b.P1_FS_S_L_W + 2*p.A_FS_W) END AS P1_FS_WR_W_Surf_01,

      CASE WHEN (b.P1_FS_S_W_T + b.P1_FS_S_L_T)=0 THEN 0 ELSE CASE WHEN (b.P1_FS_S_W_T + b.P1_FS_S_L_T) >= p.C_FS_T THEN 1 ELSE (b.P1_FS_S_W_T + b.P1_FS_S_L_T)/p.C_FS_T END END AS P1_FS_VOL_T_Surf_01,
      CASE WHEN (b.P1_FS_S_W_Y + b.P1_FS_S_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P1_FS_S_W_Y + b.P1_FS_S_L_Y) >= p.C_FS_Y THEN 1 ELSE (b.P1_FS_S_W_Y + b.P1_FS_S_L_Y)/p.C_FS_Y END END AS P1_FS_VOL_Y_Surf_01,
      CASE WHEN (b.P1_FS_S_W_M + b.P1_FS_S_L_M)=0 THEN 0 ELSE CASE WHEN (b.P1_FS_S_W_M + b.P1_FS_S_L_M) >= p.C_FS_M THEN 1 ELSE (b.P1_FS_S_W_M + b.P1_FS_S_L_M)/p.C_FS_M END END AS P1_FS_VOL_M_Surf_01,
      CASE WHEN (b.P1_FS_S_W_W + b.P1_FS_S_L_W)=0 THEN 0 ELSE CASE WHEN (b.P1_FS_S_W_W + b.P1_FS_S_L_W) >= p.C_FS_W THEN 1 ELSE (b.P1_FS_S_W_W + b.P1_FS_S_L_W)/p.C_FS_W END END AS P1_FS_VOL_W_Surf_01,

      -- SETS surface P2
      CASE WHEN (b.P2_FS_S_W_T + b.P2_FS_S_L_T)=0 THEN 0.5 ELSE (b.P2_FS_S_W_T + p.A_FS_T)/(b.P2_FS_S_W_T + b.P2_FS_S_L_T + 2*p.A_FS_T) END AS P2_FS_WR_T_Surf_01,
      CASE WHEN (b.P2_FS_S_W_Y + b.P2_FS_S_L_Y)=0 THEN 0.5 ELSE (b.P2_FS_S_W_Y + p.A_FS_Y)/(b.P2_FS_S_W_Y + b.P2_FS_S_L_Y + 2*p.A_FS_Y) END AS P2_FS_WR_Y_Surf_01,
      CASE WHEN (b.P2_FS_S_W_M + b.P2_FS_S_L_M)=0 THEN 0.5 ELSE (b.P2_FS_S_W_M + p.A_FS_M)/(b.P2_FS_S_W_M + b.P2_FS_S_L_M + 2*p.A_FS_M) END AS P2_FS_WR_M_Surf_01,
      CASE WHEN (b.P2_FS_S_W_W + b.P2_FS_S_L_W)=0 THEN 0.5 ELSE (b.P2_FS_S_W_W + p.A_FS_W)/(b.P2_FS_S_W_W + b.P2_FS_S_L_W + 2*p.A_FS_W) END AS P2_FS_WR_W_Surf_01,

      CASE WHEN (b.P2_FS_S_W_T + b.P2_FS_S_L_T)=0 THEN 0 ELSE CASE WHEN (b.P2_FS_S_W_T + b.P2_FS_S_L_T) >= p.C_FS_T THEN 1 ELSE (b.P2_FS_S_W_T + b.P2_FS_S_L_T)/p.C_FS_T END END AS P2_FS_VOL_T_Surf_01,
      CASE WHEN (b.P2_FS_S_W_Y + b.P2_FS_S_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P2_FS_S_W_Y + b.P2_FS_S_L_Y) >= p.C_FS_Y THEN 1 ELSE (b.P2_FS_S_W_Y + b.P2_FS_S_L_Y)/p.C_FS_Y END END AS P2_FS_VOL_Y_Surf_01,
      CASE WHEN (b.P2_FS_S_W_M + b.P2_FS_S_L_M)=0 THEN 0 ELSE CASE WHEN (b.P2_FS_S_W_M + b.P2_FS_S_L_M) >= p.C_FS_M THEN 1 ELSE (b.P2_FS_S_W_M + b.P2_FS_S_L_M)/p.C_FS_M END END AS P2_FS_VOL_M_Surf_01,
      CASE WHEN (b.P2_FS_S_W_W + b.P2_FS_S_L_W)=0 THEN 0 ELSE CASE WHEN (b.P2_FS_S_W_W + b.P2_FS_S_L_W) >= p.C_FS_W THEN 1 ELSE (b.P2_FS_S_W_W + b.P2_FS_S_L_W)/p.C_FS_W END END AS P2_FS_VOL_W_Surf_01,

      -- GAMES P1 agnostično
      CASE WHEN (b.P1_FG_W_T + b.P1_FG_L_T)=0 THEN 0.5 ELSE (b.P1_FG_W_T + p.A_FG_T)/(b.P1_FG_W_T + b.P1_FG_L_T + 2*p.A_FG_T) END AS P1_FG_WR_T_01,
      CASE WHEN (b.P1_FG_W_Y + b.P1_FG_L_Y)=0 THEN 0.5 ELSE (b.P1_FG_W_Y + p.A_FG_Y)/(b.P1_FG_W_Y + b.P1_FG_L_Y + 2*p.A_FG_Y) END AS P1_FG_WR_Y_01,
      CASE WHEN (b.P1_FG_W_M + b.P1_FG_L_M)=0 THEN 0.5 ELSE (b.P1_FG_W_M + p.A_FG_M)/(b.P1_FG_W_M + b.P1_FG_L_M + 2*p.A_FG_M) END AS P1_FG_WR_M_01,
      CASE WHEN (b.P1_FG_W_W + b.P1_FG_L_W)=0 THEN 0.5 ELSE (b.P1_FG_W_W + p.A_FG_W)/(b.P1_FG_W_W + b.P1_FG_L_W + 2*p.A_FG_W) END AS P1_FG_WR_W_01,

      CASE WHEN (b.P1_FG_W_T + b.P1_FG_L_T)=0 THEN 0 ELSE CASE WHEN (b.P1_FG_W_T + b.P1_FG_L_T) >= p.C_FG_T THEN 1 ELSE (b.P1_FG_W_T + b.P1_FG_L_T)/p.C_FG_T END END AS P1_FG_VOL_T_01,
      CASE WHEN (b.P1_FG_W_Y + b.P1_FG_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P1_FG_W_Y + b.P1_FG_L_Y) >= p.C_FG_Y THEN 1 ELSE (b.P1_FG_W_Y + b.P1_FG_L_Y)/p.C_FG_Y END END AS P1_FG_VOL_Y_01,
      CASE WHEN (b.P1_FG_W_M + b.P1_FG_L_M)=0 THEN 0 ELSE CASE WHEN (b.P1_FG_W_M + b.P1_FG_L_M) >= p.C_FG_M THEN 1 ELSE (b.P1_FG_W_M + b.P1_FG_L_M)/p.C_FG_M END END AS P1_FG_VOL_M_01,
      CASE WHEN (b.P1_FG_W_W + b.P1_FG_L_W)=0 THEN 0 ELSE CASE WHEN (b.P1_FG_W_W + b.P1_FG_L_W) >= p.C_FG_W THEN 1 ELSE (b.P1_FG_W_W + b.P1_FG_L_W)/p.C_FG_W END END AS P1_FG_VOL_W_01,

      -- GAMES P2 agnostično
      CASE WHEN (b.P2_FG_W_T + b.P2_FG_L_T)=0 THEN 0.5 ELSE (b.P2_FG_W_T + p.A_FG_T)/(b.P2_FG_W_T + b.P2_FG_L_T + 2*p.A_FG_T) END AS P2_FG_WR_T_01,
      CASE WHEN (b.P2_FG_W_Y + b.P2_FG_L_Y)=0 THEN 0.5 ELSE (b.P2_FG_W_Y + p.A_FG_Y)/(b.P2_FG_W_Y + b.P2_FG_L_Y + 2*p.A_FG_Y) END AS P2_FG_WR_Y_01,
      CASE WHEN (b.P2_FG_W_M + b.P2_FG_L_M)=0 THEN 0.5 ELSE (b.P2_FG_W_M + p.A_FG_M)/(b.P2_FG_W_M + b.P2_FG_L_M + 2*p.A_FG_M) END AS P2_FG_WR_M_01,
      CASE WHEN (b.P2_FG_W_W + b.P2_FG_L_W)=0 THEN 0.5 ELSE (b.P2_FG_W_W + p.A_FG_W)/(b.P2_FG_W_W + b.P2_FG_L_W + 2*p.A_FG_W) END AS P2_FG_WR_W_01,

      CASE WHEN (b.P2_FG_W_T + b.P2_FG_L_T)=0 THEN 0 ELSE CASE WHEN (b.P2_FG_W_T + b.P2_FG_L_T) >= p.C_FG_T THEN 1 ELSE (b.P2_FG_W_T + b.P2_FG_L_T)/p.C_FG_T END END AS P2_FG_VOL_T_01,
      CASE WHEN (b.P2_FG_W_Y + b.P2_FG_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P2_FG_W_Y + b.P2_FG_L_Y) >= p.C_FG_Y THEN 1 ELSE (b.P2_FG_W_Y + b.P2_FG_L_Y)/p.C_FG_Y END END AS P2_FG_VOL_Y_01,
      CASE WHEN (b.P2_FG_W_M + b.P2_FG_L_M)=0 THEN 0 ELSE CASE WHEN (b.P2_FG_W_M + b.P2_FG_L_M) >= p.C_FG_M THEN 1 ELSE (b.P2_FG_W_M + b.P2_FG_L_M)/p.C_FG_M END END AS P2_FG_VOL_M_01,
      CASE WHEN (b.P2_FG_W_W + b.P2_FG_L_W)=0 THEN 0 ELSE CASE WHEN (b.P2_FG_W_W + b.P2_FG_L_W) >= p.C_FG_W THEN 1 ELSE (b.P2_FG_W_W + b.P2_FG_L_W)/p.C_FG_W END END AS P2_FG_VOL_W_01,

      -- GAMES surface P1
      CASE WHEN (b.P1_FG_S_W_T + b.P1_FG_S_L_T)=0 THEN 0.5 ELSE (b.P1_FG_S_W_T + p.A_FG_T)/(b.P1_FG_S_W_T + b.P1_FG_S_L_T + 2*p.A_FG_T) END AS P1_FG_WR_T_Surf_01,
      CASE WHEN (b.P1_FG_S_W_Y + b.P1_FG_S_L_Y)=0 THEN 0.5 ELSE (b.P1_FG_S_W_Y + p.A_FG_Y)/(b.P1_FG_S_W_Y + b.P1_FG_S_L_Y + 2*p.A_FG_Y) END AS P1_FG_WR_Y_Surf_01,
      CASE WHEN (b.P1_FG_S_W_M + b.P1_FG_S_L_M)=0 THEN 0.5 ELSE (b.P1_FG_S_W_M + p.A_FG_M)/(b.P1_FG_S_W_M + b.P1_FG_S_L_M + 2*p.A_FG_M) END AS P1_FG_WR_M_Surf_01,
      CASE WHEN (b.P1_FG_S_W_W + b.P1_FG_S_L_W)=0 THEN 0.5 ELSE (b.P1_FG_S_W_W + p.A_FG_W)/(b.P1_FG_S_W_W + b.P1_FG_S_L_W + 2*p.A_FG_W) END AS P1_FG_WR_W_Surf_01,

      CASE WHEN (b.P1_FG_S_W_T + b.P1_FG_S_L_T)=0 THEN 0 ELSE CASE WHEN (b.P1_FG_S_W_T + b.P1_FG_S_L_T) >= p.C_FG_T THEN 1 ELSE (b.P1_FG_S_W_T + b.P1_FG_S_L_T)/p.C_FG_T END END AS P1_FG_VOL_T_Surf_01,
      CASE WHEN (b.P1_FG_S_W_Y + b.P1_FG_S_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P1_FG_S_W_Y + b.P1_FG_S_L_Y) >= p.C_FG_Y THEN 1 ELSE (b.P1_FG_S_W_Y + b.P1_FG_S_L_Y)/p.C_FG_Y END END AS P1_FG_VOL_Y_Surf_01,
      CASE WHEN (b.P1_FG_S_W_M + b.P1_FG_S_L_M)=0 THEN 0 ELSE CASE WHEN (b.P1_FG_S_W_M + b.P1_FG_S_L_M) >= p.C_FG_M THEN 1 ELSE (b.P1_FG_S_W_M + b.P1_FG_S_L_M)/p.C_FG_M END END AS P1_FG_VOL_M_Surf_01,
      CASE WHEN (b.P1_FG_S_W_W + b.P1_FG_S_L_W)=0 THEN 0 ELSE CASE WHEN (b.P1_FG_S_W_W + b.P1_FG_S_L_W) >= p.C_FG_W THEN 1 ELSE (b.P1_FG_S_W_W + b.P1_FG_S_L_W)/p.C_FG_W END END AS P1_FG_VOL_W_Surf_01,

      -- GAMES surface P2
      CASE WHEN (b.P2_FG_S_W_T + b.P2_FG_S_L_T)=0 THEN 0.5 ELSE (b.P2_FG_S_W_T + p.A_FG_T)/(b.P2_FG_S_W_T + b.P2_FG_S_L_T + 2*p.A_FG_T) END AS P2_FG_WR_T_Surf_01,
      CASE WHEN (b.P2_FG_S_W_Y + b.P2_FG_S_L_Y)=0 THEN 0.5 ELSE (b.P2_FG_S_W_Y + p.A_FG_Y)/(b.P2_FG_S_W_Y + b.P2_FG_S_L_Y + 2*p.A_FG_Y) END AS P2_FG_WR_Y_Surf_01,
      CASE WHEN (b.P2_FG_S_W_M + b.P2_FG_S_L_M)=0 THEN 0.5 ELSE (b.P2_FG_S_W_M + p.A_FG_M)/(b.P2_FG_S_W_M + b.P2_FG_S_L_M + 2*p.A_FG_M) END AS P2_FG_WR_M_Surf_01,
      CASE WHEN (b.P2_FG_S_W_W + b.P2_FG_S_L_W)=0 THEN 0.5 ELSE (b.P2_FG_S_W_W + p.A_FG_W)/(b.P2_FG_S_W_W + b.P2_FG_S_L_W + 2*p.A_FG_W) END AS P2_FG_WR_W_Surf_01,

      CASE WHEN (b.P2_FG_S_W_T + b.P2_FG_S_L_T)=0 THEN 0 ELSE CASE WHEN (b.P2_FG_S_W_T + b.P2_FG_S_L_T) >= p.C_FG_T THEN 1 ELSE (b.P2_FG_S_W_T + b.P2_FG_S_L_T)/p.C_FG_T END END AS P2_FG_VOL_T_Surf_01,
      CASE WHEN (b.P2_FG_S_W_Y + b.P2_FG_S_L_Y)=0 THEN 0 ELSE CASE WHEN (b.P2_FG_S_W_Y + b.P2_FG_S_L_Y) >= p.C_FG_Y THEN 1 ELSE (b.P2_FG_S_W_Y + b.P2_FG_S_L_Y)/p.C_FG_Y END END AS P2_FG_VOL_Y_Surf_01,
      CASE WHEN (b.P2_FG_S_W_M + b.P2_FG_S_L_M)=0 THEN 0 ELSE CASE WHEN (b.P2_FG_S_W_M + b.P2_FG_S_L_M) >= p.C_FG_M THEN 1 ELSE (b.P2_FG_S_W_M + b.P2_FG_S_L_M)/p.C_FG_M END END AS P2_FG_VOL_M_Surf_01,
      CASE WHEN (b.P2_FG_S_W_W + b.P2_FG_S_L_W)=0 THEN 0 ELSE CASE WHEN (b.P2_FG_S_W_W + b.P2_FG_S_L_W) >= p.C_FG_W THEN 1 ELSE (b.P2_FG_S_W_W + b.P2_FG_S_L_W)/p.C_FG_W END END AS P2_FG_VOL_W_Surf_01
    FROM base b
    CROSS JOIN param p
),
/* =========================
   Meta: DELTA (surface - global) u [0,1] i DIFF (P1 vs P2) u [0,1]
   ========================= */
meta AS (
    SELECT
      c.*,

      /* MATCH delte */
      ((c.P1_FM_WR_T_Surf_01 - c.P1_FM_WR_T_01) + 1.0)/2.0 AS P1_FM_WR_T_Delta_01,
      ((c.P1_FM_WR_Y_Surf_01 - c.P1_FM_WR_Y_01) + 1.0)/2.0 AS P1_FM_WR_Y_Delta_01,
      ((c.P1_FM_WR_M_Surf_01 - c.P1_FM_WR_M_01) + 1.0)/2.0 AS P1_FM_WR_M_Delta_01,
      ((c.P1_FM_WR_W_Surf_01 - c.P1_FM_WR_W_01) + 1.0)/2.0 AS P1_FM_WR_W_Delta_01,

      ((c.P2_FM_WR_T_Surf_01 - c.P2_FM_WR_T_01) + 1.0)/2.0 AS P2_FM_WR_T_Delta_01,
      ((c.P2_FM_WR_Y_Surf_01 - c.P2_FM_WR_Y_01) + 1.0)/2.0 AS P2_FM_WR_Y_Delta_01,
      ((c.P2_FM_WR_M_Surf_01 - c.P2_FM_WR_M_01) + 1.0)/2.0 AS P2_FM_WR_M_Delta_01,
      ((c.P2_FM_WR_W_Surf_01 - c.P2_FM_WR_W_01) + 1.0)/2.0 AS P2_FM_WR_W_Delta_01,

      /* SETS delte */
      ((c.P1_FS_WR_T_Surf_01 - c.P1_FS_WR_T_01) + 1.0)/2.0 AS P1_FS_WR_T_Delta_01,
      ((c.P1_FS_WR_Y_Surf_01 - c.P1_FS_WR_Y_01) + 1.0)/2.0 AS P1_FS_WR_Y_Delta_01,
      ((c.P1_FS_WR_M_Surf_01 - c.P1_FS_WR_M_01) + 1.0)/2.0 AS P1_FS_WR_M_Delta_01,
      ((c.P1_FS_WR_W_Surf_01 - c.P1_FS_WR_W_01) + 1.0)/2.0 AS P1_FS_WR_W_Delta_01,

      ((c.P2_FS_WR_T_Surf_01 - c.P2_FS_WR_T_01) + 1.0)/2.0 AS P2_FS_WR_T_Delta_01,
      ((c.P2_FS_WR_Y_Surf_01 - c.P2_FS_WR_Y_01) + 1.0)/2.0 AS P2_FS_WR_Y_Delta_01,
      ((c.P2_FS_WR_M_Surf_01 - c.P2_FS_WR_M_01) + 1.0)/2.0 AS P2_FS_WR_M_Delta_01,
      ((c.P2_FS_WR_W_Surf_01 - c.P2_FS_WR_W_01) + 1.0)/2.0 AS P2_FS_WR_W_Delta_01,

      /* GAMES delte */
      ((c.P1_FG_WR_T_Surf_01 - c.P1_FG_WR_T_01) + 1.0)/2.0 AS P1_FG_WR_T_Delta_01,
      ((c.P1_FG_WR_Y_Surf_01 - c.P1_FG_WR_Y_01) + 1.0)/2.0 AS P1_FG_WR_Y_Delta_01,
      ((c.P1_FG_WR_M_Surf_01 - c.P1_FG_WR_M_01) + 1.0)/2.0 AS P1_FG_WR_M_Delta_01,
      ((c.P1_FG_WR_W_Surf_01 - c.P1_FG_WR_W_01) + 1.0)/2.0 AS P1_FG_WR_W_Delta_01,

      ((c.P2_FG_WR_T_Surf_01 - c.P2_FG_WR_T_01) + 1.0)/2.0 AS P2_FG_WR_T_Delta_01,
      ((c.P2_FG_WR_Y_Surf_01 - c.P2_FG_WR_Y_01) + 1.0)/2.0 AS P2_FG_WR_Y_Delta_01,
      ((c.P2_FG_WR_M_Surf_01 - c.P2_FG_WR_M_01) + 1.0)/2.0 AS P2_FG_WR_M_Delta_01,
      ((c.P2_FG_WR_W_Surf_01 - c.P2_FG_WR_W_01) + 1.0)/2.0 AS P2_FG_WR_W_Delta_01,

      /* DIFF (P1 vs P2) – samo za WR, i za surface WR */
      ((c.P1_FM_WR_T_01 - c.P2_FM_WR_T_01)+1.0)/2.0 AS FM_WR_T_Diff_01,
      ((c.P1_FM_WR_Y_01 - c.P2_FM_WR_Y_01)+1.0)/2.0 AS FM_WR_Y_Diff_01,
      ((c.P1_FM_WR_M_01 - c.P2_FM_WR_M_01)+1.0)/2.0 AS FM_WR_M_Diff_01,
      ((c.P1_FM_WR_W_01 - c.P2_FM_WR_W_01)+1.0)/2.0 AS FM_WR_W_Diff_01,

      ((c.P1_FM_WR_T_Surf_01 - c.P2_FM_WR_T_Surf_01)+1.0)/2.0 AS FM_WR_T_Surf_Diff_01,
      ((c.P1_FM_WR_Y_Surf_01 - c.P2_FM_WR_Y_Surf_01)+1.0)/2.0 AS FM_WR_Y_Surf_Diff_01,
      ((c.P1_FM_WR_M_Surf_01 - c.P2_FM_WR_M_Surf_01)+1.0)/2.0 AS FM_WR_M_Surf_Diff_01,
      ((c.P1_FM_WR_W_Surf_01 - c.P2_FM_WR_W_Surf_01)+1.0)/2.0 AS FM_WR_W_Surf_Diff_01,

      ((c.P1_FS_WR_T_01 - c.P2_FS_WR_T_01)+1.0)/2.0 AS FS_WR_T_Diff_01,
      ((c.P1_FS_WR_Y_01 - c.P2_FS_WR_Y_01)+1.0)/2.0 AS FS_WR_Y_Diff_01,
      ((c.P1_FS_WR_M_01 - c.P2_FS_WR_M_01)+1.0)/2.0 AS FS_WR_M_Diff_01,
      ((c.P1_FS_WR_W_01 - c.P2_FS_WR_W_01)+1.0)/2.0 AS FS_WR_W_Diff_01,

      ((c.P1_FS_WR_T_Surf_01 - c.P2_FS_WR_T_Surf_01)+1.0)/2.0 AS FS_WR_T_Surf_Diff_01,
      ((c.P1_FS_WR_Y_Surf_01 - c.P2_FS_WR_Y_Surf_01)+1.0)/2.0 AS FS_WR_Y_Surf_Diff_01,
      ((c.P1_FS_WR_M_Surf_01 - c.P2_FS_WR_M_Surf_01)+1.0)/2.0 AS FS_WR_M_Surf_Diff_01,
      ((c.P1_FS_WR_W_Surf_01 - c.P2_FS_WR_W_Surf_01)+1.0)/2.0 AS FS_WR_W_Surf_Diff_01,

      ((c.P1_FG_WR_T_01 - c.P2_FG_WR_T_01)+1.0)/2.0 AS FG_WR_T_Diff_01,
      ((c.P1_FG_WR_Y_01 - c.P2_FG_WR_Y_01)+1.0)/2.0 AS FG_WR_Y_Diff_01,
      ((c.P1_FG_WR_M_01 - c.P2_FG_WR_M_01)+1.0)/2.0 AS FG_WR_M_Diff_01,
      ((c.P1_FG_WR_W_01 - c.P2_FG_WR_W_01)+1.0)/2.0 AS FG_WR_W_Diff_01,

      ((c.P1_FG_WR_T_Surf_01 - c.P2_FG_WR_T_Surf_01)+1.0)/2.0 AS FG_WR_T_Surf_Diff_01,
      ((c.P1_FG_WR_Y_Surf_01 - c.P2_FG_WR_Y_Surf_01)+1.0)/2.0 AS FG_WR_Y_Surf_Diff_01,
      ((c.P1_FG_WR_M_Surf_01 - c.P2_FG_WR_M_Surf_01)+1.0)/2.0 AS FG_WR_M_Surf_Diff_01,
      ((c.P1_FG_WR_W_Surf_01 - c.P2_FG_WR_W_Surf_01)+1.0)/2.0 AS FG_WR_W_Surf_Diff_01
    FROM calc c
)
SELECT
  m.MatchTPId,

  /* ========= MATCH (agnostično) ========= */
  m.P1_FM_WR_T_01, m.P1_FM_WR_Y_01, m.P1_FM_WR_M_01, m.P1_FM_WR_W_01,
  m.P2_FM_WR_T_01, m.P2_FM_WR_Y_01, m.P2_FM_WR_M_01, m.P2_FM_WR_W_01,
  m.P1_FM_VOL_T_01, m.P1_FM_VOL_Y_01, m.P1_FM_VOL_M_01, m.P1_FM_VOL_W_01,
  m.P2_FM_VOL_T_01, m.P2_FM_VOL_Y_01, m.P2_FM_VOL_M_01, m.P2_FM_VOL_W_01,
  m.P1_FM_WR_T_isMissing, m.P1_FM_WR_Y_isMissing, m.P1_FM_WR_M_isMissing, m.P1_FM_WR_W_isMissing,
  m.P2_FM_WR_T_isMissing, m.P2_FM_WR_Y_isMissing, m.P2_FM_WR_M_isMissing, m.P2_FM_WR_W_isMissing,
  m.FM_WR_T_Diff_01, m.FM_WR_Y_Diff_01, m.FM_WR_M_Diff_01, m.FM_WR_W_Diff_01,

  /* ========= MATCH (surface) ========= */
  m.P1_FM_WR_T_Surf_01, m.P1_FM_WR_Y_Surf_01, m.P1_FM_WR_M_Surf_01, m.P1_FM_WR_W_Surf_01,
  m.P2_FM_WR_T_Surf_01, m.P2_FM_WR_Y_Surf_01, m.P2_FM_WR_M_Surf_01, m.P2_FM_WR_W_Surf_01,
  m.P1_FM_VOL_T_Surf_01, m.P1_FM_VOL_Y_Surf_01, m.P1_FM_VOL_M_Surf_01, m.P1_FM_VOL_W_Surf_01,
  m.P2_FM_VOL_T_Surf_01, m.P2_FM_VOL_Y_Surf_01, m.P2_FM_VOL_M_Surf_01, m.P2_FM_VOL_W_Surf_01,
  m.P1_FM_WR_T_Surf_isMissing, m.P1_FM_WR_Y_Surf_isMissing, m.P1_FM_WR_M_Surf_isMissing, m.P1_FM_WR_W_Surf_isMissing,
  m.P2_FM_WR_T_Surf_isMissing, m.P2_FM_WR_Y_Surf_isMissing, m.P2_FM_WR_M_Surf_isMissing, m.P2_FM_WR_W_Surf_isMissing,
  m.P1_FM_WR_T_Delta_01, m.P1_FM_WR_Y_Delta_01, m.P1_FM_WR_M_Delta_01, m.P1_FM_WR_W_Delta_01,
  m.P2_FM_WR_T_Delta_01, m.P2_FM_WR_Y_Delta_01, m.P2_FM_WR_M_Delta_01, m.P2_FM_WR_W_Delta_01,
  m.FM_WR_T_Surf_Diff_01, m.FM_WR_Y_Surf_Diff_01, m.FM_WR_M_Surf_Diff_01, m.FM_WR_W_Surf_Diff_01,

  /* ========= SETS ========= */
  m.P1_FS_WR_T_01, m.P1_FS_WR_Y_01, m.P1_FS_WR_M_01, m.P1_FS_WR_W_01,
  m.P2_FS_WR_T_01, m.P2_FS_WR_Y_01, m.P2_FS_WR_M_01, m.P2_FS_WR_W_01,
  m.P1_FS_VOL_T_01, m.P1_FS_VOL_Y_01, m.P1_FS_VOL_M_01, m.P1_FS_VOL_W_01,
  m.P2_FS_VOL_T_01, m.P2_FS_VOL_Y_01, m.P2_FS_VOL_M_01, m.P2_FS_VOL_W_01,
  m.P1_FS_WR_T_Surf_01, m.P1_FS_WR_Y_Surf_01, m.P1_FS_WR_M_Surf_01, m.P1_FS_WR_W_Surf_01,
  m.P2_FS_WR_T_Surf_01, m.P2_FS_WR_Y_Surf_01, m.P2_FS_WR_M_Surf_01, m.P2_FS_WR_W_Surf_01,
  m.P1_FS_VOL_T_Surf_01, m.P1_FS_VOL_Y_Surf_01, m.P1_FS_VOL_M_Surf_01, m.P1_FS_VOL_W_Surf_01,
  m.P2_FS_VOL_T_Surf_01, m.P2_FS_VOL_Y_Surf_01, m.P2_FS_VOL_M_Surf_01, m.P2_FS_VOL_W_Surf_01,
  m.P1_FS_WR_T_Delta_01, m.P1_FS_WR_Y_Delta_01, m.P1_FS_WR_M_Delta_01, m.P1_FS_WR_W_Delta_01,
  m.P2_FS_WR_T_Delta_01, m.P2_FS_WR_Y_Delta_01, m.P2_FS_WR_M_Delta_01, m.P2_FS_WR_W_Delta_01,
  m.FS_WR_T_Diff_01, m.FS_WR_Y_Diff_01, m.FS_WR_M_Diff_01, m.FS_WR_W_Diff_01,
  m.FS_WR_T_Surf_Diff_01, m.FS_WR_Y_Surf_Diff_01, m.FS_WR_M_Surf_Diff_01, m.FS_WR_W_Surf_Diff_01,

  /* ========= GAMES ========= */
  m.P1_FG_WR_T_01, m.P1_FG_WR_Y_01, m.P1_FG_WR_M_01, m.P1_FG_WR_W_01,
  m.P2_FG_WR_T_01, m.P2_FG_WR_Y_01, m.P2_FG_WR_M_01, m.P2_FG_WR_W_01,
  m.P1_FG_VOL_T_01, m.P1_FG_VOL_Y_01, m.P1_FG_VOL_M_01, m.P1_FG_VOL_W_01,
  m.P2_FG_VOL_T_01, m.P2_FG_VOL_Y_01, m.P2_FG_VOL_M_01, m.P2_FG_VOL_W_01,
  m.P1_FG_WR_T_Surf_01, m.P1_FG_WR_Y_Surf_01, m.P1_FG_WR_M_Surf_01, m.P1_FG_WR_W_Surf_01,
  m.P2_FG_WR_T_Surf_01, m.P2_FG_WR_Y_Surf_01, m.P2_FG_WR_M_Surf_01, m.P2_FG_WR_W_Surf_01,
  m.P1_FG_VOL_T_Surf_01, m.P1_FG_VOL_Y_Surf_01, m.P1_FG_VOL_M_Surf_01, m.P1_FG_VOL_W_Surf_01,
  m.P2_FG_VOL_T_Surf_01, m.P2_FG_VOL_Y_Surf_01, m.P2_FG_VOL_M_Surf_01, m.P2_FG_VOL_W_Surf_01,
  m.P1_FG_WR_T_Delta_01, m.P1_FG_WR_Y_Delta_01, m.P1_FG_WR_M_Delta_01, m.P1_FG_WR_W_Delta_01,
  m.P2_FG_WR_T_Delta_01, m.P2_FG_WR_Y_Delta_01, m.P2_FG_WR_M_Delta_01, m.P2_FG_WR_W_Delta_01,
  m.FG_WR_T_Diff_01, m.FG_WR_Y_Diff_01, m.FG_WR_M_Diff_01, m.FG_WR_W_Diff_01,
  m.FG_WR_T_Surf_Diff_01, m.FG_WR_Y_Surf_Diff_01, m.FG_WR_M_Surf_Diff_01, m.FG_WR_W_Surf_Diff_01

FROM meta m;
GO
