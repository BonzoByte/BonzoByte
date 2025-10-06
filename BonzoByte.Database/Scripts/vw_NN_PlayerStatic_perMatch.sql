CREATE VIEW dbo.vw_NN_PlayerStatic_perMatch
AS
WITH scale AS (
    /* ZAMIJENI ove konstante kad “zamrzneš” vrijednosti iz TRAIN splita */
    SELECT
        CAST(16.0 AS float)  AS Age_min,
        CAST(40.0 AS float)  AS Age_p99,
        CAST(0.0  AS float)  AS YP_min,
        CAST(25.0 AS float)  AS YP_p99,

        CAST(150.0 AS float) AS H_min,
        CAST(205.0 AS float) AS H_p99,
        CAST(45.0  AS float) AS W_min,
        CAST(105.0 AS float) AS W_p99,

        CAST(17.0 AS float)  AS BMI_min,
        CAST(30.0 AS float)  AS BMI_p99,

        CAST(185.0 AS float) AS H_ATP_med,
        CAST(174.0 AS float) AS H_WTA_med,
        CAST(80.0  AS float) AS W_ATP_med,
        CAST(65.0  AS float) AS W_WTA_med
),
base AS (
    SELECT
        m.MatchTPId,
        m.DateTime        AS MatchDt,
        CASE WHEN te.TournamentTypeId = 1 THEN 1 ELSE 0 END AS Type_IsWTA,

        p1.PlayerTPId AS P1_Id,
        p1.PlayerBirthDate AS P1_BirthDate,
        p1.PlayerHeight    AS P1_HeightCm,
        p1.PlayerWeight    AS P1_WeightKg,
        p1.PlayerTurnedPro AS P1_TurnedProYear,
        p1.PlaysId         AS P1_PlaysId,

        p2.PlayerTPId AS P2_Id,
        p2.PlayerBirthDate AS P2_BirthDate,
        p2.PlayerHeight    AS P2_HeightCm,
        p2.PlayerWeight    AS P2_WeightKg,
        p2.PlayerTurnedPro AS P2_TurnedProYear,
        p2.PlaysId         AS P2_PlaysId
    FROM dbo.Match m join TournamentEvent te on m.TournamentEventTPId = te.TournamentEventTPId
    JOIN dbo.Player p1 ON p1.PlayerTPId = m.Player1TPId
    JOIN dbo.Player p2 ON p2.PlayerTPId = m.Player2TPId
),
/* === NOVI hands CTE baziran na Plays(PlaysId, PlaysName: 'right'/'left') === */
hands AS (
    SELECT
        b.MatchTPId,

        -- P1
        CASE 
            WHEN b.P1_PlaysId = 1 
                 OR LOWER(LTRIM(RTRIM(pl1.PlaysName))) = 'right' THEN 1 ELSE 0 
        END AS P1_Hand_R,
        CASE 
            WHEN b.P1_PlaysId = 2 
                 OR LOWER(LTRIM(RTRIM(pl1.PlaysName))) = 'left'  THEN 1 ELSE 0 
        END AS P1_Hand_L,
        CASE 
            WHEN b.P1_PlaysId IS NULL 
                 OR (
                       b.P1_PlaysId NOT IN (1,2) 
                       AND LOWER(LTRIM(RTRIM(pl1.PlaysName))) NOT IN ('right','left')
                    )
            THEN 1 ELSE 0
        END AS P1_Hand_isMissing,
        CASE 
            WHEN b.P1_PlaysId IS NULL 
                 OR (
                       b.P1_PlaysId NOT IN (1,2) 
                       AND LOWER(LTRIM(RTRIM(pl1.PlaysName))) NOT IN ('right','left')
                    )
            THEN 1 ELSE 0
        END AS P1_Hand_U,

        -- P2
        CASE 
            WHEN b.P2_PlaysId = 1 
                 OR LOWER(LTRIM(RTRIM(pl2.PlaysName))) = 'right' THEN 1 ELSE 0 
        END AS P2_Hand_R,
        CASE 
            WHEN b.P2_PlaysId = 2 
                 OR LOWER(LTRIM(RTRIM(pl2.PlaysName))) = 'left'  THEN 1 ELSE 0 
        END AS P2_Hand_L,
        CASE 
            WHEN b.P2_PlaysId IS NULL 
                 OR (
                       b.P2_PlaysId NOT IN (1,2) 
                       AND LOWER(LTRIM(RTRIM(pl2.PlaysName))) NOT IN ('right','left')
                    )
            THEN 1 ELSE 0
        END AS P2_Hand_isMissing,
        CASE 
            WHEN b.P2_PlaysId IS NULL 
                 OR (
                       b.P2_PlaysId NOT IN (1,2) 
                       AND LOWER(LTRIM(RTRIM(pl2.PlaysName))) NOT IN ('right','left')
                    )
            THEN 1 ELSE 0
        END AS P2_Hand_U
    FROM base b
    LEFT JOIN dbo.Plays pl1 ON pl1.PlaysId = b.P1_PlaysId
    LEFT JOIN dbo.Plays pl2 ON pl2.PlaysId = b.P2_PlaysId
),
feat AS (
    SELECT 
        b.*,
        CASE WHEN b.P1_BirthDate IS NOT NULL 
             THEN DATEDIFF(day, b.P1_BirthDate, b.MatchDt) / 365.2422 END AS P1_Age,
        CASE WHEN b.P2_BirthDate IS NOT NULL 
             THEN DATEDIFF(day, b.P2_BirthDate, b.MatchDt) / 365.2422 END AS P2_Age,
        CASE 
            WHEN b.P1_TurnedProYear IS NULL THEN NULL
            ELSE CASE 
                    WHEN DATEFROMPARTS(b.P1_TurnedProYear,7,1) > b.MatchDt THEN 0
                    ELSE DATEDIFF(year, DATEFROMPARTS(b.P1_TurnedProYear,7,1), b.MatchDt)
                 END
        END AS P1_YearsPro,
        CASE 
            WHEN b.P2_TurnedProYear IS NULL THEN NULL
            ELSE CASE 
                    WHEN DATEFROMPARTS(b.P2_TurnedProYear,7,1) > b.MatchDt THEN 0
                    ELSE DATEDIFF(year, DATEFROMPARTS(b.P2_TurnedProYear,7,1), b.MatchDt)
                 END
        END AS P2_YearsPro
    FROM base b
),
impute AS (
    SELECT 
        f.*,
        s.H_min, s.H_p99, s.W_min, s.W_p99, s.BMI_min, s.BMI_p99,
        s.H_ATP_med, s.H_WTA_med, s.W_ATP_med, s.W_WTA_med,
        s.Age_min, s.Age_p99, s.YP_min, s.YP_p99,

        CASE WHEN f.P1_HeightCm IS NULL THEN 1 ELSE 0 END AS P1_Height_isMissing,
        CASE WHEN f.P2_HeightCm IS NULL THEN 1 ELSE 0 END AS P2_Height_isMissing,
        CASE WHEN f.P1_WeightKg IS NULL THEN 1 ELSE 0 END AS P1_Weight_isMissing,
        CASE WHEN f.P2_WeightKg IS NULL THEN 1 ELSE 0 END AS P2_Weight_isMissing,

        COALESCE(f.P1_HeightCm, CASE WHEN f.Type_IsWTA=1 THEN s.H_WTA_med ELSE s.H_ATP_med END) AS P1_Height_imputed,
        COALESCE(f.P2_HeightCm, CASE WHEN f.Type_IsWTA=1 THEN s.H_WTA_med ELSE s.H_ATP_med END) AS P2_Height_imputed,
        COALESCE(f.P1_WeightKg, CASE WHEN f.Type_IsWTA=1 THEN s.W_WTA_med ELSE s.W_ATP_med END) AS P1_Weight_imputed,
        COALESCE(f.P2_WeightKg, CASE WHEN f.Type_IsWTA=1 THEN s.W_WTA_med ELSE s.W_ATP_med END) AS P2_Weight_imputed,

        CASE WHEN f.P1_HeightCm IS NOT NULL AND f.P1_WeightKg IS NOT NULL 
             THEN (f.P1_WeightKg / POWER(f.P1_HeightCm/100.0, 2)) END AS P1_BMI_raw,
        CASE WHEN f.P2_HeightCm IS NOT NULL AND f.P2_WeightKg IS NOT NULL 
             THEN (f.P2_WeightKg / POWER(f.P2_HeightCm/100.0, 2)) END AS P2_BMI_raw,

        (COALESCE(f.P1_WeightKg, CASE WHEN f.Type_IsWTA=1 THEN s.W_WTA_med ELSE s.W_ATP_med END)
         / POWER(COALESCE(f.P1_HeightCm, CASE WHEN f.Type_IsWTA=1 THEN s.H_WTA_med ELSE s.H_ATP_med END)/100.0, 2)
        ) AS P1_BMI_imputed,
        (COALESCE(f.P2_WeightKg, CASE WHEN f.Type_IsWTA=1 THEN s.W_WTA_med ELSE s.W_ATP_med END)
         / POWER(COALESCE(f.P2_HeightCm, CASE WHEN f.Type_IsWTA=1 THEN s.H_WTA_med ELSE s.H_ATP_med END)/100.0, 2)
        ) AS P2_BMI_imputed
    FROM feat f
    CROSS JOIN scale s
),
norm AS (
    SELECT
        i.MatchTPId,
        i.Type_IsWTA,

        CASE WHEN i.P1_Age IS NULL THEN 0
             WHEN i.P1_Age <= i.Age_min THEN 0
             WHEN i.P1_Age >= i.Age_p99 THEN 1
             ELSE (i.P1_Age - i.Age_min) / NULLIF(i.Age_p99 - i.Age_min,0) END AS P1_Age_01,
        CASE WHEN i.P2_Age IS NULL THEN 0
             WHEN i.P2_Age <= i.Age_min THEN 0
             WHEN i.P2_Age >= i.Age_p99 THEN 1
             ELSE (i.P2_Age - i.Age_min) / NULLIF(i.Age_p99 - i.Age_min,0) END AS P2_Age_01,
        CASE WHEN i.P1_Age IS NULL THEN 1 ELSE 0 END AS P1_Age_isMissing,
        CASE WHEN i.P2_Age IS NULL THEN 1 ELSE 0 END AS P2_Age_isMissing,

        CASE WHEN i.P1_YearsPro IS NULL THEN 0
             WHEN i.P1_YearsPro <= i.YP_min THEN 0
             WHEN i.P1_YearsPro >= i.YP_p99 THEN 1
             ELSE (i.P1_YearsPro - i.YP_min) / NULLIF(i.YP_p99 - i.YP_min,0) END AS P1_YearsPro_01,
        CASE WHEN i.P2_YearsPro IS NULL THEN 0
             WHEN i.P2_YearsPro <= i.YP_min THEN 0
             WHEN i.P2_YearsPro >= i.YP_p99 THEN 1
             ELSE (i.P2_YearsPro - i.YP_min) / NULLIF(i.YP_p99 - i.YP_min,0) END AS P2_YearsPro_01,
        CASE WHEN i.P1_YearsPro IS NULL THEN 1 ELSE 0 END AS P1_YearsPro_isMissing,
        CASE WHEN i.P2_YearsPro IS NULL THEN 1 ELSE 0 END AS P2_YearsPro_isMissing,

        CASE WHEN i.P1_Height_imputed <= i.H_min THEN 0
             WHEN i.P1_Height_imputed >= i.H_p99 THEN 1
             ELSE (i.P1_Height_imputed - i.H_min) / NULLIF(i.H_p99 - i.H_min,0) END AS P1_Height_01,
        CASE WHEN i.P2_Height_imputed <= i.H_min THEN 0
             WHEN i.P2_Height_imputed >= i.H_p99 THEN 1
             ELSE (i.P2_Height_imputed - i.H_min) / NULLIF(i.H_p99 - i.H_min,0) END AS P2_Height_01,
        i.P1_Height_isMissing,
        i.P2_Height_isMissing,

        CASE WHEN i.P1_Weight_imputed <= i.W_min THEN 0
             WHEN i.P1_Weight_imputed >= i.W_p99 THEN 1
             ELSE (i.P1_Weight_imputed - i.W_min) / NULLIF(i.W_p99 - i.W_min,0) END AS P1_Weight_01,
        CASE WHEN i.P2_Weight_imputed <= i.W_min THEN 0
             WHEN i.P2_Weight_imputed >= i.W_p99 THEN 1
             ELSE (i.P2_Weight_imputed - i.W_min) / NULLIF(i.W_p99 - i.W_min,0) END AS P2_Weight_01,
        i.P1_Weight_isMissing,
        i.P2_Weight_isMissing,

        CASE WHEN i.P1_BMI_raw IS NULL THEN 0
             WHEN i.P1_BMI_raw <= i.BMI_min THEN 0
             WHEN i.P1_BMI_raw >= i.BMI_p99 THEN 1
             ELSE (i.P1_BMI_raw - i.BMI_min) / NULLIF(i.BMI_p99 - i.BMI_min,0) END AS P1_BMI_01,
        CASE WHEN i.P2_BMI_raw IS NULL THEN 0
             WHEN i.P2_BMI_raw <= i.BMI_min THEN 0
             WHEN i.P2_BMI_raw >= i.BMI_p99 THEN 1
             ELSE (i.P2_BMI_raw - i.BMI_min) / NULLIF(i.BMI_p99 - i.BMI_min,0) END AS P2_BMI_01,
        CASE WHEN i.P1_BMI_raw IS NULL THEN 1 ELSE 0 END AS P1_BMI_isMissing,
        CASE WHEN i.P2_BMI_raw IS NULL THEN 1 ELSE 0 END AS P2_BMI_isMissing,

        CASE WHEN i.P1_BMI_imputed <= i.BMI_min THEN 0
             WHEN i.P1_BMI_imputed >= i.BMI_p99 THEN 1
             ELSE (i.P1_BMI_imputed - i.BMI_min) / NULLIF(i.BMI_p99 - i.BMI_min,0) END AS P1_BMI_imputed_01,
        CASE WHEN i.P2_BMI_imputed <= i.BMI_min THEN 0
             WHEN i.P2_BMI_imputed >= i.BMI_p99 THEN 1
             ELSE (i.P2_BMI_imputed - i.BMI_min) / NULLIF(i.BMI_p99 - i.BMI_min,0) END AS P2_BMI_imputed_01,

        CASE 
            WHEN i.P1_Age IS NULL OR i.P2_Age IS NULL THEN 0.5
            ELSE (((CASE WHEN i.P1_Age<=i.Age_min THEN 0 WHEN i.P1_Age>=i.Age_p99 THEN 1 ELSE (i.P1_Age-i.Age_min)/(i.Age_p99-i.Age_min) END)
                 -  (CASE WHEN i.P2_Age<=i.Age_min THEN 0 WHEN i.P2_Age>=i.Age_p99 THEN 1 ELSE (i.P2_Age-i.Age_min)/(i.Age_p99-i.Age_min) END)) + 1.0)/2.0
        END AS Age_Diff_01,

        CASE 
            WHEN i.P1_YearsPro IS NULL OR i.P2_YearsPro IS NULL THEN 0.5
            ELSE (((CASE WHEN i.P1_YearsPro<=i.YP_min THEN 0 WHEN i.P1_YearsPro>=i.YP_p99 THEN 1 ELSE (i.P1_YearsPro-i.YP_min)/(i.YP_p99-i.YP_min) END)
                 -  (CASE WHEN i.P2_YearsPro<=i.YP_min THEN 0 WHEN i.P2_YearsPro>=i.YP_p99 THEN 1 ELSE (i.P2_YearsPro-i.YP_min)/(i.YP_p99-i.YP_min) END)) + 1.0)/2.0
        END AS YearsPro_Diff_01,

        CASE 
            WHEN i.P1_Height_imputed IS NULL OR i.P2_Height_imputed IS NULL THEN 0.5
            ELSE (((CASE WHEN i.P1_Height_imputed<=i.H_min THEN 0 WHEN i.P1_Height_imputed>=i.H_p99 THEN 1 ELSE (i.P1_Height_imputed-i.H_min)/(i.H_p99-i.H_min) END)
                 -  (CASE WHEN i.P2_Height_imputed<=i.H_min THEN 0 WHEN i.P2_Height_imputed>=i.H_p99 THEN 1 ELSE (i.P2_Height_imputed-i.H_min)/(i.H_p99-i.H_min) END)) + 1.0)/2.0
        END AS Height_Diff_01,

        CASE 
            WHEN i.P1_Weight_imputed IS NULL OR i.P2_Weight_imputed IS NULL THEN 0.5
            ELSE (((CASE WHEN i.P1_Weight_imputed<=i.W_min THEN 0 WHEN i.P1_Weight_imputed>=i.W_p99 THEN 1 ELSE (i.P1_Weight_imputed-i.W_min)/(i.W_p99-i.W_min) END)
                 -  (CASE WHEN i.P2_Weight_imputed<=i.W_min THEN 0 WHEN i.P2_Weight_imputed>=i.W_p99 THEN 1 ELSE (i.P2_Weight_imputed-i.W_min)/(i.W_p99-i.W_min) END)) + 1.0)/2.0
        END AS Weight_Diff_01,

        CASE 
            WHEN i.P1_BMI_raw IS NULL OR i.P2_BMI_raw IS NULL THEN 0.5
            ELSE (((CASE WHEN i.P1_BMI_raw<=i.BMI_min THEN 0 WHEN i.P1_BMI_raw>=i.BMI_p99 THEN 1 ELSE (i.P1_BMI_raw-i.BMI_min)/(i.BMI_p99-i.BMI_min) END)
                 -  (CASE WHEN i.P2_BMI_raw<=i.BMI_min THEN 0 WHEN i.P2_BMI_raw>=i.BMI_p99 THEN 1 ELSE (i.P2_BMI_raw-i.BMI_min)/(i.BMI_p99-i.BMI_min) END)) + 1.0)/2.0
        END AS BMI_Diff_01
    FROM impute i
)
SELECT 
    n.MatchTPId,
    n.Type_IsWTA,

    n.P1_Age_01,  n.P2_Age_01,  n.P1_Age_isMissing,  n.P2_Age_isMissing,
    n.P1_YearsPro_01, n.P2_YearsPro_01, n.P1_YearsPro_isMissing, n.P2_YearsPro_isMissing,

    n.P1_Height_01, n.P2_Height_01, n.P1_Height_isMissing, n.P2_Height_isMissing,
    n.P1_Weight_01, n.P2_Weight_01, n.P1_Weight_isMissing, n.P2_Weight_isMissing,
    n.P1_BMI_01,    n.P2_BMI_01,    n.P1_BMI_isMissing,    n.P2_BMI_isMissing,
    n.P1_BMI_imputed_01, n.P2_BMI_imputed_01,

    n.Age_Diff_01, n.YearsPro_Diff_01, n.Height_Diff_01, n.Weight_Diff_01, n.BMI_Diff_01,

    h.P1_Hand_R, h.P1_Hand_L, h.P1_Hand_U, h.P1_Hand_isMissing,
    h.P2_Hand_R, h.P2_Hand_L, h.P2_Hand_U, h.P2_Hand_isMissing,

    CASE 
        WHEN h.P1_Hand_isMissing=1 OR h.P2_Hand_isMissing=1 THEN 0.5
        WHEN (h.P1_Hand_R=1 AND h.P2_Hand_R=1) OR (h.P1_Hand_L=1 AND h.P2_Hand_L=1) THEN 1.0
        ELSE 0.0
    END AS Hand_Same01

FROM norm n
JOIN hands h ON h.MatchTPId = n.MatchTPId;
GO
