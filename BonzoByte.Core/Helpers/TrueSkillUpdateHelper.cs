using BonzoByte.Core.Models;
using BonzoByte.Core.Models.TrueSkill;

namespace BonzoByte.Core.Helpers
{
    /// <summary>
    /// TrueSkill batch helper (bez refleksije).
    /// - Global TS (M/SM/GSM) računa iz pre-match snapshotova spremljenih na Match objektu.
    /// - Per-surface TS (MS*/SMS*/GSMS*) računa iz pre-match snapshotova na Player objektima.
    /// - Po defaultu SM/GSM koriste SD od M (granulacija → prenizak SD, tvoja praksa).
    /// - Opcionalno možeš forsirati i da per-surface M koristi globalni M-SD (ako želiš replicirati staro ponašanje).
    /// </summary>
    public static class TrueSkillUpdateHelper
    {
        private const double DEFAULT_MEAN = 25.0;
        private const double DEFAULT_SD = 8.333333333;

        // === 1) GLOBAL: koristi PRE-MATCH vrijednosti s Match-a, vraća posteriore u Match ===
        public static void ComputeAndApplyGlobalFromMatchSnapshots(Match match)
        {
            if (match == null || match.Player1TPId == null || match.Player2TPId == null)
                return;

            int p1 = match.Player1TPId.Value;
            int p2 = match.Player2TPId.Value;
            string details = match.ResultDetails ?? "";

            var tsM = TrueSkillHelper.CalculateM(
                match.Player1TrueSkillMeanM ?? DEFAULT_MEAN,
                match.Player2TrueSkillMeanM ?? DEFAULT_MEAN,
                match.Player1TrueSkillStandardDeviationM ?? DEFAULT_SD,
                match.Player2TrueSkillStandardDeviationM ?? DEFAULT_SD
            );

            var tsSM = TrueSkillHelper.CalculateSM(
                match.Player1TrueSkillMeanSM ?? DEFAULT_MEAN,
                match.Player2TrueSkillMeanSM ?? DEFAULT_MEAN,
                match.Player1TrueSkillStandardDeviationSM ?? DEFAULT_SD,
                match.Player2TrueSkillStandardDeviationSM ?? DEFAULT_SD,
                p1, p2, details
            );

            var tsGSM = TrueSkillHelper.CalculateGSM(
                match.Player1TrueSkillMeanGSM ?? DEFAULT_MEAN,
                match.Player2TrueSkillMeanGSM ?? DEFAULT_MEAN,
                match.Player1TrueSkillStandardDeviationGSM ?? DEFAULT_SD,
                match.Player2TrueSkillStandardDeviationGSM ?? DEFAULT_SD,
                p1, p2, details
            );

            // upiši posteriore (GLOBAL)
            match.Player1TrueSkillMeanM = tsM.MeanP1; match.Player1TrueSkillStandardDeviationM = tsM.SdP1; match.WinProbabilityPlayer1M = tsM.WinProbabilityP1;
            match.Player2TrueSkillMeanM = tsM.MeanP2; match.Player2TrueSkillStandardDeviationM = tsM.SdP2;

            match.Player1TrueSkillMeanSM = tsSM.MeanP1; match.Player1TrueSkillStandardDeviationSM = tsSM.SdP1; match.WinProbabilityPlayer1SM = tsSM.WinProbabilityP1;
            match.Player2TrueSkillMeanSM = tsSM.MeanP2; match.Player2TrueSkillStandardDeviationSM = tsSM.SdP2;

            match.Player1TrueSkillMeanGSM = tsGSM.MeanP1; match.Player1TrueSkillStandardDeviationGSM = tsGSM.SdP1; match.WinProbabilityPlayer1GSM = tsGSM.WinProbabilityP1;
            match.Player2TrueSkillMeanGSM = tsGSM.MeanP2; match.Player2TrueSkillStandardDeviationGSM = tsGSM.SdP2;
        }

