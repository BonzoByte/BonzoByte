CREATE VIEW dbo.vw_NN_H2H_perMatch
AS
/* ======================================================
   Parametri
   ====================================================== */
WITH param AS (
    SELECT CAST(8.333333 AS float) AS SIGMA_CAP  -- gornja granica za σ normalizaciju (≈ TS domain)
),
/* ======================================================
   Sirovi H2H (OLD) + TS (global) + surface-pick
   ====================================================== */
base AS (
    SELECT
        m.MatchTPId,
        m.SurfaceId,

        /* ---------- H2H pWin (pre-match, OLD), GLOBAL (M/SM/GSM) ---------- */
        CAST(m.WinProbabilityPlayer1H2HM   AS float) AS pH2H_M,
        CAST(m.WinProbabilityPlayer1H2HSM  AS float) AS pH2H_SM,
        CAST(m.WinProbabilityPlayer1H2HGSM AS float) AS pH2H_GSM,

        /* σ (OLD), GLOBAL */
        CAST(m.Player1H2HTrueSkillStandardDeviationOldM   AS float) AS P1_S_M,
        CAST(m.Player2H2HTrueSkillStandardDeviationOldM   AS float) AS P2_S_M,
        CAST(m.Player1H2HTrueSkillStandardDeviationOldSM  AS float) AS P1_S_SM,
        CAST(m.Player2H2HTrueSkillStandardDeviationOldSM  AS float) AS P2_S_SM,
        CAST(m.Player1H2HTrueSkillStandardDeviationOldGSM AS float) AS P1_S_GSM,
        CAST(m.Player2H2HTrueSkillStandardDeviationOldGSM AS float) AS P2_S_GSM,

        /* ---------- H2H pWin (pre-match, OLD), SURFACE-pick (MS/SMS/GSMS) ---------- */
        CAST(CASE m.SurfaceId WHEN 1 THEN m.WinProbabilityPlayer1H2HMS1 WHEN 2 THEN m.WinProbabilityPlayer1H2HMS2
                              WHEN 3 THEN m.WinProbabilityPlayer1H2HMS3 WHEN 4 THEN m.WinProbabilityPlayer1H2HMS4 END AS float) AS pH2H_MS,
        CAST(CASE m.SurfaceId WHEN 1 THEN m.WinProbabilityPlayer1H2HSMS1 WHEN 2 THEN m.WinProbabilityPlayer1H2HSMS2
                              WHEN 3 THEN m.WinProbabilityPlayer1H2HSMS3 WHEN 4 THEN m.WinProbabilityPlayer1H2HSMS4 END AS float) AS pH2H_SMS,
        CAST(CASE m.SurfaceId WHEN 1 THEN m.WinProbabilityPlayer1H2HGSMS1 WHEN 2 THEN m.WinProbabilityPlayer1H2HGSMS2
                              WHEN 3 THEN m.WinProbabilityPlayer1H2HGSMS3 WHEN 4 THEN m.WinProbabilityPlayer1H2HGSMS4 END AS float) AS pH2H_GSMS,

        /* σ (OLD), SURFACE-pick */
        CAST(CASE m.SurfaceId WHEN 1 THEN m.Player1H2HTrueSkillStandardDeviationOldMS1 WHEN 2 THEN m.Player1H2HTrueSkillStandardDeviationOldMS2
                              WHEN 3 THEN m.Player1H2HTrueSkillStandardDeviationOldMS3 WHEN 4 THEN m.Player1H2HTrueSkillStandardDeviationOldMS4 END AS float) AS P1_S_MS,
        CAST(CASE m.SurfaceId WHEN 1 THEN m.Player2H2HTrueSkillStandardDeviationOldMS1 WHEN 2 THEN m.Player2H2HTrueSkillStandardDeviationOldMS2
                              WHEN 3 THEN m.Player2H2HTrueSkillStandardDeviationOldMS3 WHEN 4 THEN m.Player2H2HTrueSkillStandardDeviationOldMS4 END AS float) AS P2_S_MS,

        CAST(CASE m.SurfaceId WHEN 1 THEN m.Player1H2HTrueSkillStandardDeviationOldSMS1 WHEN 2 THEN m.Player1H2HTrueSkillStandardDeviationOldSMS2
                              WHEN 3 THEN m.Player1H2HTrueSkillStandardDeviationOldSMS3 WHEN 4 THEN m.Player1H2HTrueSkillStandardDeviationOldSMS4 END AS float) AS P1_S_SMS,
        CAST(CASE m.SurfaceId WHEN 1 THEN m.Player2H2HTrueSkillStandardDeviationOldSMS1 WHEN 2 THEN m.Player2H2HTrueSkillStandardDeviationOldSMS2
                              WHEN 3 THEN m.Player2H2HTrueSkillStandardDeviationOldSMS3 WHEN 4 THEN m.Player2H2HTrueSkillStandardDeviationOldSMS4 END AS float) AS P2_S_SMS,

        CAST(CASE m.SurfaceId WHEN 1 THEN m.Player1H2HTrueSkillStandardDeviationOldGSMS1 WHEN 2 THEN m.Player1H2HTrueSkillStandardDeviationOldGSMS2
                              WHEN 3 THEN m.Player1H2HTrueSkillStandardDeviationOldGSMS3 WHEN 4 THEN m.Player1H2HTrueSkillStandardDeviationOldGSMS4 END AS float) AS P1_S_GSMS,
        CAST(CASE m.SurfaceId WHEN 1 THEN m.Player2H2HTrueSkillStandardDeviationOldGSMS1 WHEN 2 THEN m.Player2H2HTrueSkillStandardDeviationOldGSMS2
                              WHEN 3 THEN m.Player2H2HTrueSkillStandardDeviationOldGSMS3 WHEN 4 THEN m.Player2H2HTrueSkillStandardDeviationOldGSMS4 END AS float) AS P2_S_GSMS,

        /* ---------- TS pWin (bez H2H) GLOBAL & SURFACE ---------- */
        CAST(m.WinProbabilityPlayer1M   AS float) AS pTS_M,
        CAST(m.WinProbabilityPlayer1SM  AS float) AS pTS_SM,
        CAST(m.WinProbabilityPlayer1GSM AS float) AS pTS_GSM,

        CAST(CASE m.SurfaceId WHEN 1 THEN m.WinProbabilityPlayer1MS1 WHEN 2 THEN m.WinProbabilityPlayer1MS2
                              WHEN 3 THEN m.WinProbabilityPlayer1MS3 WHEN 4 THEN m.WinProbabilityPlayer1MS4 END AS float) AS pTS_MS,
        CAST(CASE m.SurfaceId WHEN 1 THEN m.WinProbabilityPlayer1SMS1 WHEN 2 THEN m.WinProbabilityPlayer1SMS2
                              WHEN 3 THEN m.WinProbabilityPlayer1SMS3 WHEN 4 THEN m.WinProbabilityPlayer1SMS4 END AS float) AS pTS_SMS,
        CAST(CASE m.SurfaceId WHEN 1 THEN m.WinProbabilityPlayer1GSMS1 WHEN 2 THEN m.WinProbabilityPlayer1GSMS2
                              WHEN 3 THEN m.WinProbabilityPlayer1GSMS3 WHEN 4 THEN m.WinProbabilityPlayer1GSMS4 END AS float) AS pTS_GSMS
    FROM dbo.[Match] m
),
/* ======================================================
   Agregati global (M/SM/GSM) i surface (MS/SMS/GSMS)
   ====================================================== */
