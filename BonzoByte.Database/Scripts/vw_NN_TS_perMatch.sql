CREATE VIEW dbo.vw_NN_TS_perMatch
AS
WITH base AS (
    SELECT
        m.MatchTPId,
        m.SurfaceId,

        -- Agnostični p (već 0–1)
        CAST(m.[WinProbabilityPlayer1M]  AS float) AS pM_raw,
        CAST(m.[WinProbabilityPlayer1SM] AS float) AS pSM_raw,
        CAST(m.[WinProbabilityPlayer1GSM] AS float) AS pGSM_raw,

        -- Surface-specifični p za STVARNI SurfaceId (1..4)
        CAST(CASE m.SurfaceId
             WHEN 1 THEN m.[WinProbabilityPlayer1MS1]
             WHEN 2 THEN m.[WinProbabilityPlayer1MS2]
             WHEN 3 THEN m.[WinProbabilityPlayer1MS3]
             WHEN 4 THEN m.[WinProbabilityPlayer1MS4]
        END AS float) AS pMS_S_raw,

        CAST(CASE m.SurfaceId
             WHEN 1 THEN m.[WinProbabilityPlayer1SMS1]
             WHEN 2 THEN m.[WinProbabilityPlayer1SMS2]
             WHEN 3 THEN m.[WinProbabilityPlayer1SMS3]
             WHEN 4 THEN m.[WinProbabilityPlayer1SMS4]
        END AS float) AS pSMS_S_raw,

        CAST(CASE m.SurfaceId
             WHEN 1 THEN m.[WinProbabilityPlayer1GSMS1]
             WHEN 2 THEN m.[WinProbabilityPlayer1GSMS2]
             WHEN 3 THEN m.[WinProbabilityPlayer1GSMS3]
             WHEN 4 THEN m.[WinProbabilityPlayer1GSMS4]
        END AS float) AS pGSMS_S_raw,

        -- "Uncertainty" = (σ1+σ2)/(2*σ0), σ0≈8.3333  → [0..1], clamp kasnije
        CAST(m.[Player1TrueSkillStandardDeviationOldM]  AS float) 
          + CAST(m.[Player2TrueSkillStandardDeviationOldM]  AS float) AS sdSum_M_raw,

        CAST(m.[Player1TrueSkillStandardDeviationOldSM] AS float) 
          + CAST(m.[Player2TrueSkillStandardDeviationOldSM] AS float) AS sdSum_SM_raw,

        CAST(m.[Player1TrueSkillStandardDeviationOldGSM] AS float) 
          + CAST(m.[Player2TrueSkillStandardDeviationOldGSM] AS float) AS sdSum_GSM_raw,

        CAST(CASE m.SurfaceId
             WHEN 1 THEN m.[Player1TrueSkillStandardDeviationOldMS1] + m.[Player2TrueSkillStandardDeviationOldMS1]
             WHEN 2 THEN m.[Player1TrueSkillStandardDeviationOldMS2] + m.[Player2TrueSkillStandardDeviationOldMS2]
             WHEN 3 THEN m.[Player1TrueSkillStandardDeviationOldMS3] + m.[Player2TrueSkillStandardDeviationOldMS3]
             WHEN 4 THEN m.[Player1TrueSkillStandardDeviationOldMS4] + m.[Player2TrueSkillStandardDeviationOldMS4]
        END AS float) AS sdSum_MS_S_raw,

        CAST(CASE m.SurfaceId
             WHEN 1 THEN m.[Player1TrueSkillStandardDeviationOldSMS1] + m.[Player2TrueSkillStandardDeviationOldSMS1]
             WHEN 2 THEN m.[Player1TrueSkillStandardDeviationOldSMS2] + m.[Player2TrueSkillStandardDeviationOldSMS2]
             WHEN 3 THEN m.[Player1TrueSkillStandardDeviationOldSMS3] + m.[Player2TrueSkillStandardDeviationOldSMS3]
             WHEN 4 THEN m.[Player1TrueSkillStandardDeviationOldSMS4] + m.[Player2TrueSkillStandardDeviationOldSMS4]
        END AS float) AS sdSum_SMS_S_raw,

        CAST(CASE m.SurfaceId
             WHEN 1 THEN m.[Player1TrueSkillStandardDeviationOldGSMS1] + m.[Player2TrueSkillStandardDeviationOldGSMS1]
             WHEN 2 THEN m.[Player1TrueSkillStandardDeviationOldGSMS2] + m.[Player2TrueSkillStandardDeviationOldGSMS2]
             WHEN 3 THEN m.[Player1TrueSkillStandardDeviationOldGSMS3] + m.[Player2TrueSkillStandardDeviationOldGSMS3]
             WHEN 4 THEN m.[Player1TrueSkillStandardDeviationOldGSMS4] + m.[Player2TrueSkillStandardDeviationOldGSMS4]
        END AS float) AS sdSum_GSMS_S_raw
    FROM dbo.[Match] m
),
pfix AS (
    -- Default p=0.5 kad nedostaje; flagovi govore modelu da je fallback
    SELECT
        b.MatchTPId,

        COALESCE(b.pM_raw,     0.5) AS TS_pM_01,
        COALESCE(b.pSM_raw,    0.5) AS TS_pSM_01,
        COALESCE(b.pGSM_raw,   0.5) AS TS_pGSM_01,
        COALESCE(b.pMS_S_raw,  0.5) AS TS_pMS_S_01,
        COALESCE(b.pSMS_S_raw, 0.5) AS TS_pSMS_S_01,
        COALESCE(b.pGSMS_S_raw,0.5) AS TS_pGSMS_S_01,

        CASE WHEN b.pM_raw     IS NULL THEN 1 ELSE 0 END AS TS_M_isMissing,
        CASE WHEN b.pSM_raw    IS NULL THEN 1 ELSE 0 END AS TS_SM_isMissing,
        CASE WHEN b.pGSM_raw   IS NULL THEN 1 ELSE 0 END AS TS_GSM_isMissing,
        CASE WHEN b.pMS_S_raw  IS NULL THEN 1 ELSE 0 END AS TS_MS_S_isMissing,
        CASE WHEN b.pSMS_S_raw IS NULL THEN 1 ELSE 0 END AS TS_SMS_S_isMissing,
        CASE WHEN b.pGSMS_S_raw IS NULL THEN 1 ELSE 0 END AS TS_GSMS_S_isMissing,

        -- Uncertainty (σ1+σ2)/(2*8.3333); missing -> 1 (konzervativno)
        CASE 
            WHEN b.sdSum_M_raw IS NULL THEN 1
            ELSE CASE WHEN (b.sdSum_M_raw / (2*8.3333333)) > 1 THEN 1 ELSE (b.sdSum_M_raw / (2*8.3333333)) END
        END AS TS_uncert_M_01,

        CASE 
            WHEN b.sdSum_SM_raw IS NULL THEN 1
            ELSE CASE WHEN (b.sdSum_SM_raw / (2*8.3333333)) > 1 THEN 1 ELSE (b.sdSum_SM_raw / (2*8.3333333)) END
        END AS TS_uncert_SM_01,

        CASE 
            WHEN b.sdSum_GSM_raw IS NULL THEN 1
            ELSE CASE WHEN (b.sdSum_GSM_raw / (2*8.3333333)) > 1 THEN 1 ELSE (b.sdSum_GSM_raw / (2*8.3333333)) END
        END AS TS_uncert_GSM_01,

        CASE 
            WHEN b.sdSum_MS_S_raw IS NULL THEN 1
            ELSE CASE WHEN (b.sdSum_MS_S_raw / (2*8.3333333)) > 1 THEN 1 ELSE (b.sdSum_MS_S_raw / (2*8.3333333)) END
        END AS TS_uncert_MS_S_01,

        CASE 
            WHEN b.sdSum_SMS_S_raw IS NULL THEN 1
            ELSE CASE WHEN (b.sdSum_SMS_S_raw / (2*8.3333333)) > 1 THEN 1 ELSE (b.sdSum_SMS_S_raw / (2*8.3333333)) END
        END AS TS_uncert_SMS_S_01,

        CASE 
            WHEN b.sdSum_GSMS_S_raw IS NULL THEN 1
            ELSE CASE WHEN (b.sdSum_GSMS_S_raw / (2*8.3333333)) > 1 THEN 1 ELSE (b.sdSum_GSMS_S_raw / (2*8.3333333)) END
        END AS TS_uncert_GSMS_S_01
    FROM base b
),
meta AS (
    -- pAvg + spread + delte (mapirane u [0,1])
    SELECT
        p.*,
        ca.pmax, ca.pmin,

        -- Prosjek šest kanala
        (p.TS_pM_01 + p.TS_pSM_01 + p.TS_pGSM_01 + p.TS_pMS_S_01 + p.TS_pSMS_S_01 + p.TS_pGSMS_S_01) / 6.0 AS TS_pAvg_01,

        -- Spread (max - min) ∈ [0,1]
        (ca.pmax - ca.pmin) AS TS_spread_01,

        -- Surface delte (p(surface) − p(agnostični)), mapirane u [0,1]
        ((p.TS_pMS_S_01  - p.TS_pM_01   + 1.0)/2.0) AS TS_delta_M_01,
        ((p.TS_pSMS_S_01 - p.TS_pSM_01  + 1.0)/2.0) AS TS_delta_SM_01,
        ((p.TS_pGSMS_S_01- p.TS_pGSM_01 + 1.0)/2.0) AS TS_delta_GSM_01,

        -- Koliko modela je prisutno (0..6)
        (1 - p.TS_M_isMissing) 
      + (1 - p.TS_SM_isMissing) 
      + (1 - p.TS_GSM_isMissing) 
      + (1 - p.TS_MS_S_isMissing) 
      + (1 - p.TS_SMS_S_isMissing) 
      + (1 - p.TS_GSMS_S_isMissing) AS TS_modelsPresent
    FROM pfix p
    CROSS APPLY (
        SELECT 
            MAX(v) AS pmax,
            MIN(v) AS pmin
        FROM (VALUES
            (p.TS_pM_01),
            (p.TS_pSM_01),
            (p.TS_pGSM_01),
            (p.TS_pMS_S_01),
            (p.TS_pSMS_S_01),
            (p.TS_pGSMS_S_01)
        ) AS t(v)
    ) ca
)
SELECT
    m.MatchTPId,

    -- 6 glavnih TS p-kanala (0–1)
    m.TS_pM_01, m.TS_pSM_01, m.TS_pGSM_01,
    m.TS_pMS_S_01, m.TS_pSMS_S_01, m.TS_pGSMS_S_01,

    -- Meta signali (0–1)
    m.TS_pAvg_01,
    m.TS_spread_01,
    m.TS_delta_M_01, m.TS_delta_SM_01, m.TS_delta_GSM_01,

    -- Missing flagovi
    m.TS_M_isMissing, m.TS_SM_isMissing, m.TS_GSM_isMissing,
    m.TS_MS_S_isMissing, m.TS_SMS_S_isMissing, m.TS_GSMS_S_isMissing,

    -- Broj prisutnih modela (0..6)
    m.TS_modelsPresent,

    -- Uncertainty kanali (0–1), veće = više nesigurnosti
    m.TS_uncert_M_01, m.TS_uncert_SM_01, m.TS_uncert_GSM_01,
    m.TS_uncert_MS_S_01, m.TS_uncert_SMS_S_01, m.TS_uncert_GSMS_S_01
FROM meta m;
GO