        // === 2) SURFACE: koristi PRE-MATCH vrijednosti s Player-a, vraća posteriore u Match za AKTIVNU podlogu ===
        public static void ComputeAndApplySurfaceFromPlayers(
            Match match,
            Player player1,
            Player player2,
            bool useMsdForSmGsm = true,          // tvoj default: SM/GSM uzimaju M-SD (global/surface – vidi niže)
            bool useGlobalMsdForSurfaceM = false // ako želiš da i MS* koristi globalni M-SD umjesto surface M-SD
        )
        {
            if (match == null || match.Player1TPId == null || match.Player2TPId == null || match.SurfaceId == null)
                return;

            int p1Id = match.Player1TPId.Value;
            int p2Id = match.Player2TPId.Value;
            int s = (int)match.SurfaceId;
            string details = match.ResultDetails ?? "";

            // dohvat pre-match mean/SD iz Player-a
            (double m1, double s1, double m2, double s2) = GetMInputs(player1, player2, s, useGlobalMsdForSurfaceM);
            (double sm1, double ssm1, double sm2, double ssm2) = GetSMInputs(player1, player2, s, useMsdForSmGsm);
            (double gm1, double sgm1, double gm2, double sgm2) = GetGSMInputs(player1, player2, s, useMsdForSmGsm);

            // izračun
            var tsMS = TrueSkillHelper.CalculateM(m1, m2, s1, s2);
            var tsSMS = TrueSkillHelper.CalculateSM(sm1, sm2, ssm1, ssm2, p1Id, p2Id, details);
            var tsGS = TrueSkillHelper.CalculateGSM(gm1, gm2, sgm1, sgm2, p1Id, p2Id, details);

            // upis u Match (per-surface)
            ApplySurfacePosterior(match, s, "M", tsMS);
            ApplySurfacePosterior(match, s, "SM", tsSMS);
            ApplySurfacePosterior(match, s, "GSM", tsGS);
        }