agg AS (
    SELECT
      b.MatchTPId,

      /* --- H2H GLOBAL agregat --- */
      (SELECT AVG(v) FROM (VALUES (b.pH2H_M),(b.pH2H_SM),(b.pH2H_GSM)) t(v)) AS pH2H_glb_raw,
      (SELECT MAX(v) - MIN(v) FROM (VALUES (b.pH2H_M),(b.pH2H_SM),(b.pH2H_GSM)) t(v) WHERE v IS NOT NULL) AS pH2H_glb_range,
      (SELECT STDEV(v) FROM (VALUES (b.pH2H_M),(b.pH2H_SM),(b.pH2H_GSM)) t(v) WHERE v IS NOT NULL) AS pH2H_glb_stdev,
      (CASE WHEN b.pH2H_M IS NULL  THEN 0 ELSE 1 END
       +     CASE WHEN b.pH2H_SM IS NULL THEN 0 ELSE 1 END
       +     CASE WHEN b.pH2H_GSM IS NULL THEN 0 ELSE 1 END) AS pH2H_glb_cnt,

      /* --- H2H SURFACE agregat --- */
      (SELECT AVG(v) FROM (VALUES (b.pH2H_MS),(b.pH2H_SMS),(b.pH2H_GSMS)) t(v)) AS pH2H_sfc_raw,
      (SELECT MAX(v) - MIN(v) FROM (VALUES (b.pH2H_MS),(b.pH2H_SMS),(b.pH2H_GSMS)) t(v) WHERE v IS NOT NULL) AS pH2H_sfc_range,
      (SELECT STDEV(v) FROM (VALUES (b.pH2H_MS),(b.pH2H_SMS),(b.pH2H_GSMS)) t(v) WHERE v IS NOT NULL) AS pH2H_sfc_stdev,
      (CASE WHEN b.pH2H_MS IS NULL  THEN 0 ELSE 1 END
       +     CASE WHEN b.pH2H_SMS IS NULL THEN 0 ELSE 1 END
       +     CASE WHEN b.pH2H_GSMS IS NULL THEN 0 ELSE 1 END) AS pH2H_sfc_cnt,

      /* --- Combined σ per model + prosjeci --- */
      /* global */
      CASE WHEN b.P1_S_M   IS NOT NULL AND b.P2_S_M   IS NOT NULL THEN SQRT(POWER(b.P1_S_M  ,2)+POWER(b.P2_S_M  ,2)) END AS sigma_M,
      CASE WHEN b.P1_S_SM  IS NOT NULL AND b.P2_S_SM  IS NOT NULL THEN SQRT(POWER(b.P1_S_SM ,2)+POWER(b.P2_S_SM ,2)) END AS sigma_SM,
      CASE WHEN b.P1_S_GSM IS NOT NULL AND b.P2_S_GSM IS NOT NULL THEN SQRT(POWER(b.P1_S_GSM,2)+POWER(b.P2_S_GSM,2)) END AS sigma_GSM,
      (SELECT AVG(v) FROM (VALUES (
        CASE WHEN b.P1_S_M   IS NOT NULL AND b.P2_S_M   IS NOT NULL THEN SQRT(POWER(b.P1_S_M  ,2)+POWER(b.P2_S_M  ,2)) END),
        (CASE WHEN b.P1_S_SM  IS NOT NULL AND b.P2_S_SM  IS NOT NULL THEN SQRT(POWER(b.P1_S_SM ,2)+POWER(b.P2_S_SM ,2)) END),
        (CASE WHEN b.P1_S_GSM IS NOT NULL AND b.P2_S_GSM IS NOT NULL THEN SQRT(POWER(b.P1_S_GSM,2)+POWER(b.P2_S_GSM,2)) END)
      ) t(v)) AS sigma_glb_avg,

      /* surface */
      CASE WHEN b.P1_S_MS   IS NOT NULL AND b.P2_S_MS   IS NOT NULL THEN SQRT(POWER(b.P1_S_MS  ,2)+POWER(b.P2_S_MS  ,2)) END AS sigma_MS,
      CASE WHEN b.P1_S_SMS  IS NOT NULL AND b.P2_S_SMS  IS NOT NULL THEN SQRT(POWER(b.P1_S_SMS ,2)+POWER(b.P2_S_SMS ,2)) END AS sigma_SMS,
      CASE WHEN b.P1_S_GSMS IS NOT NULL AND b.P2_S_GSMS IS NOT NULL THEN SQRT(POWER(b.P1_S_GSMS,2)+POWER(b.P2_S_GSMS,2)) END AS sigma_GSMS,
      (SELECT AVG(v) FROM (VALUES (
        CASE WHEN b.P1_S_MS   IS NOT NULL AND b.P2_S_MS   IS NOT NULL THEN SQRT(POWER(b.P1_S_MS  ,2)+POWER(b.P2_S_MS  ,2)) END),
        (CASE WHEN b.P1_S_SMS  IS NOT NULL AND b.P2_S_SMS  IS NOT NULL THEN SQRT(POWER(b.P1_S_SMS ,2)+POWER(b.P2_S_SMS ,2)) END),
        (CASE WHEN b.P1_S_GSMS IS NOT NULL AND b.P2_S_GSMS IS NOT NULL THEN SQRT(POWER(b.P1_S_GSMS,2)+POWER(b.P2_S_GSMS,2)) END)
      ) t(v)) AS sigma_sfc_avg,

      /* --- TS agregati (za delte) --- */
      (SELECT AVG(v) FROM (VALUES (b.pTS_M),(b.pTS_SM),(b.pTS_GSM)) t(v)) AS pTS_glb_raw,
      (SELECT AVG(v) FROM (VALUES (b.pTS_MS),(b.pTS_SMS),(b.pTS_GSMS)) t(v)) AS pTS_sfc_raw,

      /* Za EXPer-model delte: TS per model */
      b.pTS_M, b.pTS_SM, b.pTS_GSM,
      b.pTS_MS, b.pTS_SMS, b.pTS_GSMS,

      /* Per-model H2H pWin (treba nam u EXP) */
      b.pH2H_M, b.pH2H_SM, b.pH2H_GSM,
      b.pH2H_MS, b.pH2H_SMS, b.pH2H_GSMS
    FROM base b
),
/* ======================================================
   CORE feature-i (kao u prethodnoj verziji)
   ====================================================== */
