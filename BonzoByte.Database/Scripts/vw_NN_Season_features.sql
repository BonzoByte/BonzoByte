USE [BonzoByte]
GO

/****** Object:  View [dbo].[vw_NN_Season_features]    Script Date: 22.8.2025. 15:42:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_NN_Season_features]
AS
/* Odaberi bazni datum: Match.DateTime ili, ako fali, TournamentEventDate */
WITH Base AS (
  SELECT
      m.MatchTPId,
      COALESCE(m.[DateTime], te.TournamentEventDate) AS BaseDate
  FROM dbo.Match m
  LEFT JOIN dbo.TournamentEvent te
    ON te.TournamentEventTPId = m.TournamentEventTPId
)
SELECT
    b.MatchTPId,

    /* --- Dan u tjednu (DOW) --- 
       Deterministički izračun neovisan o SET DATEFIRST:
       1900-01-01 je ponedjeljak u SQL Serveru; modulo daj 0..6.
    */
    ((DATEDIFF(DAY, '19000101', b.BaseDate) % 7) + 7) % 7 AS DOW_Idx_0_6,

    /* Sin/Cos u 0–1 (ciklički) */
    (SIN(2 * PI() * ( ( (DATEDIFF(DAY,'19000101', b.BaseDate) % 7 + 7) % 7 ) / 7.0 )) + 1.0) / 2.0 AS DOW_Sin01,
    (COS(2 * PI() * ( ( (DATEDIFF(DAY,'19000101', b.BaseDate) % 7 + 7) % 7 ) / 7.0 )) + 1.0) / 2.0 AS DOW_Cos01,

    /* --- Dan u godini (DOY) --- */
    DATEPART(DAYOFYEAR, b.BaseDate) AS DOY_1_366,

    /* Sin/Cos u 0–1; 365.25 ublažava prijelaze preko prijestupnih godina */
    (SIN(2 * PI() * ((DATEPART(DAYOFYEAR, b.BaseDate) - 1) / 365.25)) + 1.0) / 2.0 AS Season_Sin01,
    (COS(2 * PI() * ((DATEPART(DAYOFYEAR, b.BaseDate) - 1) / 365.25)) + 1.0) / 2.0 AS Season_Cos01

FROM Base b;
GO