        // === 3) Player <= Match (push back posteriore nakon meča) ===
        public static void PushPosteriorToPlayers(Match match, Player p1, Player p2)
        {
            // GLOBAL
            p1.TrueSkillMeanM = match.Player1TrueSkillMeanM;
            p1.TrueSkillStandardDeviationM = match.Player1TrueSkillStandardDeviationM;
            p1.TrueSkillMeanSM = match.Player1TrueSkillMeanSM;
            p1.TrueSkillStandardDeviationSM = match.Player1TrueSkillStandardDeviationSM;
            p1.TrueSkillMeanGSM = match.Player1TrueSkillMeanGSM;
            p1.TrueSkillStandardDeviationGSM = match.Player1TrueSkillStandardDeviationGSM;

            p2.TrueSkillMeanM = match.Player2TrueSkillMeanM;
            p2.TrueSkillStandardDeviationM = match.Player2TrueSkillStandardDeviationM;
            p2.TrueSkillMeanSM = match.Player2TrueSkillMeanSM;
            p2.TrueSkillStandardDeviationSM = match.Player2TrueSkillStandardDeviationSM;
            p2.TrueSkillMeanGSM = match.Player2TrueSkillMeanGSM;
            p2.TrueSkillStandardDeviationGSM = match.Player2TrueSkillStandardDeviationGSM;

            // SURFACE (kopiramo samo aktivnu podlogu; po želji možeš kopirati sve ako ih računaš unaprijed)
            int s = match.SurfaceId ?? 0;
            switch (s)
            {
                case 1:
                    p1.TrueSkillMeanMS1 = match.Player1TrueSkillMeanMS1;
                    p1.TrueSkillStandardDeviationMS1 = match.Player1TrueSkillStandardDeviationMS1;
                    p1.TrueSkillMeanSMS1 = match.Player1TrueSkillMeanSMS1;
                    p1.TrueSkillStandardDeviationSMS1 = match.Player1TrueSkillStandardDeviationSMS1;
                    p1.TrueSkillMeanGSMS1 = match.Player1TrueSkillMeanGSMS1;
                    p1.TrueSkillStandardDeviationGSMS1 = match.Player1TrueSkillStandardDeviationGSMS1;

                    p2.TrueSkillMeanMS1 = match.Player2TrueSkillMeanMS1;
                    p2.TrueSkillStandardDeviationMS1 = match.Player2TrueSkillStandardDeviationMS1;
                    p2.TrueSkillMeanSMS1 = match.Player2TrueSkillMeanSMS1;
                    p2.TrueSkillStandardDeviationSMS1 = match.Player2TrueSkillStandardDeviationSMS1;
                    p2.TrueSkillMeanGSMS1 = match.Player2TrueSkillMeanGSMS1;
                    p2.TrueSkillStandardDeviationGSMS1 = match.Player2TrueSkillStandardDeviationGSMS1;
                    break;

                case 2:
                    p1.TrueSkillMeanMS2 = match.Player1TrueSkillMeanMS2;
                    p1.TrueSkillStandardDeviationMS2 = match.Player1TrueSkillStandardDeviationMS2;
                    p1.TrueSkillMeanSMS2 = match.Player1TrueSkillMeanSMS2;
                    p1.TrueSkillStandardDeviationSMS2 = match.Player1TrueSkillStandardDeviationSMS2;
                    p1.TrueSkillMeanGSMS2 = match.Player1TrueSkillMeanGSMS2;
                    p1.TrueSkillStandardDeviationGSMS2 = match.Player1TrueSkillStandardDeviationGSMS2;

                    p2.TrueSkillMeanMS2 = match.Player2TrueSkillMeanMS2;
                    p2.TrueSkillStandardDeviationMS2 = match.Player2TrueSkillStandardDeviationMS2;
                    p2.TrueSkillMeanSMS2 = match.Player2TrueSkillMeanSMS2;
                    p2.TrueSkillStandardDeviationSMS2 = match.Player2TrueSkillStandardDeviationSMS2;
                    p2.TrueSkillMeanGSMS2 = match.Player2TrueSkillMeanGSMS2;
                    p2.TrueSkillStandardDeviationGSMS2 = match.Player2TrueSkillStandardDeviationGSMS2;
                    break;

                case 3:
                    p1.TrueSkillMeanMS3 = match.Player1TrueSkillMeanMS3;
                    p1.TrueSkillStandardDeviationMS3 = match.Player1TrueSkillStandardDeviationMS3;
                    p1.TrueSkillMeanSMS3 = match.Player1TrueSkillMeanSMS3;
                    p1.TrueSkillStandardDeviationSMS3 = match.Player1TrueSkillStandardDeviationSMS3;
                    p1.TrueSkillMeanGSMS3 = match.Player1TrueSkillMeanGSMS3;
                    p1.TrueSkillStandardDeviationGSMS3 = match.Player1TrueSkillStandardDeviationGSMS3;

                    p2.TrueSkillMeanMS3 = match.Player2TrueSkillMeanMS3;
                    p2.TrueSkillStandardDeviationMS3 = match.Player2TrueSkillStandardDeviationMS3;
                    p2.TrueSkillMeanSMS3 = match.Player2TrueSkillMeanSMS3;
                    p2.TrueSkillStandardDeviationSMS3 = match.Player2TrueSkillStandardDeviationSMS3;
                    p2.TrueSkillMeanGSMS3 = match.Player2TrueSkillMeanGSMS3;
                    p2.TrueSkillStandardDeviationGSMS3 = match.Player2TrueSkillStandardDeviationGSMS3;
                    break;

                case 4:
                    p1.TrueSkillMeanMS4 = match.Player1TrueSkillMeanMS4;
                    p1.TrueSkillStandardDeviationMS4 = match.Player1TrueSkillStandardDeviationMS4;
                    p1.TrueSkillMeanSMS4 = match.Player1TrueSkillMeanSMS4;
                    p1.TrueSkillStandardDeviationSMS4 = match.Player1TrueSkillStandardDeviationSMS4;
                    p1.TrueSkillMeanGSMS4 = match.Player1TrueSkillMeanGSMS4;
                    p1.TrueSkillStandardDeviationGSMS4 = match.Player1TrueSkillStandardDeviationGSMS4;

                    p2.TrueSkillMeanMS4 = match.Player2TrueSkillMeanMS4;
                    p2.TrueSkillStandardDeviationMS4 = match.Player2TrueSkillStandardDeviationMS4;
                    p2.TrueSkillMeanSMS4 = match.Player2TrueSkillMeanSMS4;
                    p2.TrueSkillStandardDeviationSMS4 = match.Player2TrueSkillStandardDeviationSMS4;
                    p2.TrueSkillMeanGSMS4 = match.Player2TrueSkillMeanGSMS4;
                    p2.TrueSkillStandardDeviationGSMS4 = match.Player2TrueSkillStandardDeviationGSMS4;
                    break;
            }
        }