core AS (
    SELECT
      a.MatchTPId,

      COALESCE(a.pH2H_glb_raw, 0.5) AS pH2H_glb_01,
      COALESCE(a.pH2H_sfc_raw, 0.5) AS pH2H_sfc_01,
      COALESCE(a.pTS_glb_raw,  0.5) AS pTS_glb_01,
      COALESCE(a.pTS_sfc_raw,  0.5) AS pTS_sfc_01,

      -- Confidence (avg σ)
      CASE WHEN a.sigma_glb_avg IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_glb_avg >= p.SIGMA_CAP THEN 1 ELSE a.sigma_glb_avg / p.SIGMA_CAP END END AS H2H_glb_Confidence_01,
      CASE WHEN a.sigma_sfc_avg IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_sfc_avg >= p.SIGMA_CAP THEN 1 ELSE a.sigma_sfc_avg / p.SIGMA_CAP END END AS H2H_sfc_Confidence_01,

      -- Agreement (1 - range); ako <2 modela → 0.5
      CASE WHEN a.pH2H_glb_cnt >= 2 AND a.pH2H_glb_range IS NOT NULL THEN 1 - a.pH2H_glb_range ELSE 0.5 END AS H2H_glb_Agreement_01,
      CASE WHEN a.pH2H_sfc_cnt >= 2 AND a.pH2H_sfc_range IS NOT NULL THEN 1 - a.pH2H_sfc_range ELSE 0.5 END AS H2H_sfc_Agreement_01,

      -- Delta/AbsDelta
      ((COALESCE(a.pH2H_glb_raw,0.5) - COALESCE(a.pTS_glb_raw,0.5)) + 1.0)/2.0 AS H2H_glb_Delta_01,
      ABS(COALESCE(a.pH2H_glb_raw,0.5) - COALESCE(a.pTS_glb_raw,0.5))         AS H2H_glb_AbsDelta_01,
      ((COALESCE(a.pH2H_sfc_raw,0.5) - COALESCE(a.pTS_sfc_raw,0.5)) + 1.0)/2.0 AS H2H_sfc_Delta_01,
      ABS(COALESCE(a.pH2H_sfc_raw,0.5) - COALESCE(a.pTS_sfc_raw,0.5))          AS H2H_sfc_AbsDelta_01,

      -- Extreme + ConsensusStrength
      CASE WHEN a.pH2H_glb_raw IS NULL THEN 0 ELSE 2*ABS(a.pH2H_glb_raw - 0.5) END AS H2H_glb_Extreme_01,
      CASE WHEN a.pH2H_sfc_raw IS NULL THEN 0 ELSE 2*ABS(a.pH2H_sfc_raw - 0.5) END AS H2H_sfc_Extreme_01,

      (CASE WHEN a.sigma_glb_avg IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_glb_avg >= p.SIGMA_CAP THEN 1 ELSE a.sigma_glb_avg / p.SIGMA_CAP END END)
      * (CASE WHEN a.pH2H_glb_cnt >= 2 AND a.pH2H_glb_range IS NOT NULL THEN 1 - a.pH2H_glb_range ELSE 0.5 END) AS H2H_glb_ConsensusStrength_01,

      (CASE WHEN a.sigma_sfc_avg IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_sfc_avg >= p.SIGMA_CAP THEN 1 ELSE a.sigma_sfc_avg / p.SIGMA_CAP END END)
      * (CASE WHEN a.pH2H_sfc_cnt >= 2 AND a.pH2H_sfc_range IS NOT NULL THEN 1 - a.pH2H_sfc_range ELSE 0.5 END) AS H2H_sfc_ConsensusStrength_01,

      -- Blend (λ=confidence_avg)
      CASE WHEN a.sigma_glb_avg IS NULL THEN 0.5
           ELSE ((1 - CASE WHEN a.sigma_glb_avg >= p.SIGMA_CAP THEN 1 ELSE a.sigma_glb_avg / p.SIGMA_CAP END) * COALESCE(a.pH2H_glb_raw,0.5)
               + (    CASE WHEN a.sigma_glb_avg >= p.SIGMA_CAP THEN 1 ELSE a.sigma_glb_avg / p.SIGMA_CAP END) * COALESCE(a.pTS_glb_raw ,0.5)) END AS H2H_glb_pBlend_01,

      CASE WHEN a.sigma_sfc_avg IS NULL THEN 0.5
           ELSE ((1 - CASE WHEN a.sigma_sfc_avg >= p.SIGMA_CAP THEN 1 ELSE a.sigma_sfc_avg / p.SIGMA_CAP END) * COALESCE(a.pH2H_sfc_raw,0.5)
               + (    CASE WHEN a.sigma_sfc_avg >= p.SIGMA_CAP THEN 1 ELSE a.sigma_sfc_avg / p.SIGMA_CAP END) * COALESCE(a.pTS_sfc_raw ,0.5)) END AS H2H_sfc_pBlend_01,

      /* Flags */
      CASE WHEN a.pH2H_glb_cnt=0 THEN 1 ELSE 0 END AS H2H_glb_isMissing,
      CASE WHEN a.pH2H_sfc_cnt=0 THEN 1 ELSE 0 END AS H2H_sfc_isMissing

    FROM agg a
    CROSS JOIN param p
),
/* ======================================================
   EXPERIMENTAL: per-model pWin, confidence, delta, blend,
   plus disagreement metrics (range/stdev/model-count)
   ====================================================== */
