USE [BonzoByte]
GO

/****** Object:  View [dbo].[vw_NN_Tournament_features]    Script Date: 22.8.2025. 15:42:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_NN_Tournament_features]
AS
/* Normalizacija iz TournamentEvent bez joinova/casta na lookup tablice */
WITH TE_Norm AS (
  SELECT
      te.TournamentEventTPId,
      te.TournamentEventDate,

      /* TYPE → INT (dozvoli i tekst) */
      COALESCE(
        TRY_CONVERT(int, te.TournamentTypeId),
        CASE LOWER(LTRIM(RTRIM(CONVERT(varchar(50), te.TournamentTypeId))))
          WHEN 'atp doubles' THEN 1
          WHEN 'atp singles' THEN 2
          WHEN 'wta doubles' THEN 3
          WHEN 'wta singles' THEN 4
          ELSE NULL
        END
      ) AS TE_TypeId_Norm,

      /* LEVEL → INT ili label ('>50K','<50K','q','cup') */
      COALESCE(
        TRY_CONVERT(int, te.TournamentLevelId),
        CASE LTRIM(RTRIM(CONVERT(varchar(50), te.TournamentLevelId)))
          WHEN '>50K' THEN 1
          WHEN '<50K' THEN 2
          WHEN 'q'    THEN 3
          WHEN 'cup'  THEN 4
          ELSE NULL
        END
      ) AS TE_LevelId_Norm,

      /* SURFACE → INT ili tekst ('unknown','clay','grass','hard') */
      COALESCE(
        TRY_CONVERT(int, te.SurfaceId),
        CASE LOWER(LTRIM(RTRIM(CONVERT(varchar(50), te.SurfaceId))))
          WHEN 'unknown' THEN 1
          WHEN 'clay'    THEN 2
          WHEN 'grass'   THEN 3
          WHEN 'hard'    THEN 4
          ELSE NULL
        END
      ) AS TE_SurfaceId_Norm,

      /* PRIZE → FLOAT (0/NULL = missing) */
      TRY_CONVERT(float, te.[Prize]) AS Prize_Float
  FROM dbo.TournamentEvent te
),
Base AS (
  SELECT
      m.MatchTPId,
      n.TE_TypeId_Norm,
      n.TE_LevelId_Norm,
      n.TE_SurfaceId_Norm,
      n.Prize_Float,
      YEAR(COALESCE(n.TournamentEventDate, m.[DateTime])) AS EventYear,
      CASE WHEN n.Prize_Float > 0 THEN LOG10(n.Prize_Float + 1.0) END AS LPrize
  FROM dbo.Match m
  JOIN TE_Norm n
    ON n.TournamentEventTPId = m.TournamentEventTPId
  WHERE n.TE_TypeId_Norm IN (2,4)   -- singles only
)
SELECT
    b.MatchTPId,

    /* ===== TournamentType (binarno, 0–1) ===== */
    CASE WHEN b.TE_TypeId_Norm = 4 THEN 1 ELSE 0 END AS Type_IsWTA,  -- 1=WTA, 0=ATP

    /* ===== Prize (0–1, bez NULL) ===== */
    CASE WHEN b.Prize_Float IS NULL OR b.Prize_Float <= 0 THEN 1 ELSE 0 END AS Prize_isMissing,

    /* Percentil po (godina, tip) – inflation-aware */
    CASE 
      WHEN b.Prize_Float > 0 THEN 
           PERCENT_RANK() OVER (PARTITION BY b.EventYear, b.TE_TypeId_Norm ORDER BY b.Prize_Float)
      ELSE 0
    END AS Prize_Pctl_YearType,

    /* Global log-minmax 0–1 (cross-era) */
    CASE 
      WHEN b.Prize_Float > 0 THEN 
           (b.LPrize - MIN(b.LPrize) OVER ())
           / NULLIF(MAX(b.LPrize) OVER () - MIN(b.LPrize) OVER (), 0)
      ELSE 0
    END AS Prize_LogNorm_Global_0_1,

    /* ===== TournamentLevel (0–1, bez NULL) ===== */
    CASE WHEN b.TE_LevelId_Norm IS NULL THEN 1 ELSE 0 END AS Level_isMissing,
    CASE WHEN b.TE_LevelId_Norm = 1 THEN 1 ELSE 0 END AS Lvl_gt50k,
    CASE WHEN b.TE_LevelId_Norm = 2 THEN 1 ELSE 0 END AS Lvl_lt50k,
    CASE WHEN b.TE_LevelId_Norm = 3 THEN 1 ELSE 0 END AS Lvl_q,
    CASE WHEN b.TE_LevelId_Norm = 4 THEN 1 ELSE 0 END AS Lvl_cup,

    /* Ordinalni prior (0–1); missing → 0.50 */
    CASE 
      WHEN b.TE_LevelId_Norm = 3 THEN 0.00  -- q
      WHEN b.TE_LevelId_Norm = 2 THEN 0.33  -- <50K
      WHEN b.TE_LevelId_Norm = 4 THEN 0.50  -- cup
      WHEN b.TE_LevelId_Norm = 1 THEN 1.00  -- >50K
      ELSE 0.50
    END AS Level_Prior_01,

    /* ===== Surface (0–1, bez NULL) ===== */
    /* one-hot */
    CASE WHEN b.TE_SurfaceId_Norm IS NULL OR b.TE_SurfaceId_Norm = 1 THEN 1 ELSE 0 END AS Surf_Unknown,
    CASE WHEN b.TE_SurfaceId_Norm = 2 THEN 1 ELSE 0 END AS Surf_Clay,
    CASE WHEN b.TE_SurfaceId_Norm = 3 THEN 1 ELSE 0 END AS Surf_Grass,
    CASE WHEN b.TE_SurfaceId_Norm = 4 THEN 1 ELSE 0 END AS Surf_Hard,

    /* flagovi */
    CASE WHEN b.TE_SurfaceId_Norm IS NULL OR b.TE_SurfaceId_Norm = 1 THEN 1 ELSE 0 END AS Surface_isUnknown,
    CASE WHEN b.TE_SurfaceId_Norm IS NULL OR b.TE_SurfaceId_Norm = 1 THEN 0 ELSE 1 END AS Surface_isKnown,

    /* speed prior 0–1: clay=0.00, hard=0.50, grass=1.00, unknown→0.50 */
    CASE 
      WHEN b.TE_SurfaceId_Norm = 2 THEN 0.00
      WHEN b.TE_SurfaceId_Norm = 4 THEN 0.50
      WHEN b.TE_SurfaceId_Norm = 3 THEN 1.00
      ELSE 0.50  -- unknown / missing
    END AS SurfaceSpeedIdx_01,

    /* ===== BACK-COMPAT ALIASI (radi tvojih SELECT-a) ===== */
    /* pm1 varijanta iz 0–1 */
    (2.0 * (
      CASE 
        WHEN b.Prize_Float > 0 THEN 
             (b.LPrize - MIN(b.LPrize) OVER ())
             / NULLIF(MAX(b.LPrize) OVER () - MIN(b.LPrize) OVER (), 0)
        ELSE 0
      END
    ) - 1.0) AS Prize_LogNorm_Global_pm1,

    /* stari naziv za “unknown” flag */
    CASE WHEN b.TE_SurfaceId_Norm IS NULL OR b.TE_SurfaceId_Norm = 1 THEN 1 ELSE 0 END AS Surface_isMissing,

    /* stari naziv za speed prior (identično _01) */
    CASE 
      WHEN b.TE_SurfaceId_Norm = 2 THEN 0.00
      WHEN b.TE_SurfaceId_Norm = 4 THEN 0.50
      WHEN b.TE_SurfaceId_Norm = 3 THEN 1.00
      ELSE 0.50
    END AS SurfaceSpeedIdx,

    /* posebna varijanta: unknown & cup → 0.50 (ionako 0.50 za unknown) */
    CASE 
      WHEN (b.TE_SurfaceId_Norm IS NULL OR b.TE_SurfaceId_Norm = 1)
           AND b.TE_LevelId_Norm = 4 THEN 0.50
      WHEN b.TE_SurfaceId_Norm = 2 THEN 0.00
      WHEN b.TE_SurfaceId_Norm = 4 THEN 0.50
      WHEN b.TE_SurfaceId_Norm = 3 THEN 1.00
      ELSE 0.50
    END AS SurfaceSpeedIdx_ImputedCup05

FROM Base b;
GO