        // ----------------- privatni helperi -----------------

        private static (double m1, double s1, double m2, double s2)
            GetMInputs(Player p1, Player p2, int s, bool useGlobalMsdForSurfaceM)
        {
            // mean: po podlozi (MS*), SD: po podlozi ili globalni M-SD (ovisno o flagu)
            double p1Msd = useGlobalMsdForSurfaceM
                ? (p1.TrueSkillStandardDeviationM ?? DEFAULT_SD)
                : s switch
                {
                    1 => p1.TrueSkillStandardDeviationMS1 ?? DEFAULT_SD,
                    2 => p1.TrueSkillStandardDeviationMS2 ?? DEFAULT_SD,
                    3 => p1.TrueSkillStandardDeviationMS3 ?? DEFAULT_SD,
                    4 => p1.TrueSkillStandardDeviationMS4 ?? DEFAULT_SD,
                    _ => DEFAULT_SD
                };
            double p2Msd = useGlobalMsdForSurfaceM
                ? (p2.TrueSkillStandardDeviationM ?? DEFAULT_SD)
                : s switch
                {
                    1 => p2.TrueSkillStandardDeviationMS1 ?? DEFAULT_SD,
                    2 => p2.TrueSkillStandardDeviationMS2 ?? DEFAULT_SD,
                    3 => p2.TrueSkillStandardDeviationMS3 ?? DEFAULT_SD,
                    4 => p2.TrueSkillStandardDeviationMS4 ?? DEFAULT_SD,
                    _ => DEFAULT_SD
                };

            double p1Mean = s switch
            {
                1 => p1.TrueSkillMeanMS1 ?? DEFAULT_MEAN,
                2 => p1.TrueSkillMeanMS2 ?? DEFAULT_MEAN,
                3 => p1.TrueSkillMeanMS3 ?? DEFAULT_MEAN,
                4 => p1.TrueSkillMeanMS4 ?? DEFAULT_MEAN,
                _ => DEFAULT_MEAN
            };
            double p2Mean = s switch
            {
                1 => p2.TrueSkillMeanMS1 ?? DEFAULT_MEAN,
                2 => p2.TrueSkillMeanMS2 ?? DEFAULT_MEAN,
                3 => p2.TrueSkillMeanMS3 ?? DEFAULT_MEAN,
                4 => p2.TrueSkillMeanMS4 ?? DEFAULT_MEAN,
                _ => DEFAULT_MEAN
            };

            return (p1Mean, p1Msd, p2Mean, p2Msd);
        }

        private static (double m1, double s1, double m2, double s2)
            GetSMInputs(Player p1, Player p2, int s, bool useMsdForSmGsm)
        {
            // mean: SMS*, SD: M (global/surface?) — tvoja praksa: SD od M
            double p1Sd = useMsdForSmGsm
                ? (p1.TrueSkillStandardDeviationM ?? DEFAULT_SD)
                : s switch
                {
                    1 => p1.TrueSkillStandardDeviationSMS1 ?? DEFAULT_SD,
                    2 => p1.TrueSkillStandardDeviationSMS2 ?? DEFAULT_SD,
                    3 => p1.TrueSkillStandardDeviationSMS3 ?? DEFAULT_SD,
                    4 => p1.TrueSkillStandardDeviationSMS4 ?? DEFAULT_SD,
                    _ => DEFAULT_SD
                };
            double p2Sd = useMsdForSmGsm
                ? (p2.TrueSkillStandardDeviationM ?? DEFAULT_SD)
                : s switch
                {
                    1 => p2.TrueSkillStandardDeviationSMS1 ?? DEFAULT_SD,
                    2 => p2.TrueSkillStandardDeviationSMS2 ?? DEFAULT_SD,
                    3 => p2.TrueSkillStandardDeviationSMS3 ?? DEFAULT_SD,
                    4 => p2.TrueSkillStandardDeviationSMS4 ?? DEFAULT_SD,
                    _ => DEFAULT_SD
                };

            double p1Mean = s switch
            {
                1 => p1.TrueSkillMeanSMS1 ?? DEFAULT_MEAN,
                2 => p1.TrueSkillMeanSMS2 ?? DEFAULT_MEAN,
                3 => p1.TrueSkillMeanSMS3 ?? DEFAULT_MEAN,
                4 => p1.TrueSkillMeanSMS4 ?? DEFAULT_MEAN,
                _ => DEFAULT_MEAN
            };
            double p2Mean = s switch
            {
                1 => p2.TrueSkillMeanSMS1 ?? DEFAULT_MEAN,
                2 => p2.TrueSkillMeanSMS2 ?? DEFAULT_MEAN,
                3 => p2.TrueSkillMeanSMS3 ?? DEFAULT_MEAN,
                4 => p2.TrueSkillMeanSMS4 ?? DEFAULT_MEAN,
                _ => DEFAULT_MEAN
            };

            return (p1Mean, p1Sd, p2Mean, p2Sd);
        }