exp AS (
    SELECT
      a.MatchTPId,

      -- ===== GLOBAL (M/SM/GSM) =====
      COALESCE(a.pH2H_M ,0.5) AS pH2H_M_01,
      COALESCE(a.pH2H_SM,0.5) AS pH2H_SM_01,
      COALESCE(a.pH2H_GSM,0.5) AS pH2H_GSM_01,

      -- per-model confidence
      CASE WHEN a.sigma_M  IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_M  >= p.SIGMA_CAP THEN 1 ELSE a.sigma_M  / p.SIGMA_CAP END END AS H2H_M_Confidence_01,
      CASE WHEN a.sigma_SM IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_SM >= p.SIGMA_CAP THEN 1 ELSE a.sigma_SM / p.SIGMA_CAP END END AS H2H_SM_Confidence_01,
      CASE WHEN a.sigma_GSM IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_GSM>= p.SIGMA_CAP THEN 1 ELSE a.sigma_GSM/ p.SIGMA_CAP END END AS H2H_GSM_Confidence_01,

      -- delte vs TS (po modelu)
      ((COALESCE(a.pH2H_M ,0.5) - COALESCE(a.pTS_M ,0.5)) + 1.0)/2.0  AS H2H_M_Delta_01,
      ((COALESCE(a.pH2H_SM,0.5) - COALESCE(a.pTS_SM,0.5)) + 1.0)/2.0  AS H2H_SM_Delta_01,
      ((COALESCE(a.pH2H_GSM,0.5) - COALESCE(a.pTS_GSM,0.5)) + 1.0)/2.0 AS H2H_GSM_Delta_01,

      ABS(COALESCE(a.pH2H_M ,0.5) - COALESCE(a.pTS_M ,0.5))  AS H2H_M_AbsDelta_01,
      ABS(COALESCE(a.pH2H_SM,0.5) - COALESCE(a.pTS_SM,0.5))  AS H2H_SM_AbsDelta_01,
      ABS(COALESCE(a.pH2H_GSM,0.5) - COALESCE(a.pTS_GSM,0.5)) AS H2H_GSM_AbsDelta_01,

      -- per-model blend
      CASE WHEN a.sigma_M  IS NULL THEN 0.5 ELSE ( (1 - CASE WHEN a.sigma_M  >= p.SIGMA_CAP THEN 1 ELSE a.sigma_M  / p.SIGMA_CAP END) * COALESCE(a.pH2H_M ,0.5)
                                                 + (     CASE WHEN a.sigma_M  >= p.SIGMA_CAP THEN 1 ELSE a.sigma_M  / p.SIGMA_CAP END) * COALESCE(a.pTS_M ,0.5) ) END AS H2H_M_pBlend_01,
      CASE WHEN a.sigma_SM IS NULL THEN 0.5 ELSE ( (1 - CASE WHEN a.sigma_SM >= p.SIGMA_CAP THEN 1 ELSE a.sigma_SM / p.SIGMA_CAP END) * COALESCE(a.pH2H_SM,0.5)
                                                 + (     CASE WHEN a.sigma_SM >= p.SIGMA_CAP THEN 1 ELSE a.sigma_SM / p.SIGMA_CAP END) * COALESCE(a.pTS_SM,0.5) ) END AS H2H_SM_pBlend_01,
      CASE WHEN a.sigma_GSM IS NULL THEN 0.5 ELSE ( (1 - CASE WHEN a.sigma_GSM>= p.SIGMA_CAP THEN 1 ELSE a.sigma_GSM/ p.SIGMA_CAP END) * COALESCE(a.pH2H_GSM,0.5)
                                                 + (     CASE WHEN a.sigma_GSM>= p.SIGMA_CAP THEN 1 ELSE a.sigma_GSM/ p.SIGMA_CAP END) * COALESCE(a.pTS_GSM,0.5) ) END AS H2H_GSM_pBlend_01,

      -- ekstremnost po modelu
      2*ABS(COALESCE(a.pH2H_M ,0.5) - 0.5)  AS H2H_M_Extreme_01,
      2*ABS(COALESCE(a.pH2H_SM,0.5) - 0.5)  AS H2H_SM_Extreme_01,
      2*ABS(COALESCE(a.pH2H_GSM,0.5) - 0.5) AS H2H_GSM_Extreme_01,

      -- disagreement metrike global
      COALESCE(a.pH2H_glb_range,0) AS H2H_glb_DisagreeRange_01,
      COALESCE(CASE WHEN a.pH2H_glb_stdev IS NULL THEN 0 ELSE CASE WHEN a.pH2H_glb_stdev*2 > 1 THEN 1 ELSE a.pH2H_glb_stdev*2 END END, 0) AS H2H_glb_DisagreeStdev_01,
      (CAST(a.pH2H_glb_cnt AS float) / 3.0) AS H2H_glb_ModelCount_01,

      -- ===== SURFACE (MS/SMS/GSMS) =====
      COALESCE(a.pH2H_MS ,0.5) AS pH2H_MS_01,
      COALESCE(a.pH2H_SMS,0.5) AS pH2H_SMS_01,
      COALESCE(a.pH2H_GSMS,0.5) AS pH2H_GSMS_01,

      CASE WHEN a.sigma_MS  IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_MS  >= p.SIGMA_CAP THEN 1 ELSE a.sigma_MS  / p.SIGMA_CAP END END AS H2H_MS_Confidence_01,
      CASE WHEN a.sigma_SMS IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_SMS >= p.SIGMA_CAP THEN 1 ELSE a.sigma_SMS / p.SIGMA_CAP END END AS H2H_SMS_Confidence_01,
      CASE WHEN a.sigma_GSMS IS NULL THEN 0 ELSE 1 - CASE WHEN a.sigma_GSMS>= p.SIGMA_CAP THEN 1 ELSE a.sigma_GSMS/ p.SIGMA_CAP END END AS H2H_GSMS_Confidence_01,

      ((COALESCE(a.pH2H_MS ,0.5) - COALESCE(a.pTS_MS ,0.5)) + 1.0)/2.0  AS H2H_MS_Delta_01,
      ((COALESCE(a.pH2H_SMS,0.5) - COALESCE(a.pTS_SMS,0.5)) + 1.0)/2.0  AS H2H_SMS_Delta_01,
      ((COALESCE(a.pH2H_GSMS,0.5) - COALESCE(a.pTS_GSMS,0.5)) + 1.0)/2.0 AS H2H_GSMS_Delta_01,

      ABS(COALESCE(a.pH2H_MS ,0.5) - COALESCE(a.pTS_MS ,0.5))  AS H2H_MS_AbsDelta_01,
      ABS(COALESCE(a.pH2H_SMS,0.5) - COALESCE(a.pTS_SMS,0.5))  AS H2H_SMS_AbsDelta_01,
      ABS(COALESCE(a.pH2H_GSMS,0.5) - COALESCE(a.pTS_GSMS,0.5)) AS H2H_GSMS_AbsDelta_01,

      CASE WHEN a.sigma_MS  IS NULL THEN 0.5 ELSE ( (1 - CASE WHEN a.sigma_MS  >= p.SIGMA_CAP THEN 1 ELSE a.sigma_MS  / p.SIGMA_CAP END) * COALESCE(a.pH2H_MS ,0.5)
                                                 + (     CASE WHEN a.sigma_MS  >= p.SIGMA_CAP THEN 1 ELSE a.sigma_MS  / p.SIGMA_CAP END) * COALESCE(a.pTS_MS ,0.5) ) END AS H2H_MS_pBlend_01,
      CASE WHEN a.sigma_SMS IS NULL THEN 0.5 ELSE ( (1 - CASE WHEN a.sigma_SMS >= p.SIGMA_CAP THEN 1 ELSE a.sigma_SMS / p.SIGMA_CAP END) * COALESCE(a.pH2H_SMS,0.5)
                                                 + (     CASE WHEN a.sigma_SMS >= p.SIGMA_CAP THEN 1 ELSE a.sigma_SMS / p.SIGMA_CAP END) * COALESCE(a.pTS_SMS,0.5) ) END AS H2H_SMS_pBlend_01,
      CASE WHEN a.sigma_GSMS IS NULL THEN 0.5 ELSE ( (1 - CASE WHEN a.sigma_GSMS>= p.SIGMA_CAP THEN 1 ELSE a.sigma_GSMS/ p.SIGMA_CAP END) * COALESCE(a.pH2H_GSMS,0.5)
                                                 + (     CASE WHEN a.sigma_GSMS>= p.SIGMA_CAP THEN 1 ELSE a.sigma_GSMS/ p.SIGMA_CAP END) * COALESCE(a.pTS_GSMS,0.5) ) END AS H2H_GSMS_pBlend_01,

      2*ABS(COALESCE(a.pH2H_MS ,0.5) - 0.5)  AS H2H_MS_Extreme_01,
      2*ABS(COALESCE(a.pH2H_SMS,0.5) - 0.5)  AS H2H_SMS_Extreme_01,
      2*ABS(COALESCE(a.pH2H_GSMS,0.5) - 0.5) AS H2H_GSMS_Extreme_01,

      COALESCE(a.pH2H_sfc_range,0) AS H2H_sfc_DisagreeRange_01,
      COALESCE(CASE WHEN a.pH2H_sfc_stdev IS NULL THEN 0 ELSE CASE WHEN a.pH2H_sfc_stdev*2 > 1 THEN 1 ELSE a.pH2H_sfc_stdev*2 END END, 0) AS H2H_sfc_DisagreeStdev_01,
      (CAST(a.pH2H_sfc_cnt AS float) / 3.0) AS H2H_sfc_ModelCount_01

    FROM agg a
    CROSS JOIN param p
)

