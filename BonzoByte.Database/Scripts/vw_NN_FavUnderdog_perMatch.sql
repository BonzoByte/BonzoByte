CREATE VIEW dbo.vw_NN_FavUnderdog_perMatch
AS
/* =========================
   Parametri (podesivo)
   ========================= */
WITH param AS (
    SELECT 
        -- Smoothing α (veći za kratke prozore)
        CAST(1.0 AS float) AS A_FAV_T, CAST(1.0 AS float) AS A_FAV_Y, CAST(2.0 AS float) AS A_FAV_M, CAST(2.0 AS float) AS A_FAV_W,
        CAST(1.0 AS float) AS A_UDG_T, CAST(1.0 AS float) AS A_UDG_Y, CAST(2.0 AS float) AS A_UDG_M, CAST(2.0 AS float) AS A_UDG_W,

        -- Volume cap (#mečeva u ulozi) – siguran default; kasnije “zamrznuti” iz TRAIN-a
        CAST(200.0 AS float) AS C_FAV_T, CAST(60.0 AS float) AS C_FAV_Y, CAST(20.0 AS float) AS C_FAV_M, CAST(5.0 AS float) AS C_FAV_W,
        CAST(200.0 AS float) AS C_UDG_T, CAST(60.0 AS float) AS C_UDG_Y, CAST(20.0 AS float) AS C_UDG_M, CAST(5.0 AS float) AS C_UDG_W
),
/* =========================
   Raw brojači i prosjeci pWin (cast u float)
   ========================= */
base AS (
    SELECT
      m.MatchTPId,

      /* ---------- P1 TOTAL ---------- */
      CAST(COALESCE(m.Player1TotalWinsAsFavourite,0)        AS float) AS P1_WF_T,
      CAST(COALESCE(m.Player1TotalLossesAsFavourite,0)      AS float) AS P1_LF_T,
      CAST(COALESCE(m.Player1TotalWinsAsUnderdog,0)         AS float) AS P1_WU_T,
      CAST(COALESCE(m.Player1TotalLossesAsUnderdog,0)       AS float) AS P1_LU_T,
      CAST(m.Player1AverageWinningProbabilityAtWonAsFavourite        AS float) AS P1_pW_F_T,
      CAST(m.Player1AverageWinningProbabilityAtLossAsFavourite       AS float) AS P1_pL_F_T,
      CAST(m.Player1AverageWinningProbabilityAtWonAsUnderdog         AS float) AS P1_pW_U_T,
      CAST(m.Player1AverageWinningProbabilityAtLossAsUnderdog        AS float) AS P1_pL_U_T,

      /* ---------- P1 YEAR ---------- */
      CAST(COALESCE(m.Player1TotalWinsAsFavouriteLastYear,0)   AS float) AS P1_WF_Y,
      CAST(COALESCE(m.Player1TotalLossesAsFavouriteLastYear,0) AS float) AS P1_LF_Y,
      CAST(COALESCE(m.Player1TotalWinsAsUnderdogLastYear,0)    AS float) AS P1_WU_Y,
      CAST(COALESCE(m.Player1TotalLossesAsUnderdogLastYear,0)  AS float) AS P1_LU_Y,
      CAST(m.Player1AverageWinningProbabilityAtWonAsFavouriteLastYear   AS float) AS P1_pW_F_Y,
      CAST(m.Player1AverageWinningProbabilityAtLossAsFavouriteLastYear  AS float) AS P1_pL_F_Y,
      CAST(m.Player1AverageWinningProbabilityAtWonAsUnderdogLastYear    AS float) AS P1_pW_U_Y,
      CAST(m.Player1AverageWinningProbabilityAtLossAsUnderdogLastYear   AS float) AS P1_pL_U_Y,

      /* ---------- P1 MONTH ---------- */
      CAST(COALESCE(m.Player1TotalWinsAsFavouriteLastMonth,0)   AS float) AS P1_WF_M,
      CAST(COALESCE(m.Player1TotalLossesAsFavouriteLastMonth,0) AS float) AS P1_LF_M,
      CAST(COALESCE(m.Player1TotalWinsAsUnderdogLastMonth,0)    AS float) AS P1_WU_M,
      CAST(COALESCE(m.Player1TotalLossesAsUnderdogLastMonth,0)  AS float) AS P1_LU_M,
      CAST(m.Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth   AS float) AS P1_pW_F_M,
      CAST(m.Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth  AS float) AS P1_pL_F_M,
      CAST(m.Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth    AS float) AS P1_pW_U_M,
      CAST(m.Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth   AS float) AS P1_pL_U_M,

      /* ---------- P1 WEEK ---------- */
      CAST(COALESCE(m.Player1TotalWinsAsFavouriteLastWeek,0)   AS float) AS P1_WF_W,
      CAST(COALESCE(m.Player1TotalLossesAsFavouriteLastWeek,0) AS float) AS P1_LF_W,
      CAST(COALESCE(m.Player1TotalWinsAsUnderdogLastWeek,0)    AS float) AS P1_WU_W,
      CAST(COALESCE(m.Player1TotalLossesAsUnderdogLastWeek,0)  AS float) AS P1_LU_W,
      CAST(m.Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek   AS float) AS P1_pW_F_W,
      CAST(m.Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek  AS float) AS P1_pL_F_W,
      CAST(m.Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek    AS float) AS P1_pW_U_W,
      CAST(m.Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek   AS float) AS P1_pL_U_W,

      /* ---------- P2 TOTAL ---------- */
      CAST(COALESCE(m.Player2TotalWinsAsFavourite,0)        AS float) AS P2_WF_T,
      CAST(COALESCE(m.Player2TotalLossesAsFavourite,0)      AS float) AS P2_LF_T,
      CAST(COALESCE(m.Player2TotalWinsAsUnderdog,0)         AS float) AS P2_WU_T,
      CAST(COALESCE(m.Player2TotalLossesAsUnderdog,0)       AS float) AS P2_LU_T,
      CAST(m.Player2AverageWinningProbabilityAtWonAsFavourite        AS float) AS P2_pW_F_T,
      CAST(m.Player2AverageWinningProbabilityAtLossAsFavourite       AS float) AS P2_pL_F_T,
      CAST(m.Player2AverageWinningProbabilityAtWonAsUnderdog         AS float) AS P2_pW_U_T,
      CAST(m.Player2AverageWinningProbabilityAtLossAsUnderdog        AS float) AS P2_pL_U_T,

      /* ---------- P2 YEAR ---------- */
      CAST(COALESCE(m.Player2TotalWinsAsFavouriteLastYear,0)   AS float) AS P2_WF_Y,
      CAST(COALESCE(m.Player2TotalLossesAsFavouriteLastYear,0) AS float) AS P2_LF_Y,
      CAST(COALESCE(m.Player2TotalWinsAsUnderdogLastYear,0)    AS float) AS P2_WU_Y,
      CAST(COALESCE(m.Player2TotalLossesAsUnderdogLastYear,0)  AS float) AS P2_LU_Y,
      CAST(m.Player2AverageWinningProbabilityAtWonAsFavouriteLastYear   AS float) AS P2_pW_F_Y,
      CAST(m.Player2AverageWinningProbabilityAtLossAsFavouriteLastYear  AS float) AS P2_pL_F_Y,
      CAST(m.Player2AverageWinningProbabilityAtWonAsUnderdogLastYear    AS float) AS P2_pW_U_Y,
      CAST(m.Player2AverageWinningProbabilityAtLossAsUnderdogLastYear   AS float) AS P2_pL_U_Y,

      /* ---------- P2 MONTH ---------- */
      CAST(COALESCE(m.Player2TotalWinsAsFavouriteLastMonth,0)   AS float) AS P2_WF_M,
      CAST(COALESCE(m.Player2TotalLossesAsFavouriteLastMonth,0) AS float) AS P2_LF_M,
      CAST(COALESCE(m.Player2TotalWinsAsUnderdogLastMonth,0)    AS float) AS P2_WU_M,
      CAST(COALESCE(m.Player2TotalLossesAsUnderdogLastMonth,0)  AS float) AS P2_LU_M,
      CAST(m.Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth   AS float) AS P2_pW_F_M,
      CAST(m.Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth  AS float) AS P2_pL_F_M,
      CAST(m.Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth    AS float) AS P2_pW_U_M,
      CAST(m.Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth   AS float) AS P2_pL_U_M,

      /* ---------- P2 WEEK ---------- */
      CAST(COALESCE(m.Player2TotalWinsAsFavouriteLastWeek,0)   AS float) AS P2_WF_W,
      CAST(COALESCE(m.Player2TotalLossesAsFavouriteLastWeek,0) AS float) AS P2_LF_W,
      CAST(COALESCE(m.Player2TotalWinsAsUnderdogLastWeek,0)    AS float) AS P2_WU_W,
      CAST(COALESCE(m.Player2TotalLossesAsUnderdogLastWeek,0)  AS float) AS P2_LU_W,
      CAST(m.Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek   AS float) AS P2_pW_F_W,
      CAST(m.Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek  AS float) AS P2_pL_F_W,
      CAST(m.Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek    AS float) AS P2_pW_U_W,
      CAST(m.Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek   AS float) AS P2_pL_U_W
    FROM dbo.[Match] m
),
/* =========================
   Izračun svih metrika po horizontu/ulozi (P1 i P2)
   ========================= */
calc AS (
    SELECT
      b.MatchTPId,

      /* ------- TOTAL (T) ------- */
      /* P1 Favorite */
      (b.P1_WF_T + b.P1_LF_T) AS P1_FAV_CNT_T,
      CASE WHEN (b.P1_WF_T + b.P1_LF_T)=0 THEN 0.5 ELSE (b.P1_WF_T + p.A_FAV_T)/(b.P1_WF_T + b.P1_LF_T + 2*p.A_FAV_T) END AS P1_FAV_WR_T_01,
      CASE WHEN (b.P1_WF_T + b.P1_LF_T)=0 THEN 0   ELSE CASE WHEN (b.P1_WF_T + b.P1_LF_T) >= p.C_FAV_T THEN 1 ELSE (b.P1_WF_T + b.P1_LF_T)/p.C_FAV_T END END AS P1_FAV_VOL_T_01,
      CASE WHEN (b.P1_WF_T + b.P1_LF_T)=0 THEN 0.5 ELSE (COALESCE(b.P1_pW_F_T,0.5)*b.P1_WF_T + COALESCE(b.P1_pL_F_T,0.5)*b.P1_LF_T) / NULLIF((b.P1_WF_T + b.P1_LF_T),0) END AS P1_FAV_pExp_T,
      CASE WHEN (b.P1_WF_T + b.P1_LF_T)=0 THEN 0.5 ELSE b.P1_WF_T / NULLIF((b.P1_WF_T + b.P1_LF_T),0) END AS P1_FAV_pReal_T,
      CASE WHEN b.P1_LF_T=0 THEN 0 ELSE COALESCE(b.P1_pL_F_T,0.5) END AS P1_FAV_LossShock_T_01,

      /* P1 Underdog */
      (b.P1_WU_T + b.P1_LU_T) AS P1_UDG_CNT_T,
      CASE WHEN (b.P1_WU_T + b.P1_LU_T)=0 THEN 0.5 ELSE (b.P1_WU_T + p.A_UDG_T)/(b.P1_WU_T + b.P1_LU_T + 2*p.A_UDG_T) END AS P1_UDG_WR_T_01,
      CASE WHEN (b.P1_WU_T + b.P1_LU_T)=0 THEN 0   ELSE CASE WHEN (b.P1_WU_T + b.P1_LU_T) >= p.C_UDG_T THEN 1 ELSE (b.P1_WU_T + b.P1_LU_T)/p.C_UDG_T END END AS P1_UDG_VOL_T_01,
      CASE WHEN (b.P1_WU_T + b.P1_LU_T)=0 THEN 0.5 ELSE (COALESCE(b.P1_pW_U_T,0.5)*b.P1_WU_T + COALESCE(b.P1_pL_U_T,0.5)*b.P1_LU_T) / NULLIF((b.P1_WU_T + b.P1_LU_T),0) END AS P1_UDG_pExp_T,
      CASE WHEN (b.P1_WU_T + b.P1_LU_T)=0 THEN 0.5 ELSE b.P1_WU_T / NULLIF((b.P1_WU_T + b.P1_LU_T),0) END AS P1_UDG_pReal_T,
      CASE WHEN b.P1_WU_T=0 THEN 0 ELSE 1 - COALESCE(b.P1_pW_U_T,0.5) END AS P1_UDG_WinShock_T_01,

      /* P2 Favorite */
      (b.P2_WF_T + b.P2_LF_T) AS P2_FAV_CNT_T,
      CASE WHEN (b.P2_WF_T + b.P2_LF_T)=0 THEN 0.5 ELSE (b.P2_WF_T + p.A_FAV_T)/(b.P2_WF_T + b.P2_LF_T + 2*p.A_FAV_T) END AS P2_FAV_WR_T_01,
      CASE WHEN (b.P2_WF_T + b.P2_LF_T)=0 THEN 0   ELSE CASE WHEN (b.P2_WF_T + b.P2_LF_T) >= p.C_FAV_T THEN 1 ELSE (b.P2_WF_T + b.P2_LF_T)/p.C_FAV_T END END AS P2_FAV_VOL_T_01,
      CASE WHEN (b.P2_WF_T + b.P2_LF_T)=0 THEN 0.5 ELSE (COALESCE(b.P2_pW_F_T,0.5)*b.P2_WF_T + COALESCE(b.P2_pL_F_T,0.5)*b.P2_LF_T) / NULLIF((b.P2_WF_T + b.P2_LF_T),0) END AS P2_FAV_pExp_T,
      CASE WHEN (b.P2_WF_T + b.P2_LF_T)=0 THEN 0.5 ELSE b.P2_WF_T / NULLIF((b.P2_WF_T + b.P2_LF_T),0) END AS P2_FAV_pReal_T,
      CASE WHEN b.P2_LF_T=0 THEN 0 ELSE COALESCE(b.P2_pL_F_T,0.5) END AS P2_FAV_LossShock_T_01,

      /* P2 Underdog */
      (b.P2_WU_T + b.P2_LU_T) AS P2_UDG_CNT_T,
      CASE WHEN (b.P2_WU_T + b.P2_LU_T)=0 THEN 0.5 ELSE (b.P2_WU_T + p.A_UDG_T)/(b.P2_WU_T + b.P2_LU_T + 2*p.A_UDG_T) END AS P2_UDG_WR_T_01,
      CASE WHEN (b.P2_WU_T + b.P2_LU_T)=0 THEN 0   ELSE CASE WHEN (b.P2_WU_T + b.P2_LU_T) >= p.C_UDG_T THEN 1 ELSE (b.P2_WU_T + b.P2_LU_T)/p.C_UDG_T END END AS P2_UDG_VOL_T_01,
      CASE WHEN (b.P2_WU_T + b.P2_LU_T)=0 THEN 0.5 ELSE (COALESCE(b.P2_pW_U_T,0.5)*b.P2_WU_T + COALESCE(b.P2_pL_U_T,0.5)*b.P2_LU_T) / NULLIF((b.P2_WU_T + b.P2_LU_T),0) END AS P2_UDG_pExp_T,
      CASE WHEN (b.P2_WU_T + b.P2_LU_T)=0 THEN 0.5 ELSE b.P2_WU_T / NULLIF((b.P2_WU_T + b.P2_LU_T),0) END AS P2_UDG_pReal_T,
      CASE WHEN b.P2_WU_T=0 THEN 0 ELSE 1 - COALESCE(b.P2_pW_U_T,0.5) END AS P2_UDG_WinShock_T_01,

      /* FavShare (profil) */
      CASE 
        WHEN (b.P1_WF_T + b.P1_LF_T + b.P1_WU_T + b.P1_LU_T)=0 THEN 0.5
        ELSE (b.P1_WF_T + b.P1_LF_T) / NULLIF((b.P1_WF_T + b.P1_LF_T + b.P1_WU_T + b.P1_LU_T),0)
      END AS P1_FavShare_T_01,
      CASE 
        WHEN (b.P2_WF_T + b.P2_LF_T + b.P2_WU_T + b.P2_LU_T)=0 THEN 0.5
        ELSE (b.P2_WF_T + b.P2_LF_T) / NULLIF((b.P2_WF_T + b.P2_LF_T + b.P2_WU_T + b.P2_LU_T),0)
      END AS P2_FavShare_T_01,

      /* ------- YEAR (Y) ------- */
      /* P1 */
      (b.P1_WF_Y + b.P1_LF_Y) AS P1_FAV_CNT_Y,
      CASE WHEN (b.P1_WF_Y + b.P1_LF_Y)=0 THEN 0.5 ELSE (b.P1_WF_Y + p.A_FAV_Y)/(b.P1_WF_Y + b.P1_LF_Y + 2*p.A_FAV_Y) END AS P1_FAV_WR_Y_01,
      CASE WHEN (b.P1_WF_Y + b.P1_LF_Y)=0 THEN 0   ELSE CASE WHEN (b.P1_WF_Y + b.P1_LF_Y) >= p.C_FAV_Y THEN 1 ELSE (b.P1_WF_Y + b.P1_LF_Y)/p.C_FAV_Y END END AS P1_FAV_VOL_Y_01,
      CASE WHEN (b.P1_WF_Y + b.P1_LF_Y)=0 THEN 0.5 ELSE (COALESCE(b.P1_pW_F_Y,0.5)*b.P1_WF_Y + COALESCE(b.P1_pL_F_Y,0.5)*b.P1_LF_Y) / NULLIF((b.P1_WF_Y + b.P1_LF_Y),0) END AS P1_FAV_pExp_Y,
      CASE WHEN (b.P1_WF_Y + b.P1_LF_Y)=0 THEN 0.5 ELSE b.P1_WF_Y / NULLIF((b.P1_WF_Y + b.P1_LF_Y),0) END AS P1_FAV_pReal_Y,
      CASE WHEN b.P1_LF_Y=0 THEN 0 ELSE COALESCE(b.P1_pL_F_Y,0.5) END AS P1_FAV_LossShock_Y_01,

      (b.P1_WU_Y + b.P1_LU_Y) AS P1_UDG_CNT_Y,
      CASE WHEN (b.P1_WU_Y + b.P1_LU_Y)=0 THEN 0.5 ELSE (b.P1_WU_Y + p.A_UDG_Y)/(b.P1_WU_Y + b.P1_LU_Y + 2*p.A_UDG_Y) END AS P1_UDG_WR_Y_01,
      CASE WHEN (b.P1_WU_Y + b.P1_LU_Y)=0 THEN 0   ELSE CASE WHEN (b.P1_WU_Y + b.P1_LU_Y) >= p.C_UDG_Y THEN 1 ELSE (b.P1_WU_Y + b.P1_LU_Y)/p.C_UDG_Y END END AS P1_UDG_VOL_Y_01,
      CASE WHEN (b.P1_WU_Y + b.P1_LU_Y)=0 THEN 0.5 ELSE (COALESCE(b.P1_pW_U_Y,0.5)*b.P1_WU_Y + COALESCE(b.P1_pL_U_Y,0.5)*b.P1_LU_Y) / NULLIF((b.P1_WU_Y + b.P1_LU_Y),0) END AS P1_UDG_pExp_Y,
      CASE WHEN (b.P1_WU_Y + b.P1_LU_Y)=0 THEN 0.5 ELSE b.P1_WU_Y / NULLIF((b.P1_WU_Y + b.P1_LU_Y),0) END AS P1_UDG_pReal_Y,
      CASE WHEN b.P1_WU_Y=0 THEN 0 ELSE 1 - COALESCE(b.P1_pW_U_Y,0.5) END AS P1_UDG_WinShock_Y_01,

      /* P2 */
      (b.P2_WF_Y + b.P2_LF_Y) AS P2_FAV_CNT_Y,
      CASE WHEN (b.P2_WF_Y + b.P2_LF_Y)=0 THEN 0.5 ELSE (b.P2_WF_Y + p.A_FAV_Y)/(b.P2_WF_Y + b.P2_LF_Y + 2*p.A_FAV_Y) END AS P2_FAV_WR_Y_01,
      CASE WHEN (b.P2_WF_Y + b.P2_LF_Y)=0 THEN 0   ELSE CASE WHEN (b.P2_WF_Y + b.P2_LF_Y) >= p.C_FAV_Y THEN 1 ELSE (b.P2_WF_Y + b.P2_LF_Y)/p.C_FAV_Y END END AS P2_FAV_VOL_Y_01,
      CASE WHEN (b.P2_WF_Y + b.P2_LF_Y)=0 THEN 0.5 ELSE (COALESCE(b.P2_pW_F_Y,0.5)*b.P2_WF_Y + COALESCE(b.P2_pL_F_Y,0.5)*b.P2_LF_Y) / NULLIF((b.P2_WF_Y + b.P2_LF_Y),0) END AS P2_FAV_pExp_Y,
      CASE WHEN (b.P2_WF_Y + b.P2_LF_Y)=0 THEN 0.5 ELSE b.P2_WF_Y / NULLIF((b.P2_WF_Y + b.P2_LF_Y),0) END AS P2_FAV_pReal_Y,
      CASE WHEN b.P2_LF_Y=0 THEN 0 ELSE COALESCE(b.P2_pL_F_Y,0.5) END AS P2_FAV_LossShock_Y_01,

      (b.P2_WU_Y + b.P2_LU_Y) AS P2_UDG_CNT_Y,
      CASE WHEN (b.P2_WU_Y + b.P2_LU_Y)=0 THEN 0.5 ELSE (b.P2_WU_Y + p.A_UDG_Y)/(b.P2_WU_Y + b.P2_LU_Y + 2*p.A_UDG_Y) END AS P2_UDG_WR_Y_01,
      CASE WHEN (b.P2_WU_Y + b.P2_LU_Y)=0 THEN 0   ELSE CASE WHEN (b.P2_WU_Y + b.P2_LU_Y) >= p.C_UDG_Y THEN 1 ELSE (b.P2_WU_Y + b.P2_LU_Y)/p.C_UDG_Y END END AS P2_UDG_VOL_Y_01,
      CASE WHEN (b.P2_WU_Y + b.P2_LU_Y)=0 THEN 0.5 ELSE (COALESCE(b.P2_pW_U_Y,0.5)*b.P2_WU_Y + COALESCE(b.P2_pL_U_Y,0.5)*b.P2_LU_Y) / NULLIF((b.P2_WU_Y + b.P2_LU_Y),0) END AS P2_UDG_pExp_Y,
      CASE WHEN (b.P2_WU_Y + b.P2_LU_Y)=0 THEN 0.5 ELSE b.P2_WU_Y / NULLIF((b.P2_WU_Y + b.P2_LU_Y),0) END AS P2_UDG_pReal_Y,
      CASE WHEN b.P2_WU_Y=0 THEN 0 ELSE 1 - COALESCE(b.P2_pW_U_Y,0.5) END AS P2_UDG_WinShock_Y_01,

      /* FavShare */
      CASE 
        WHEN (b.P1_WF_Y + b.P1_LF_Y + b.P1_WU_Y + b.P1_LU_Y)=0 THEN 0.5
        ELSE (b.P1_WF_Y + b.P1_LF_Y) / NULLIF((b.P1_WF_Y + b.P1_LF_Y + b.P1_WU_Y + b.P1_LU_Y),0)
      END AS P1_FavShare_Y_01,
      CASE 
        WHEN (b.P2_WF_Y + b.P2_LF_Y + b.P2_WU_Y + b.P2_LU_Y)=0 THEN 0.5
        ELSE (b.P2_WF_Y + b.P2_LF_Y) / NULLIF((b.P2_WF_Y + b.P2_LF_Y + b.P2_WU_Y + b.P2_LU_Y),0)
      END AS P2_FavShare_Y_01,

      /* ------- MONTH (M) ------- */
      /* P1 */
      (b.P1_WF_M + b.P1_LF_M) AS P1_FAV_CNT_M,
      CASE WHEN (b.P1_WF_M + b.P1_LF_M)=0 THEN 0.5 ELSE (b.P1_WF_M + p.A_FAV_M)/(b.P1_WF_M + b.P1_LF_M + 2*p.A_FAV_M) END AS P1_FAV_WR_M_01,
      CASE WHEN (b.P1_WF_M + b.P1_LF_M)=0 THEN 0   ELSE CASE WHEN (b.P1_WF_M + b.P1_LF_M) >= p.C_FAV_M THEN 1 ELSE (b.P1_WF_M + b.P1_LF_M)/p.C_FAV_M END END AS P1_FAV_VOL_M_01,
      CASE WHEN (b.P1_WF_M + b.P1_LF_M)=0 THEN 0.5 ELSE (COALESCE(b.P1_pW_F_M,0.5)*b.P1_WF_M + COALESCE(b.P1_pL_F_M,0.5)*b.P1_LF_M) / NULLIF((b.P1_WF_M + b.P1_LF_M),0) END AS P1_FAV_pExp_M,
      CASE WHEN (b.P1_WF_M + b.P1_LF_M)=0 THEN 0.5 ELSE b.P1_WF_M / NULLIF((b.P1_WF_M + b.P1_LF_M),0) END AS P1_FAV_pReal_M,
      CASE WHEN b.P1_LF_M=0 THEN 0 ELSE COALESCE(b.P1_pL_F_M,0.5) END AS P1_FAV_LossShock_M_01,

      (b.P1_WU_M + b.P1_LU_M) AS P1_UDG_CNT_M,
      CASE WHEN (b.P1_WU_M + b.P1_LU_M)=0 THEN 0.5 ELSE (b.P1_WU_M + p.A_UDG_M)/(b.P1_WU_M + b.P1_LU_M + 2*p.A_UDG_M) END AS P1_UDG_WR_M_01,
      CASE WHEN (b.P1_WU_M + b.P1_LU_M)=0 THEN 0   ELSE CASE WHEN (b.P1_WU_M + b.P1_LU_M) >= p.C_UDG_M THEN 1 ELSE (b.P1_WU_M + b.P1_LU_M)/p.C_UDG_M END END AS P1_UDG_VOL_M_01,
      CASE WHEN (b.P1_WU_M + b.P1_LU_M)=0 THEN 0.5 ELSE (COALESCE(b.P1_pW_U_M,0.5)*b.P1_WU_M + COALESCE(b.P1_pL_U_M,0.5)*b.P1_LU_M) / NULLIF((b.P1_WU_M + b.P1_LU_M),0) END AS P1_UDG_pExp_M,
      CASE WHEN (b.P1_WU_M + b.P1_LU_M)=0 THEN 0.5 ELSE b.P1_WU_M / NULLIF((b.P1_WU_M + b.P1_LU_M),0) END AS P1_UDG_pReal_M,
      CASE WHEN b.P1_WU_M=0 THEN 0 ELSE 1 - COALESCE(b.P1_pW_U_M,0.5) END AS P1_UDG_WinShock_M_01,

      /* P2 */
      (b.P2_WF_M + b.P2_LF_M) AS P2_FAV_CNT_M,
      CASE WHEN (b.P2_WF_M + b.P2_LF_M)=0 THEN 0.5 ELSE (b.P2_WF_M + p.A_FAV_M)/(b.P2_WF_M + b.P2_LF_M + 2*p.A_FAV_M) END AS P2_FAV_WR_M_01,
      CASE WHEN (b.P2_WF_M + b.P2_LF_M)=0 THEN 0   ELSE CASE WHEN (b.P2_WF_M + b.P2_LF_M) >= p.C_FAV_M THEN 1 ELSE (b.P2_WF_M + b.P2_LF_M)/p.C_FAV_M END END AS P2_FAV_VOL_M_01,
      CASE WHEN (b.P2_WF_M + b.P2_LF_M)=0 THEN 0.5 ELSE (COALESCE(b.P2_pW_F_M,0.5)*b.P2_WF_M + COALESCE(b.P2_pL_F_M,0.5)*b.P2_LF_M) / NULLIF((b.P2_WF_M + b.P2_LF_M),0) END AS P2_FAV_pExp_M,
      CASE WHEN (b.P2_WF_M + b.P2_LF_M)=0 THEN 0.5 ELSE b.P2_WF_M / NULLIF((b.P2_WF_M + b.P2_LF_M),0) END AS P2_FAV_pReal_M,
      CASE WHEN b.P2_LF_M=0 THEN 0 ELSE COALESCE(b.P2_pL_F_M,0.5) END AS P2_FAV_LossShock_M_01,

      (b.P2_WU_M + b.P2_LU_M) AS P2_UDG_CNT_M,
      CASE WHEN (b.P2_WU_M + b.P2_LU_M)=0 THEN 0.5 ELSE (b.P2_WU_M + p.A_UDG_M)/(b.P2_WU_M + b.P2_LU_M + 2*p.A_UDG_M) END AS P2_UDG_WR_M_01,
      CASE WHEN (b.P2_WU_M + b.P2_LU_M)=0 THEN 0   ELSE CASE WHEN (b.P2_WU_M + b.P2_LU_M) >= p.C_UDG_M THEN 1 ELSE (b.P2_WU_M + b.P2_LU_M)/p.C_UDG_M END END AS P2_UDG_VOL_M_01,
      CASE WHEN (b.P2_WU_M + b.P2_LU_M)=0 THEN 0.5 ELSE (COALESCE(b.P2_pW_U_M,0.5)*b.P2_WU_M + COALESCE(b.P2_pL_U_M,0.5)*b.P2_LU_M) / NULLIF((b.P2_WU_M + b.P2_LU_M),0) END AS P2_UDG_pExp_M,
      CASE WHEN (b.P2_WU_M + b.P2_LU_M)=0 THEN 0.5 ELSE b.P2_WU_M / NULLIF((b.P2_WU_M + b.P2_LU_M),0) END AS P2_UDG_pReal_M,
      CASE WHEN b.P2_WU_M=0 THEN 0 ELSE 1 - COALESCE(b.P2_pW_U_M,0.5) END AS P2_UDG_WinShock_M_01,

      /* FavShare */
      CASE 
        WHEN (b.P1_WF_M + b.P1_LF_M + b.P1_WU_M + b.P1_LU_M)=0 THEN 0.5
        ELSE (b.P1_WF_M + b.P1_LF_M) / NULLIF((b.P1_WF_M + b.P1_LF_M + b.P1_WU_M + b.P1_LU_M),0)
      END AS P1_FavShare_M_01,
      CASE 
        WHEN (b.P2_WF_M + b.P2_LF_M + b.P2_WU_M + b.P2_LU_M)=0 THEN 0.5
        ELSE (b.P2_WF_M + b.P2_LF_M) / NULLIF((b.P2_WF_M + b.P2_LF_M + b.P2_WU_M + b.P2_LU_M),0)
      END AS P2_FavShare_M_01,

      /* ------- WEEK (W) ------- */
      /* P1 */
      (b.P1_WF_W + b.P1_LF_W) AS P1_FAV_CNT_W,
      CASE WHEN (b.P1_WF_W + b.P1_LF_W)=0 THEN 0.5 ELSE (b.P1_WF_W + p.A_FAV_W)/(b.P1_WF_W + b.P1_LF_W + 2*p.A_FAV_W) END AS P1_FAV_WR_W_01,
      CASE WHEN (b.P1_WF_W + b.P1_LF_W)=0 THEN 0   ELSE CASE WHEN (b.P1_WF_W + b.P1_LF_W) >= p.C_FAV_W THEN 1 ELSE (b.P1_WF_W + b.P1_LF_W)/p.C_FAV_W END END AS P1_FAV_VOL_W_01,
      CASE WHEN (b.P1_WF_W + b.P1_LF_W)=0 THEN 0.5 ELSE (COALESCE(b.P1_pW_F_W,0.5)*b.P1_WF_W + COALESCE(b.P1_pL_F_W,0.5)*b.P1_LF_W) / NULLIF((b.P1_WF_W + b.P1_LF_W),0) END AS P1_FAV_pExp_W,
      CASE WHEN (b.P1_WF_W + b.P1_LF_W)=0 THEN 0.5 ELSE b.P1_WF_W / NULLIF((b.P1_WF_W + b.P1_LF_W),0) END AS P1_FAV_pReal_W,
      CASE WHEN b.P1_LF_W=0 THEN 0 ELSE COALESCE(b.P1_pL_F_W,0.5) END AS P1_FAV_LossShock_W_01,

      (b.P1_WU_W + b.P1_LU_W) AS P1_UDG_CNT_W,
      CASE WHEN (b.P1_WU_W + b.P1_LU_W)=0 THEN 0.5 ELSE (b.P1_WU_W + p.A_UDG_W)/(b.P1_WU_W + b.P1_LU_W + 2*p.A_UDG_W) END AS P1_UDG_WR_W_01,
      CASE WHEN (b.P1_WU_W + b.P1_LU_W)=0 THEN 0   ELSE CASE WHEN (b.P1_WU_W + b.P1_LU_W) >= p.C_UDG_W THEN 1 ELSE (b.P1_WU_W + b.P1_LU_W)/p.C_UDG_W END END AS P1_UDG_VOL_W_01,
      CASE WHEN (b.P1_WU_W + b.P1_LU_W)=0 THEN 0.5 ELSE (COALESCE(b.P1_pW_U_W,0.5)*b.P1_WU_W + COALESCE(b.P1_pL_U_W,0.5)*b.P1_LU_W) / NULLIF((b.P1_WU_W + b.P1_LU_W),0) END AS P1_UDG_pExp_W,
      CASE WHEN (b.P1_WU_W + b.P1_LU_W)=0 THEN 0.5 ELSE b.P1_WU_W / NULLIF((b.P1_WU_W + b.P1_LU_W),0) END AS P1_UDG_pReal_W,
      CASE WHEN b.P1_WU_W=0 THEN 0 ELSE 1 - COALESCE(b.P1_pW_U_W,0.5) END AS P1_UDG_WinShock_W_01,

      /* P2 */
      (b.P2_WF_W + b.P2_LF_W) AS P2_FAV_CNT_W,
      CASE WHEN (b.P2_WF_W + b.P2_LF_W)=0 THEN 0.5 ELSE (b.P2_WF_W + p.A_FAV_W)/(b.P2_WF_W + b.P2_LF_W + 2*p.A_FAV_W) END AS P2_FAV_WR_W_01,
      CASE WHEN (b.P2_WF_W + b.P2_LF_W)=0 THEN 0   ELSE CASE WHEN (b.P2_WF_W + b.P2_LF_W) >= p.C_FAV_W THEN 1 ELSE (b.P2_WF_W + b.P2_LF_W)/p.C_FAV_W END END AS P2_FAV_VOL_W_01,
      CASE WHEN (b.P2_WF_W + b.P2_LF_W)=0 THEN 0.5 ELSE (COALESCE(b.P2_pW_F_W,0.5)*b.P2_WF_W + COALESCE(b.P2_pL_F_W,0.5)*b.P2_LF_W) / NULLIF((b.P2_WF_W + b.P2_LF_W),0) END AS P2_FAV_pExp_W,
      CASE WHEN (b.P2_WF_W + b.P2_LF_W)=0 THEN 0.5 ELSE b.P2_WF_W / NULLIF((b.P2_WF_W + b.P2_LF_W),0) END AS P2_FAV_pReal_W,
      CASE WHEN b.P2_LF_W=0 THEN 0 ELSE COALESCE(b.P2_pL_F_W,0.5) END AS P2_FAV_LossShock_W_01,

      (b.P2_WU_W + b.P2_LU_W) AS P2_UDG_CNT_W,
      CASE WHEN (b.P2_WU_W + b.P2_LU_W)=0 THEN 0.5 ELSE (b.P2_WU_W + p.A_UDG_W)/(b.P2_WU_W + b.P2_LU_W + 2*p.A_UDG_W) END AS P2_UDG_WR_W_01,
      CASE WHEN (b.P2_WU_W + b.P2_LU_W)=0 THEN 0   ELSE CASE WHEN (b.P2_WU_W + b.P2_LU_W) >= p.C_UDG_W THEN 1 ELSE (b.P2_WU_W + b.P2_LU_W)/p.C_UDG_W END END AS P2_UDG_VOL_W_01,
      CASE WHEN (b.P2_WU_W + b.P2_LU_W)=0 THEN 0.5 ELSE (COALESCE(b.P2_pW_U_W,0.5)*b.P2_WU_W + COALESCE(b.P2_pL_U_W,0.5)*b.P2_LU_W) / NULLIF((b.P2_WU_W + b.P2_LU_W),0) END AS P2_UDG_pExp_W,
      CASE WHEN (b.P2_WU_W + b.P2_LU_W)=0 THEN 0.5 ELSE b.P2_WU_W / NULLIF((b.P2_WU_W + b.P2_LU_W),0) END AS P2_UDG_pReal_W,
      CASE WHEN b.P2_WU_W=0 THEN 0 ELSE 1 - COALESCE(b.P2_pW_U_W,0.5) END AS P2_UDG_WinShock_W_01,

      /* FavShare */
      CASE 
        WHEN (b.P1_WF_W + b.P1_LF_W + b.P1_WU_W + b.P1_LU_W)=0 THEN 0.5
        ELSE (b.P1_WF_W + b.P1_LF_W) / NULLIF((b.P1_WF_W + b.P1_LF_W + b.P1_WU_W + b.P1_LU_W),0)
      END AS P1_FavShare_W_01,
      CASE 
        WHEN (b.P2_WF_W + b.P2_LF_W + b.P2_WU_W + b.P2_LU_W)=0 THEN 0.5
        ELSE (b.P2_WF_W + b.P2_LF_W) / NULLIF((b.P2_WF_W + b.P2_LF_W + b.P2_WU_W + b.P2_LU_W),0)
      END AS P2_FavShare_W_01

    FROM base b
    CROSS JOIN param p
),
/* =========================
   Kalibracijski GAPS/ABS, diffovi i flagovi
   ========================= */
meta AS (
    SELECT
      c.*,

      /* TOTAL (T) */
      ((c.P1_FAV_pReal_T - c.P1_FAV_pExp_T) + 1.0)/2.0 AS P1_FAV_CalibGap_T_01,
      ABS(c.P1_FAV_pReal_T - c.P1_FAV_pExp_T)           AS P1_FAV_CalibAbs_T_01,
      ((c.P1_UDG_pReal_T - c.P1_UDG_pExp_T) + 1.0)/2.0 AS P1_UDG_CalibGap_T_01,
      ABS(c.P1_UDG_pReal_T - c.P1_UDG_pExp_T)           AS P1_UDG_CalibAbs_T_01,

      ((c.P2_FAV_pReal_T - c.P2_FAV_pExp_T) + 1.0)/2.0 AS P2_FAV_CalibGap_T_01,
      ABS(c.P2_FAV_pReal_T - c.P2_FAV_pExp_T)           AS P2_FAV_CalibAbs_T_01,
      ((c.P2_UDG_pReal_T - c.P2_UDG_pExp_T) + 1.0)/2.0 AS P2_UDG_CalibGap_T_01,
      ABS(c.P2_UDG_pReal_T - c.P2_UDG_pExp_T)           AS P2_UDG_CalibAbs_T_01,

      /* YEAR (Y) */
      ((c.P1_FAV_pReal_Y - c.P1_FAV_pExp_Y) + 1.0)/2.0 AS P1_FAV_CalibGap_Y_01,
      ABS(c.P1_FAV_pReal_Y - c.P1_FAV_pExp_Y)           AS P1_FAV_CalibAbs_Y_01,
      ((c.P1_UDG_pReal_Y - c.P1_UDG_pExp_Y) + 1.0)/2.0 AS P1_UDG_CalibGap_Y_01,
      ABS(c.P1_UDG_pReal_Y - c.P1_UDG_pExp_Y)           AS P1_UDG_CalibAbs_Y_01,

      ((c.P2_FAV_pReal_Y - c.P2_FAV_pExp_Y) + 1.0)/2.0 AS P2_FAV_CalibGap_Y_01,
      ABS(c.P2_FAV_pReal_Y - c.P2_FAV_pExp_Y)           AS P2_FAV_CalibAbs_Y_01,
      ((c.P2_UDG_pReal_Y - c.P2_UDG_pExp_Y) + 1.0)/2.0 AS P2_UDG_CalibGap_Y_01,
      ABS(c.P2_UDG_pReal_Y - c.P2_UDG_pExp_Y)           AS P2_UDG_CalibAbs_Y_01,

      /* MONTH (M) */
      ((c.P1_FAV_pReal_M - c.P1_FAV_pExp_M) + 1.0)/2.0 AS P1_FAV_CalibGap_M_01,
      ABS(c.P1_FAV_pReal_M - c.P1_FAV_pExp_M)           AS P1_FAV_CalibAbs_M_01,
      ((c.P1_UDG_pReal_M - c.P1_UDG_pExp_M) + 1.0)/2.0 AS P1_UDG_CalibGap_M_01,
      ABS(c.P1_UDG_pReal_M - c.P1_UDG_pExp_M)           AS P1_UDG_CalibAbs_M_01,

      ((c.P2_FAV_pReal_M - c.P2_FAV_pExp_M) + 1.0)/2.0 AS P2_FAV_CalibGap_M_01,
      ABS(c.P2_FAV_pReal_M - c.P2_FAV_pExp_M)           AS P2_FAV_CalibAbs_M_01,
      ((c.P2_UDG_pReal_M - c.P2_UDG_pExp_M) + 1.0)/2.0 AS P2_UDG_CalibGap_M_01,
      ABS(c.P2_UDG_pReal_M - c.P2_UDG_pExp_M)           AS P2_UDG_CalibAbs_M_01,

      /* WEEK (W) */
      ((c.P1_FAV_pReal_W - c.P1_FAV_pExp_W) + 1.0)/2.0 AS P1_FAV_CalibGap_W_01,
      ABS(c.P1_FAV_pReal_W - c.P1_FAV_pExp_W)           AS P1_FAV_CalibAbs_W_01,
      ((c.P1_UDG_pReal_W - c.P1_UDG_pExp_W) + 1.0)/2.0 AS P1_UDG_CalibGap_W_01,
      ABS(c.P1_UDG_pReal_W - c.P1_UDG_pExp_W)           AS P1_UDG_CalibAbs_W_01,

      ((c.P2_FAV_pReal_W - c.P2_FAV_pExp_W) + 1.0)/2.0 AS P2_FAV_CalibGap_W_01,
      ABS(c.P2_FAV_pReal_W - c.P2_FAV_pExp_W)           AS P2_FAV_CalibAbs_W_01,
      ((c.P2_UDG_pReal_W - c.P2_UDG_pExp_W) + 1.0)/2.0 AS P2_UDG_CalibGap_W_01,
      ABS(c.P2_UDG_pReal_W - c.P2_UDG_pExp_W)           AS P2_UDG_CalibAbs_W_01
    FROM calc c
),
flags AS (
    SELECT
      c.MatchTPId,

      /* Missing (nema mečeva u ulozi) */
      CASE WHEN c.P1_FAV_CNT_T=0 THEN 1 ELSE 0 END AS P1_FAV_T_isMissing,
      CASE WHEN c.P1_UDG_CNT_T=0 THEN 1 ELSE 0 END AS P1_UDG_T_isMissing,
      CASE WHEN c.P2_FAV_CNT_T=0 THEN 1 ELSE 0 END AS P2_FAV_T_isMissing,
      CASE WHEN c.P2_UDG_CNT_T=0 THEN 1 ELSE 0 END AS P2_UDG_T_isMissing,

      CASE WHEN c.P1_FAV_CNT_Y=0 THEN 1 ELSE 0 END AS P1_FAV_Y_isMissing,
      CASE WHEN c.P1_UDG_CNT_Y=0 THEN 1 ELSE 0 END AS P1_UDG_Y_isMissing,
      CASE WHEN c.P2_FAV_CNT_Y=0 THEN 1 ELSE 0 END AS P2_FAV_Y_isMissing,
      CASE WHEN c.P2_UDG_CNT_Y=0 THEN 1 ELSE 0 END AS P2_UDG_Y_isMissing,

      CASE WHEN c.P1_FAV_CNT_M=0 THEN 1 ELSE 0 END AS P1_FAV_M_isMissing,
      CASE WHEN c.P1_UDG_CNT_M=0 THEN 1 ELSE 0 END AS P1_UDG_M_isMissing,
      CASE WHEN c.P2_FAV_CNT_M=0 THEN 1 ELSE 0 END AS P2_FAV_M_isMissing,
      CASE WHEN c.P2_UDG_CNT_M=0 THEN 1 ELSE 0 END AS P2_UDG_M_isMissing,

      CASE WHEN c.P1_FAV_CNT_W=0 THEN 1 ELSE 0 END AS P1_FAV_W_isMissing,
      CASE WHEN c.P1_UDG_CNT_W=0 THEN 1 ELSE 0 END AS P1_UDG_W_isMissing,
      CASE WHEN c.P2_FAV_CNT_W=0 THEN 1 ELSE 0 END AS P2_FAV_W_isMissing,
      CASE WHEN c.P2_UDG_CNT_W=0 THEN 1 ELSE 0 END AS P2_UDG_W_isMissing,

      /* Shock-specific missing (nema odgovarajućih ishoda) */
      CASE WHEN c.P1_FAV_CNT_T=0 OR c.P1_FAV_LossShock_T_01=0 THEN 1 ELSE 0 END AS P1_FAV_LossShock_T_isMissing,
      CASE WHEN c.P2_FAV_CNT_T=0 OR c.P2_FAV_LossShock_T_01=0 THEN 1 ELSE 0 END AS P2_FAV_LossShock_T_isMissing,
      CASE WHEN c.P1_UDG_CNT_T=0 OR c.P1_UDG_WinShock_T_01=0 THEN 1 ELSE 0 END AS P1_UDG_WinShock_T_isMissing,
      CASE WHEN c.P2_UDG_CNT_T=0 OR c.P2_UDG_WinShock_T_01=0 THEN 1 ELSE 0 END AS P2_UDG_WinShock_T_isMissing,

      CASE WHEN c.P1_FAV_CNT_Y=0 OR c.P1_FAV_LossShock_Y_01=0 THEN 1 ELSE 0 END AS P1_FAV_LossShock_Y_isMissing,
      CASE WHEN c.P2_FAV_CNT_Y=0 OR c.P2_FAV_LossShock_Y_01=0 THEN 1 ELSE 0 END AS P2_FAV_LossShock_Y_isMissing,
      CASE WHEN c.P1_UDG_CNT_Y=0 OR c.P1_UDG_WinShock_Y_01=0 THEN 1 ELSE 0 END AS P1_UDG_WinShock_Y_isMissing,
      CASE WHEN c.P2_UDG_CNT_Y=0 OR c.P2_UDG_WinShock_Y_01=0 THEN 1 ELSE 0 END AS P2_UDG_WinShock_Y_isMissing,

      CASE WHEN c.P1_FAV_CNT_M=0 OR c.P1_FAV_LossShock_M_01=0 THEN 1 ELSE 0 END AS P1_FAV_LossShock_M_isMissing,
      CASE WHEN c.P2_FAV_CNT_M=0 OR c.P2_FAV_LossShock_M_01=0 THEN 1 ELSE 0 END AS P2_FAV_LossShock_M_isMissing,
      CASE WHEN c.P1_UDG_CNT_M=0 OR c.P1_UDG_WinShock_M_01=0 THEN 1 ELSE 0 END AS P1_UDG_WinShock_M_isMissing,
      CASE WHEN c.P2_UDG_CNT_M=0 OR c.P2_UDG_WinShock_M_01=0 THEN 1 ELSE 0 END AS P2_UDG_WinShock_M_isMissing,

      CASE WHEN c.P1_FAV_CNT_W=0 OR c.P1_FAV_LossShock_W_01=0 THEN 1 ELSE 0 END AS P1_FAV_LossShock_W_isMissing,
      CASE WHEN c.P2_FAV_CNT_W=0 OR c.P2_FAV_LossShock_W_01=0 THEN 1 ELSE 0 END AS P2_FAV_LossShock_W_isMissing,
      CASE WHEN c.P1_UDG_CNT_W=0 OR c.P1_UDG_WinShock_W_01=0 THEN 1 ELSE 0 END AS P1_UDG_WinShock_W_isMissing,
      CASE WHEN c.P2_UDG_CNT_W=0 OR c.P2_UDG_WinShock_W_01=0 THEN 1 ELSE 0 END AS P2_UDG_WinShock_W_isMissing
    FROM calc c
)
SELECT
  m.MatchTPId,

  /* ---------- TOTAL (T) ---------- */
  -- P1/P2 WR & VOL
  m.P1_FAV_WR_T_01, m.P1_UDG_WR_T_01, m.P1_FAV_VOL_T_01, m.P1_UDG_VOL_T_01,
  m.P2_FAV_WR_T_01, m.P2_UDG_WR_T_01, m.P2_FAV_VOL_T_01, m.P2_UDG_VOL_T_01,

  -- Kalibracija
  m.P1_FAV_CalibGap_T_01, m.P1_FAV_CalibAbs_T_01, m.P1_UDG_CalibGap_T_01, m.P1_UDG_CalibAbs_T_01,
  m.P2_FAV_CalibGap_T_01, m.P2_FAV_CalibAbs_T_01, m.P2_UDG_CalibGap_T_01, m.P2_UDG_CalibAbs_T_01,

  -- Shock & profil
  m.P1_FAV_LossShock_T_01, m.P1_UDG_WinShock_T_01, m.P1_FavShare_T_01,
  m.P2_FAV_LossShock_T_01, m.P2_UDG_WinShock_T_01, m.P2_FavShare_T_01,

  -- Flags
  f.P1_FAV_T_isMissing, f.P1_UDG_T_isMissing, f.P2_FAV_T_isMissing, f.P2_UDG_T_isMissing,
  f.P1_FAV_LossShock_T_isMissing, f.P1_UDG_WinShock_T_isMissing, f.P2_FAV_LossShock_T_isMissing, f.P2_UDG_WinShock_T_isMissing,

  -- Diff (simetrično, 0–1)
  ((m.P1_FAV_WR_T_01 - m.P2_FAV_WR_T_01)+1.0)/2.0 AS FAV_WR_T_Diff_01,
  ((m.P1_UDG_WR_T_01 - m.P2_UDG_WR_T_01)+1.0)/2.0 AS UDG_WR_T_Diff_01,
  ((m.P1_FAV_CalibGap_T_01 - m.P2_FAV_CalibGap_T_01)+1.0)/2.0 AS FAV_CalibGap_T_Diff_01,
  ((m.P1_UDG_CalibGap_T_01 - m.P2_UDG_CalibGap_T_01)+1.0)/2.0 AS UDG_CalibGap_T_Diff_01,
  ((m.P1_FAV_LossShock_T_01 - m.P2_FAV_LossShock_T_01)+1.0)/2.0 AS FAV_LossShock_T_Diff_01,
  ((m.P1_UDG_WinShock_T_01  - m.P2_UDG_WinShock_T_01)+1.0)/2.0  AS UDG_WinShock_T_Diff_01,
  ((m.P1_FavShare_T_01      - m.P2_FavShare_T_01)+1.0)/2.0      AS FavShare_T_Diff_01,

  /* ---------- YEAR (Y) ---------- */
  m.P1_FAV_WR_Y_01, m.P1_UDG_WR_Y_01, m.P1_FAV_VOL_Y_01, m.P1_UDG_VOL_Y_01,
  m.P2_FAV_WR_Y_01, m.P2_UDG_WR_Y_01, m.P2_FAV_VOL_Y_01, m.P2_UDG_VOL_Y_01,

  m.P1_FAV_CalibGap_Y_01, m.P1_FAV_CalibAbs_Y_01, m.P1_UDG_CalibGap_Y_01, m.P1_UDG_CalibAbs_Y_01,
  m.P2_FAV_CalibGap_Y_01, m.P2_FAV_CalibAbs_Y_01, m.P2_UDG_CalibGap_Y_01, m.P2_UDG_CalibAbs_Y_01,

  m.P1_FAV_LossShock_Y_01, m.P1_UDG_WinShock_Y_01, m.P1_FavShare_Y_01,
  m.P2_FAV_LossShock_Y_01, m.P2_UDG_WinShock_Y_01, m.P2_FavShare_Y_01,

  f.P1_FAV_Y_isMissing, f.P1_UDG_Y_isMissing, f.P2_FAV_Y_isMissing, f.P2_UDG_Y_isMissing,
  f.P1_FAV_LossShock_Y_isMissing, f.P1_UDG_WinShock_Y_isMissing, f.P2_FAV_LossShock_Y_isMissing, f.P2_UDG_WinShock_Y_isMissing,

  ((m.P1_FAV_WR_Y_01 - m.P2_FAV_WR_Y_01)+1.0)/2.0 AS FAV_WR_Y_Diff_01,
  ((m.P1_UDG_WR_Y_01 - m.P2_UDG_WR_Y_01)+1.0)/2.0 AS UDG_WR_Y_Diff_01,
  ((m.P1_FAV_CalibGap_Y_01 - m.P2_FAV_CalibGap_Y_01)+1.0)/2.0 AS FAV_CalibGap_Y_Diff_01,
  ((m.P1_UDG_CalibGap_Y_01 - m.P2_UDG_CalibGap_Y_01)+1.0)/2.0 AS UDG_CalibGap_Y_Diff_01,
  ((m.P1_FAV_LossShock_Y_01 - m.P2_FAV_LossShock_Y_01)+1.0)/2.0 AS FAV_LossShock_Y_Diff_01,
  ((m.P1_UDG_WinShock_Y_01  - m.P2_UDG_WinShock_Y_01)+1.0)/2.0  AS UDG_WinShock_Y_Diff_01,
  ((m.P1_FavShare_Y_01      - m.P2_FavShare_Y_01)+1.0)/2.0      AS FavShare_Y_Diff_01,

  /* ---------- MONTH (M) ---------- */
  m.P1_FAV_WR_M_01, m.P1_UDG_WR_M_01, m.P1_FAV_VOL_M_01, m.P1_UDG_VOL_M_01,
  m.P2_FAV_WR_M_01, m.P2_UDG_WR_M_01, m.P2_FAV_VOL_M_01, m.P2_UDG_VOL_M_01,

  m.P1_FAV_CalibGap_M_01, m.P1_FAV_CalibAbs_M_01, m.P1_UDG_CalibGap_M_01, m.P1_UDG_CalibAbs_M_01,
  m.P2_FAV_CalibGap_M_01, m.P2_FAV_CalibAbs_M_01, m.P2_UDG_CalibGap_M_01, m.P2_UDG_CalibAbs_M_01,

  m.P1_FAV_LossShock_M_01, m.P1_UDG_WinShock_M_01, m.P1_FavShare_M_01,
  m.P2_FAV_LossShock_M_01, m.P2_UDG_WinShock_M_01, m.P2_FavShare_M_01,

  f.P1_FAV_M_isMissing, f.P1_UDG_M_isMissing, f.P2_FAV_M_isMissing, f.P2_UDG_M_isMissing,
  f.P1_FAV_LossShock_M_isMissing, f.P1_UDG_WinShock_M_isMissing, f.P2_FAV_LossShock_M_isMissing, f.P2_UDG_WinShock_M_isMissing,

  ((m.P1_FAV_WR_M_01 - m.P2_FAV_WR_M_01)+1.0)/2.0 AS FAV_WR_M_Diff_01,
  ((m.P1_UDG_WR_M_01 - m.P2_UDG_WR_M_01)+1.0)/2.0 AS UDG_WR_M_Diff_01,
  ((m.P1_FAV_CalibGap_M_01 - m.P2_FAV_CalibGap_M_01)+1.0)/2.0 AS FAV_CalibGap_M_Diff_01,
  ((m.P1_UDG_CalibGap_M_01 - m.P2_UDG_CalibGap_M_01)+1.0)/2.0 AS UDG_CalibGap_M_Diff_01,
  ((m.P1_FAV_LossShock_M_01 - m.P2_FAV_LossShock_M_01)+1.0)/2.0 AS FAV_LossShock_M_Diff_01,
  ((m.P1_UDG_WinShock_M_01  - m.P2_UDG_WinShock_M_01)+1.0)/2.0  AS UDG_WinShock_M_Diff_01,
  ((m.P1_FavShare_M_01      - m.P2_FavShare_M_01)+1.0)/2.0      AS FavShare_M_Diff_01,

  /* ---------- WEEK (W) ---------- */
  m.P1_FAV_WR_W_01, m.P1_UDG_WR_W_01, m.P1_FAV_VOL_W_01, m.P1_UDG_VOL_W_01,
  m.P2_FAV_WR_W_01, m.P2_UDG_WR_W_01, m.P2_FAV_VOL_W_01, m.P2_UDG_VOL_W_01,

  m.P1_FAV_CalibGap_W_01, m.P1_FAV_CalibAbs_W_01, m.P1_UDG_CalibGap_W_01, m.P1_UDG_CalibAbs_W_01,
  m.P2_FAV_CalibGap_W_01, m.P2_FAV_CalibAbs_W_01, m.P2_UDG_CalibGap_W_01, m.P2_UDG_CalibAbs_W_01,

  m.P1_FAV_LossShock_W_01, m.P1_UDG_WinShock_W_01, m.P1_FavShare_W_01,
  m.P2_FAV_LossShock_W_01, m.P2_UDG_WinShock_W_01, m.P2_FavShare_W_01,

  f.P1_FAV_W_isMissing, f.P1_UDG_W_isMissing, f.P2_FAV_W_isMissing, f.P2_UDG_W_isMissing,
  f.P1_FAV_LossShock_W_isMissing, f.P1_UDG_WinShock_W_isMissing, f.P2_FAV_LossShock_W_isMissing, f.P2_UDG_WinShock_W_isMissing,

  ((m.P1_FAV_WR_W_01 - m.P2_FAV_WR_W_01)+1.0)/2.0 AS FAV_WR_W_Diff_01,
  ((m.P1_UDG_WR_W_01 - m.P2_UDG_WR_W_01)+1.0)/2.0 AS UDG_WR_W_Diff_01,
  ((m.P1_FAV_CalibGap_W_01 - m.P2_FAV_CalibGap_W_01)+1.0)/2.0 AS FAV_CalibGap_W_Diff_01,
  ((m.P1_UDG_CalibGap_W_01 - m.P2_UDG_CalibGap_W_01)+1.0)/2.0 AS UDG_CalibGap_W_Diff_01,
  ((m.P1_FAV_LossShock_W_01 - m.P2_FAV_LossShock_W_01)+1.0)/2.0 AS FAV_LossShock_W_Diff_01,
  ((m.P1_UDG_WinShock_W_01  - m.P2_UDG_WinShock_W_01)+1.0)/2.0  AS UDG_WinShock_W_Diff_01,
  ((m.P1_FavShare_W_01      - m.P2_FavShare_W_01)+1.0)/2.0      AS FavShare_W_Diff_01

FROM meta m
JOIN flags f ON f.MatchTPId = m.MatchTPId;
GO