        private static (double m1, double s1, double m2, double s2)
            GetGSMInputs(Player p1, Player p2, int s, bool useMsdForSmGsm)
        {
            // mean: GSMS*, SD: M (tvoja praksa)
            double p1Sd = useMsdForSmGsm
                ? (p1.TrueSkillStandardDeviationM ?? DEFAULT_SD)
                : s switch
                {
                    1 => p1.TrueSkillStandardDeviationGSMS1 ?? DEFAULT_SD,
                    2 => p1.TrueSkillStandardDeviationGSMS2 ?? DEFAULT_SD,
                    3 => p1.TrueSkillStandardDeviationGSMS3 ?? DEFAULT_SD,
                    4 => p1.TrueSkillStandardDeviationGSMS4 ?? DEFAULT_SD,
                    _ => DEFAULT_SD
                };
            double p2Sd = useMsdForSmGsm
                ? (p2.TrueSkillStandardDeviationM ?? DEFAULT_SD)
                : s switch
                {
                    1 => p2.TrueSkillStandardDeviationGSMS1 ?? DEFAULT_SD,
                    2 => p2.TrueSkillStandardDeviationGSMS2 ?? DEFAULT_SD,
                    3 => p2.TrueSkillStandardDeviationGSMS3 ?? DEFAULT_SD,
                    4 => p2.TrueSkillStandardDeviationGSMS4 ?? DEFAULT_SD,
                    _ => DEFAULT_SD
                };

            double p1Mean = s switch
            {
                1 => p1.TrueSkillMeanGSMS1 ?? DEFAULT_MEAN,
                2 => p1.TrueSkillMeanGSMS2 ?? DEFAULT_MEAN,
                3 => p1.TrueSkillMeanGSMS3 ?? DEFAULT_MEAN,
                4 => p1.TrueSkillMeanGSMS4 ?? DEFAULT_MEAN,
                _ => DEFAULT_MEAN
            };
            double p2Mean = s switch
            {
                1 => p2.TrueSkillMeanGSMS1 ?? DEFAULT_MEAN,
                2 => p2.TrueSkillMeanGSMS2 ?? DEFAULT_MEAN,
                3 => p2.TrueSkillMeanGSMS3 ?? DEFAULT_MEAN,
                4 => p2.TrueSkillMeanGSMS4 ?? DEFAULT_MEAN,
                _ => DEFAULT_MEAN
            };

            return (p1Mean, p1Sd, p2Mean, p2Sd);
        }

