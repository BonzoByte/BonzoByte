USE [BonzoByte]
GO

/****** Object:  View [dbo].[vw_NN_Round_Season_features]    Script Date: 22.8.2025. 15:41:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_NN_Round_Season_features]
AS
WITH Base AS (
    SELECT
        m.MatchTPId,
        COALESCE(m.[DateTime], te.TournamentEventDate) AS BaseDate,
        LOWER(LTRIM(RTRIM(r.RoundName))) AS RoundNameClean
    FROM dbo.Match m
    LEFT JOIN dbo.TournamentEvent te ON te.TournamentEventTPId = m.TournamentEventTPId
    LEFT JOIN dbo.Round r            ON r.RoundId             = m.RoundId
),
RoundMap AS (
    SELECT
        b.MatchTPId,
        b.BaseDate,
        b.RoundNameClean,
        CASE
          WHEN b.RoundNameClean IS NULL OR b.RoundNameClean = '' THEN NULL

          /* Final */
          WHEN b.RoundNameClean IN ('final','finals','f') THEN 6

          /* Semi */
          WHEN b.RoundNameClean IN ('semi final','semifinal','semi finals','semifinals','semi-final','semi-finals','sf') THEN 5

          /* Quarter */
          WHEN b.RoundNameClean IN ('quarter final','quarterfinal','quarter finals','quarterfinals','quarter-final','quarter-finals','qf','5th round') THEN 4

          /* R16 / Fourth */
          WHEN b.RoundNameClean IN ('round of 16','r16','fourth round','4th round') THEN 3

          /* R32 / Third */
          WHEN b.RoundNameClean IN ('round of 32','r32','third round','3rd round') THEN 2

          /* R64 / Second */
          WHEN b.RoundNameClean IN ('round of 64','r64','second round','2nd round') THEN 1

          /* R128 / First */
          WHEN b.RoundNameClean IN ('round of 128','r128','first round','1st round') THEN 0

          /* Qualifying: PAZI — bez 'q%' da ne uhvatimo 'quarter' */
          WHEN b.RoundNameClean = 'qualifications' OR b.RoundNameClean LIKE 'qual%' THEN -1

          ELSE NULL
        END AS Round_Ordinal
    FROM Base b
)
SELECT
    rm.MatchTPId,

    /* Flags */
    CASE WHEN rm.Round_Ordinal IS NULL THEN 1 ELSE 0 END AS Round_isMissing,
    CASE WHEN rm.Round_Ordinal = 6 THEN 1 ELSE 0 END AS Round_isFinal,
    CASE WHEN rm.Round_Ordinal = 5 THEN 1 ELSE 0 END AS Round_isSF,
    CASE WHEN rm.Round_Ordinal = 4 THEN 1 ELSE 0 END AS Round_isQF,
    CASE WHEN rm.Round_Ordinal = 3 THEN 1 ELSE 0 END AS Round_isR16,
    CASE WHEN rm.Round_Ordinal = 2 THEN 1 ELSE 0 END AS Round_isR32,
    CASE WHEN rm.Round_Ordinal = 1 THEN 1 ELSE 0 END AS Round_isR64,
    CASE WHEN rm.Round_Ordinal = 0 THEN 1 ELSE 0 END AS Round_isR128,
    CASE WHEN rm.Round_ordinal = -1 THEN 1 ELSE 0 END AS Round_isQual,

    /* Ordinal 0–1 (kvale=0, finale=1, unknown=0.5) */
    CASE WHEN rm.Round_Ordinal IS NULL THEN 0.50
         ELSE (rm.Round_Ordinal + 1.0) / 7.0 END AS Round_Ordinal_01,

    /* DOW 0–1 (neovisno o DATEFIRST) */
    ((DATEDIFF(DAY, '19000101', rm.BaseDate) % 7) + 7) % 7 AS DOW_Idx_0_6,
    (SIN(2*PI() * ( ((DATEDIFF(DAY,'19000101', rm.BaseDate) % 7 + 7) % 7) / 7.0 )) + 1.0) / 2.0 AS DOW_Sin01,
    (COS(2*PI() * ( ((DATEDIFF(DAY,'19000101', rm.BaseDate) % 7 + 7) % 7) / 7.0 )) + 1.0) / 2.0 AS DOW_Cos01,

    /* DOY 0–1 */
    DATEPART(DAYOFYEAR, rm.BaseDate) AS DOY_1_366,
    (SIN(2*PI() * ((DATEPART(DAYOFYEAR, rm.BaseDate) - 1) / 365.25)) + 1.0) / 2.0 AS Season_Sin01,
    (COS(2*PI() * ((DATEPART(DAYOFYEAR, rm.BaseDate) - 1) / 365.25)) + 1.0) / 2.0 AS Season_Cos01

FROM RoundMap rm;
GO