/* ======================================================
   FINAL SELECT: CORE + FLAGS + EXP
   ====================================================== */
SELECT
  c.MatchTPId,

  /* ---------- CORE ---------- */
  c.pH2H_glb_01, c.H2H_glb_Confidence_01, c.H2H_glb_Agreement_01,
  c.H2H_glb_Delta_01, c.H2H_glb_AbsDelta_01,
  c.H2H_glb_Extreme_01, c.H2H_glb_ConsensusStrength_01,
  c.H2H_glb_pBlend_01,

  c.pH2H_sfc_01, c.H2H_sfc_Confidence_01, c.H2H_sfc_Agreement_01,
  c.H2H_sfc_Delta_01, c.H2H_sfc_AbsDelta_01,
  c.H2H_sfc_Extreme_01, c.H2H_sfc_ConsensusStrength_01,
  c.H2H_sfc_pBlend_01,

  /* ---------- FLAGS ---------- */
  c.H2H_glb_isMissing, c.H2H_sfc_isMissing,

  /* ---------- EXPERIMENTAL (bez diffova da ne eksplodira dimenzija) ---------- */
  -- Global per-model
  e.pH2H_M_01, e.pH2H_SM_01, e.pH2H_GSM_01,
  e.H2H_M_Confidence_01, e.H2H_SM_Confidence_01, e.H2H_GSM_Confidence_01,
  e.H2H_M_Delta_01, e.H2H_SM_Delta_01, e.H2H_GSM_Delta_01,
  e.H2H_M_AbsDelta_01, e.H2H_SM_AbsDelta_01, e.H2H_GSM_AbsDelta_01,
  e.H2H_M_pBlend_01, e.H2H_SM_pBlend_01, e.H2H_GSM_pBlend_01,
  e.H2H_M_Extreme_01, e.H2H_SM_Extreme_01, e.H2H_GSM_Extreme_01,
  e.H2H_glb_DisagreeRange_01, e.H2H_glb_DisagreeStdev_01, e.H2H_glb_ModelCount_01,

  -- Surface per-model (surface-pick)
  e.pH2H_MS_01, e.pH2H_SMS_01, e.pH2H_GSMS_01,
  e.H2H_MS_Confidence_01, e.H2H_SMS_Confidence_01, e.H2H_GSMS_Confidence_01,
  e.H2H_MS_Delta_01, e.H2H_SMS_Delta_01, e.H2H_GSMS_Delta_01,
  e.H2H_MS_AbsDelta_01, e.H2H_SMS_AbsDelta_01, e.H2H_GSMS_AbsDelta_01,
  e.H2H_MS_pBlend_01, e.H2H_SMS_pBlend_01, e.H2H_GSMS_pBlend_01,
  e.H2H_MS_Extreme_01, e.H2H_SMS_Extreme_01, e.H2H_GSMS_Extreme_01,
  e.H2H_sfc_DisagreeRange_01, e.H2H_sfc_DisagreeStdev_01, e.H2H_sfc_ModelCount_01

FROM core c
JOIN exp  e ON e.MatchTPId = c.MatchTPId;
GO