        private static void ApplySurfacePosterior(Match m, int s, string model, TrueSkillResult ts)
        {
            if (s == 1 && model == "M") { m.Player1TrueSkillMeanMS1 = ts.MeanP1; m.Player1TrueSkillStandardDeviationMS1 = ts.SdP1; m.Player2TrueSkillMeanMS1 = ts.MeanP2; m.Player2TrueSkillStandardDeviationMS1 = ts.SdP2; m.WinProbabilityPlayer1MS1 = ts.WinProbabilityP1; }
            if (s == 1 && model == "SM") { m.Player1TrueSkillMeanSMS1 = ts.MeanP1; m.Player1TrueSkillStandardDeviationSMS1 = ts.SdP1; m.Player2TrueSkillMeanSMS1 = ts.MeanP2; m.Player2TrueSkillStandardDeviationSMS1 = ts.SdP2; m.WinProbabilityPlayer1SMS1 = ts.WinProbabilityP1; }
            if (s == 1 && model == "GSM") { m.Player1TrueSkillMeanGSMS1 = ts.MeanP1; m.Player1TrueSkillStandardDeviationGSMS1 = ts.SdP1; m.Player2TrueSkillMeanGSMS1 = ts.MeanP2; m.Player2TrueSkillStandardDeviationGSMS1 = ts.SdP2; m.WinProbabilityPlayer1GSMS1 = ts.WinProbabilityP1; }

            if (s == 2 && model == "M") { m.Player1TrueSkillMeanMS2 = ts.MeanP1; m.Player1TrueSkillStandardDeviationMS2 = ts.SdP1; m.Player2TrueSkillMeanMS2 = ts.MeanP2; m.Player2TrueSkillStandardDeviationMS2 = ts.SdP2; m.WinProbabilityPlayer1MS2 = ts.WinProbabilityP1; }
            if (s == 2 && model == "SM") { m.Player1TrueSkillMeanSMS2 = ts.MeanP1; m.Player1TrueSkillStandardDeviationSMS2 = ts.SdP1; m.Player2TrueSkillMeanSMS2 = ts.MeanP2; m.Player2TrueSkillStandardDeviationSMS2 = ts.SdP2; m.WinProbabilityPlayer1SMS2 = ts.WinProbabilityP1; }
            if (s == 2 && model == "GSM") { m.Player1TrueSkillMeanGSMS2 = ts.MeanP1; m.Player1TrueSkillStandardDeviationGSMS2 = ts.SdP1; m.Player2TrueSkillMeanGSMS2 = ts.MeanP2; m.Player2TrueSkillStandardDeviationGSMS2 = ts.SdP2; m.WinProbabilityPlayer1GSMS2 = ts.WinProbabilityP1; }

            if (s == 3 && model == "M") { m.Player1TrueSkillMeanMS3 = ts.MeanP1; m.Player1TrueSkillStandardDeviationMS3 = ts.SdP1; m.Player2TrueSkillMeanMS3 = ts.MeanP2; m.Player2TrueSkillStandardDeviationMS3 = ts.SdP2; m.WinProbabilityPlayer1MS3 = ts.WinProbabilityP1; }
            if (s == 3 && model == "SM") { m.Player1TrueSkillMeanSMS3 = ts.MeanP1; m.Player1TrueSkillStandardDeviationSMS3 = ts.SdP1; m.Player2TrueSkillMeanSMS3 = ts.MeanP2; m.Player2TrueSkillStandardDeviationSMS3 = ts.SdP2; m.WinProbabilityPlayer1SMS3 = ts.WinProbabilityP1; }
            if (s == 3 && model == "GSM") { m.Player1TrueSkillMeanGSMS3 = ts.MeanP1; m.Player1TrueSkillStandardDeviationGSMS3 = ts.SdP1; m.Player2TrueSkillMeanGSMS3 = ts.MeanP2; m.Player2TrueSkillStandardDeviationGSMS3 = ts.SdP2; m.WinProbabilityPlayer1GSMS3 = ts.WinProbabilityP1; }

            if (s == 4 && model == "M") { m.Player1TrueSkillMeanMS4 = ts.MeanP1; m.Player1TrueSkillStandardDeviationMS4 = ts.SdP1; m.Player2TrueSkillMeanMS4 = ts.MeanP2; m.Player2TrueSkillStandardDeviationMS4 = ts.SdP2; m.WinProbabilityPlayer1MS4 = ts.WinProbabilityP1; }
            if (s == 4 && model == "SM") { m.Player1TrueSkillMeanSMS4 = ts.MeanP1; m.Player1TrueSkillStandardDeviationSMS4 = ts.SdP1; m.Player2TrueSkillMeanSMS4 = ts.MeanP2; m.Player2TrueSkillStandardDeviationSMS4 = ts.SdP2; m.WinProbabilityPlayer1SMS4 = ts.WinProbabilityP1; }
            if (s == 4 && model == "GSM") { m.Player1TrueSkillMeanGSMS4 = ts.MeanP1; m.Player1TrueSkillStandardDeviationGSMS4 = ts.SdP1; m.Player2TrueSkillMeanGSMS4 = ts.MeanP2; m.Player2TrueSkillStandardDeviationGSMS4 = ts.SdP2; m.WinProbabilityPlayer1GSMS4 = ts.WinProbabilityP1; }
        }

        public static void BackfillMissingSurfaceWinProbabilities(
    Match match,
    Player p1,
    Player p2,
    bool useMsdForSmGsm = true,
    bool useGlobalMsdForSurfaceM = false)
        {
            if (match?.Player1TPId == null || match.Player2TPId == null) return;

            for (int s = 1; s <= 4; s++)
            {
                // M (per-surface)
                if (GetSurfaceWp(match, s, "M") is null)
                {
                    var (m1, sd1, m2, sd2) = GetMInputs(p1, p2, s, useGlobalMsdForSurfaceM);
                    var wp = SafeWp(TrueSkillHelper.CalculateWinProbability(m1, m2, sd1, sd2));
                    SetSurfaceWp(match, s, "M", wp);
                }

                // SM (per-surface; SD policy: po defaultu koristiš M-SD)
                if (GetSurfaceWp(match, s, "SM") is null)
                {
                    var (sm1, ssd1, sm2, ssd2) = GetSMInputs(p1, p2, s, useMsdForSmGsm);
                    var wp = SafeWp(TrueSkillHelper.CalculateWinProbability(sm1, sm2, ssd1, ssd2));
                    SetSurfaceWp(match, s, "SM", wp);
                }

                // GSM (per-surface; SD policy: po defaultu koristiš M-SD)
                if (GetSurfaceWp(match, s, "GSM") is null)
                {
                    var (gm1, gsd1, gm2, gsd2) = GetGSMInputs(p1, p2, s, useMsdForSmGsm);
                    var wp = SafeWp(TrueSkillHelper.CalculateWinProbability(gm1, gm2, gsd1, gsd2));
                    SetSurfaceWp(match, s, "GSM", wp);
                }
            }

            static double SafeWp(double x) =>
                (double.IsNaN(x) || double.IsInfinity(x)) ? 0.5 : Math.Max(0, Math.Min(1, x));
        }

        // --- mali helperi za čitanje/upis WP-a po podlozi/modelu ---

        private static double? GetSurfaceWp(Match m, int s, string model) => (s, model) switch
        {
            (1, "M") => m.WinProbabilityPlayer1MS1,
            (1, "SM") => m.WinProbabilityPlayer1SMS1,
            (1, "GSM") => m.WinProbabilityPlayer1GSMS1,

            (2, "M") => m.WinProbabilityPlayer1MS2,
            (2, "SM") => m.WinProbabilityPlayer1SMS2,
            (2, "GSM") => m.WinProbabilityPlayer1GSMS2,

            (3, "M") => m.WinProbabilityPlayer1MS3,
            (3, "SM") => m.WinProbabilityPlayer1SMS3,
            (3, "GSM") => m.WinProbabilityPlayer1GSMS3,

            (4, "M") => m.WinProbabilityPlayer1MS4,
            (4, "SM") => m.WinProbabilityPlayer1SMS4,
            (4, "GSM") => m.WinProbabilityPlayer1GSMS4,
            _ => null
        };

        private static void SetSurfaceWp(Match m, int s, string model, double wp)
        {
            switch ((s, model))
            {
                case (1, "M"): m.WinProbabilityPlayer1MS1 = wp; break;
                case (1, "SM"): m.WinProbabilityPlayer1SMS1 = wp; break;
                case (1, "GSM"): m.WinProbabilityPlayer1GSMS1 = wp; break;

                case (2, "M"): m.WinProbabilityPlayer1MS2 = wp; break;
                case (2, "SM"): m.WinProbabilityPlayer1SMS2 = wp; break;
                case (2, "GSM"): m.WinProbabilityPlayer1GSMS2 = wp; break;

                case (3, "M"): m.WinProbabilityPlayer1MS3 = wp; break;
                case (3, "SM"): m.WinProbabilityPlayer1SMS3 = wp; break;
                case (3, "GSM"): m.WinProbabilityPlayer1GSMS3 = wp; break;

                case (4, "M"): m.WinProbabilityPlayer1MS4 = wp; break;
                case (4, "SM"): m.WinProbabilityPlayer1SMS4 = wp; break;
                case (4, "GSM"): m.WinProbabilityPlayer1GSMS4 = wp; break;
            }
        }
    }
}