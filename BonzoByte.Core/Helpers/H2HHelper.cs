using BonzoByte.Core.Models.TrueSkill;
using System.Text.RegularExpressions;

namespace BonzoByte.Core.Helpers
{
    public static class H2HHelper
    {
        // === NOVO: PRE-MATCH SNAPSHOT ZA AKTIVNE MEČEVE (BEZ IKAKVOG TS IZRAČUNA) ===
        // Kopira POST stanje iz prethodnog H2H meča => u trenutno meču to postaje "Old".
        // Tako su za aktivni meč: H2H == H2HOld (global + per-surface), a TS Old je poravnat na aktualne P1/P2.
        public static void InjectPreMatchSnapshot(
            Models.Match match,
            Func<Models.Match, Models.Match?> getLatestH2HFromRepo)
        {
            if (match?.Player1TPId == null || match.Player2TPId == null || match.DateTime == null)
                return;

            int curP1 = match.Player1TPId.Value;
            int curP2 = match.Player2TPId.Value;
            var now = match.DateTime.Value;

            var prev = getLatestH2HFromRepo(match);

            // želimo strogo prethodni meč (ako izvor vrati i budući ili isti datum)
            if (prev?.DateTime is DateTime prevDt && prevDt >= now)
                prev = null;

            if (prev is null)
            {
                // Nema prethodnog meča → H2H i H2HOld nule; TS Old ostavi kakav jest (ili ga po želji resetiraj)
                match.Player1H2HOld = match.Player2H2HOld = 0;
                match.Player1H2H = match.Player2H2H = 0;

                match.Player1H2HOldS1 = match.Player2H2HOldS1 = 0; match.Player1H2HS1 = match.Player2H2HS1 = 0;
                match.Player1H2HOldS2 = match.Player2H2HOldS2 = 0; match.Player1H2HS2 = match.Player2H2HS2 = 0;
                match.Player1H2HOldS3 = match.Player2H2HOldS3 = 0; match.Player1H2HS3 = match.Player2H2HS3 = 0;
                match.Player1H2HOldS4 = match.Player2H2HOldS4 = 0; match.Player1H2HS4 = match.Player2H2HS4 = 0;

                return;
            }

            bool sameOrder = (prev.Player1TPId == curP1 && prev.Player2TPId == curP2)
                             || (prev.Player1TPId == curP1 && prev.Player2TPId == null && curP2 == 0); // ultra-guard

            // helperi za mapiranje na aktualni P1/P2
            T P1<T>(T p1Prev, T p2Prev) => sameOrder ? p1Prev : p2Prev;
            T P2<T>(T p1Prev, T p2Prev) => sameOrder ? p2Prev : p1Prev;

            // --- H2H brojači (global) ---
            match.Player1H2HOld = P1(prev.Player1H2H ?? 0, prev.Player2H2H ?? 0);
            match.Player2H2HOld = P2(prev.Player1H2H ?? 0, prev.Player2H2H ?? 0);
            match.Player1H2H = match.Player1H2HOld;
            match.Player2H2H = match.Player2H2HOld;

            // --- H2H brojači po podlogama (kopiramo sve četiri, pre=post iz prethodnog; current = old) ---
            match.Player1H2HOldS1 = P1(prev.Player1H2HS1 ?? 0, prev.Player2H2HS1 ?? 0);
            match.Player2H2HOldS1 = P2(prev.Player1H2HS1 ?? 0, prev.Player2H2HS1 ?? 0);
            match.Player1H2HS1 = match.Player1H2HOldS1;
            match.Player2H2HS1 = match.Player2H2HOldS1;

            match.Player1H2HOldS2 = P1(prev.Player1H2HS2 ?? 0, prev.Player2H2HS2 ?? 0);
            match.Player2H2HOldS2 = P2(prev.Player1H2HS2 ?? 0, prev.Player2H2HS2 ?? 0);
            match.Player1H2HS2 = match.Player1H2HOldS2;
            match.Player2H2HS2 = match.Player2H2HOldS2;

            match.Player1H2HOldS3 = P1(prev.Player1H2HS3 ?? 0, prev.Player2H2HS3 ?? 0);
            match.Player2H2HOldS3 = P2(prev.Player1H2HS3 ?? 0, prev.Player2H2HS3 ?? 0);
            match.Player1H2HS3 = match.Player1H2HOldS3;
            match.Player2H2HS3 = match.Player2H2HOldS3;

            match.Player1H2HOldS4 = P1(prev.Player1H2HS4 ?? 0, prev.Player2H2HS4 ?? 0);
            match.Player2H2HOldS4 = P2(prev.Player1H2HS4 ?? 0, prev.Player2H2HS4 ?? 0);
            match.Player1H2HS4 = match.Player1H2HOldS4;
            match.Player2H2HS4 = match.Player2H2HOldS4;

            // --- TS H2H OLD (GLOBAL) — post iz prethodnog, orijentiran na current P1/P2 ---
            match.Player1H2HTrueSkillMeanOldM = P1(prev.Player1H2HTrueSkillMeanM ?? 0.0, prev.Player2H2HTrueSkillMeanM ?? 0.0);
            match.Player1H2HTrueSkillStandardDeviationOldM = P1(prev.Player1H2HTrueSkillStandardDeviationM ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationM ?? 0.0);
            match.Player2H2HTrueSkillMeanOldM = P2(prev.Player1H2HTrueSkillMeanM ?? 0.0, prev.Player2H2HTrueSkillMeanM ?? 0.0);
            match.Player2H2HTrueSkillStandardDeviationOldM = P2(prev.Player1H2HTrueSkillStandardDeviationM ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationM ?? 0.0);

            match.Player1H2HTrueSkillMeanOldSM = P1(prev.Player1H2HTrueSkillMeanSM ?? 0.0, prev.Player2H2HTrueSkillMeanSM ?? 0.0);
            match.Player1H2HTrueSkillStandardDeviationOldSM = P1(prev.Player1H2HTrueSkillStandardDeviationSM ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSM ?? 0.0);
            match.Player2H2HTrueSkillMeanOldSM = P2(prev.Player1H2HTrueSkillMeanSM ?? 0.0, prev.Player2H2HTrueSkillMeanSM ?? 0.0);
            match.Player2H2HTrueSkillStandardDeviationOldSM = P2(prev.Player1H2HTrueSkillStandardDeviationSM ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSM ?? 0.0);

            match.Player1H2HTrueSkillMeanOldGSM = P1(prev.Player1H2HTrueSkillMeanGSM ?? 0.0, prev.Player2H2HTrueSkillMeanGSM ?? 0.0);
            match.Player1H2HTrueSkillStandardDeviationOldGSM = P1(prev.Player1H2HTrueSkillStandardDeviationGSM ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSM ?? 0.0);
            match.Player2H2HTrueSkillMeanOldGSM = P2(prev.Player1H2HTrueSkillMeanGSM ?? 0.0, prev.Player2H2HTrueSkillMeanGSM ?? 0.0);
            match.Player2H2HTrueSkillStandardDeviationOldGSM = P2(prev.Player1H2HTrueSkillStandardDeviationGSM ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSM ?? 0.0);

            // --- TS H2H OLD (SURFACE) — samo za površinu ovog meča (ako je valjana) ---
            if (match.SurfaceId is int s && s >= 1 && s <= 4)
            {
                // M
                if (s == 1)
                {
                    match.Player1H2HTrueSkillMeanOldMS1 = P1(prev.Player1H2HTrueSkillMeanMS1 ?? 0.0, prev.Player2H2HTrueSkillMeanMS1 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldMS1 = P1(prev.Player1H2HTrueSkillStandardDeviationMS1 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationMS1 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldMS1 = P2(prev.Player1H2HTrueSkillMeanMS1 ?? 0.0, prev.Player2H2HTrueSkillMeanMS1 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldMS1 = P2(prev.Player1H2HTrueSkillStandardDeviationMS1 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationMS1 ?? 0.0);
                }
                if (s == 2)
                {
                    match.Player1H2HTrueSkillMeanOldMS2 = P1(prev.Player1H2HTrueSkillMeanMS2 ?? 0.0, prev.Player2H2HTrueSkillMeanMS2 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldMS2 = P1(prev.Player1H2HTrueSkillStandardDeviationMS2 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationMS2 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldMS2 = P2(prev.Player1H2HTrueSkillMeanMS2 ?? 0.0, prev.Player2H2HTrueSkillMeanMS2 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldMS2 = P2(prev.Player1H2HTrueSkillStandardDeviationMS2 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationMS2 ?? 0.0);
                }
                if (s == 3)
                {
                    match.Player1H2HTrueSkillMeanOldMS3 = P1(prev.Player1H2HTrueSkillMeanMS3 ?? 0.0, prev.Player2H2HTrueSkillMeanMS3 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldMS3 = P1(prev.Player1H2HTrueSkillStandardDeviationMS3 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationMS3 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldMS3 = P2(prev.Player1H2HTrueSkillMeanMS3 ?? 0.0, prev.Player2H2HTrueSkillMeanMS3 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldMS3 = P2(prev.Player1H2HTrueSkillStandardDeviationMS3 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationMS3 ?? 0.0);
                }
                if (s == 4)
                {
                    match.Player1H2HTrueSkillMeanOldMS4 = P1(prev.Player1H2HTrueSkillMeanMS4 ?? 0.0, prev.Player2H2HTrueSkillMeanMS4 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldMS4 = P1(prev.Player1H2HTrueSkillStandardDeviationMS4 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationMS4 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldMS4 = P2(prev.Player1H2HTrueSkillMeanMS4 ?? 0.0, prev.Player2H2HTrueSkillMeanMS4 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldMS4 = P2(prev.Player1H2HTrueSkillStandardDeviationMS4 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationMS4 ?? 0.0);
                }

                // SM
                if (s == 1)
                {
                    match.Player1H2HTrueSkillMeanOldSMS1 = P1(prev.Player1H2HTrueSkillMeanSMS1 ?? 0.0, prev.Player2H2HTrueSkillMeanSMS1 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldSMS1 = P1(prev.Player1H2HTrueSkillStandardDeviationSMS1 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSMS1 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldSMS1 = P2(prev.Player1H2HTrueSkillMeanSMS1 ?? 0.0, prev.Player2H2HTrueSkillMeanSMS1 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldSMS1 = P2(prev.Player1H2HTrueSkillStandardDeviationSMS1 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSMS1 ?? 0.0);
                }
                if (s == 2)
                {
                    match.Player1H2HTrueSkillMeanOldSMS2 = P1(prev.Player1H2HTrueSkillMeanSMS2 ?? 0.0, prev.Player2H2HTrueSkillMeanSMS2 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldSMS2 = P1(prev.Player1H2HTrueSkillStandardDeviationSMS2 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSMS2 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldSMS2 = P2(prev.Player1H2HTrueSkillMeanSMS2 ?? 0.0, prev.Player2H2HTrueSkillMeanSMS2 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldSMS2 = P2(prev.Player1H2HTrueSkillStandardDeviationSMS2 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSMS2 ?? 0.0);
                }
                if (s == 3)
                {
                    match.Player1H2HTrueSkillMeanOldSMS3 = P1(prev.Player1H2HTrueSkillMeanSMS3 ?? 0.0, prev.Player2H2HTrueSkillMeanSMS3 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldSMS3 = P1(prev.Player1H2HTrueSkillStandardDeviationSMS3 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSMS3 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldSMS3 = P2(prev.Player1H2HTrueSkillMeanSMS3 ?? 0.0, prev.Player2H2HTrueSkillMeanSMS3 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldSMS3 = P2(prev.Player1H2HTrueSkillStandardDeviationSMS3 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSMS3 ?? 0.0);
                }
                if (s == 4)
                {
                    match.Player1H2HTrueSkillMeanOldSMS4 = P1(prev.Player1H2HTrueSkillMeanSMS4 ?? 0.0, prev.Player2H2HTrueSkillMeanSMS4 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldSMS4 = P1(prev.Player1H2HTrueSkillStandardDeviationSMS4 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSMS4 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldSMS4 = P2(prev.Player1H2HTrueSkillMeanSMS4 ?? 0.0, prev.Player2H2HTrueSkillMeanSMS4 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldSMS4 = P2(prev.Player1H2HTrueSkillStandardDeviationSMS4 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationSMS4 ?? 0.0);
                }

                // GSM
                if (s == 1)
                {
                    match.Player1H2HTrueSkillMeanOldGSMS1 = P1(prev.Player1H2HTrueSkillMeanGSMS1 ?? 0.0, prev.Player2H2HTrueSkillMeanGSMS1 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldGSMS1 = P1(prev.Player1H2HTrueSkillStandardDeviationGSMS1 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSMS1 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldGSMS1 = P2(prev.Player1H2HTrueSkillMeanGSMS1 ?? 0.0, prev.Player2H2HTrueSkillMeanGSMS1 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldGSMS1 = P2(prev.Player1H2HTrueSkillStandardDeviationGSMS1 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSMS1 ?? 0.0);
                }
                if (s == 2)
                {
                    match.Player1H2HTrueSkillMeanOldGSMS2 = P1(prev.Player1H2HTrueSkillMeanGSMS2 ?? 0.0, prev.Player2H2HTrueSkillMeanGSMS2 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldGSMS2 = P1(prev.Player1H2HTrueSkillStandardDeviationGSMS2 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSMS2 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldGSMS2 = P2(prev.Player1H2HTrueSkillMeanGSMS2 ?? 0.0, prev.Player2H2HTrueSkillMeanGSMS2 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldGSMS2 = P2(prev.Player1H2HTrueSkillStandardDeviationGSMS2 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSMS2 ?? 0.0);
                }
                if (s == 3)
                {
                    match.Player1H2HTrueSkillMeanOldGSMS3 = P1(prev.Player1H2HTrueSkillMeanGSMS3 ?? 0.0, prev.Player2H2HTrueSkillMeanGSMS3 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldGSMS3 = P1(prev.Player1H2HTrueSkillStandardDeviationGSMS3 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSMS3 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldGSMS3 = P2(prev.Player1H2HTrueSkillMeanGSMS3 ?? 0.0, prev.Player2H2HTrueSkillMeanGSMS3 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldGSMS3 = P2(prev.Player1H2HTrueSkillStandardDeviationGSMS3 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSMS3 ?? 0.0);
                }
                if (s == 4)
                {
                    match.Player1H2HTrueSkillMeanOldGSMS4 = P1(prev.Player1H2HTrueSkillMeanGSMS4 ?? 0.0, prev.Player2H2HTrueSkillMeanGSMS4 ?? 0.0);
                    match.Player1H2HTrueSkillStandardDeviationOldGSMS4 = P1(prev.Player1H2HTrueSkillStandardDeviationGSMS4 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSMS4 ?? 0.0);
                    match.Player2H2HTrueSkillMeanOldGSMS4 = P2(prev.Player1H2HTrueSkillMeanGSMS4 ?? 0.0, prev.Player2H2HTrueSkillMeanGSMS4 ?? 0.0);
                    match.Player2H2HTrueSkillStandardDeviationOldGSMS4 = P2(prev.Player1H2HTrueSkillStandardDeviationGSMS4 ?? 0.0, prev.Player2H2HTrueSkillStandardDeviationGSMS4 ?? 0.0);
                }
            }

            // NE diramo WinProbabilityPlayer1H2H* niti nove TS vrijednosti.
        }

        // === POSTOJEĆE: pun “finish” flow za završene mečeve (ostaje kako je) ===
        public static void ComputeAndInjectH2H(
            Models.Match match,
            Func<Models.Match, Models.Match?> getLatestH2HFromRepo,
            bool useGlobalMSDForAllModels = true)
        {
            if (match?.Player1TPId == null || match.Player2TPId == null || match.DateTime == null)
                return;

            int p1 = match.Player1TPId.Value;
            int p2 = match.Player2TPId.Value;
            int surface = match.SurfaceId ?? 0;
            var now = match.DateTime.Value;

            var prev = getLatestH2HFromRepo(match);
            if (prev?.DateTime is DateTime prevDt && prevDt >= now)
                prev = null;

            var (p1Tot, p2Tot) = DeriveTotalsFromPrev(prev, p1, p2);
            match.Player1H2H = p1Tot; match.Player2H2H = p2Tot;
            match.Player1H2HOld = p1Tot; match.Player2H2HOld = p2Tot;

            for (int s = 1; s <= 4; s++)
            {
                var (p1S, p2S) = DeriveSurfaceTotalsFromPrev(prev, p1, p2, s);
                if (s == 1) { match.Player1H2HS1 = p1S; match.Player2H2HS1 = p2S; match.Player1H2HOldS1 = p1S; match.Player2H2HOldS1 = p2S; }
                if (s == 2) { match.Player1H2HS2 = p1S; match.Player2H2HS2 = p2S; match.Player1H2HOldS2 = p1S; match.Player2H2HOldS2 = p2S; }
                if (s == 3) { match.Player1H2HS3 = p1S; match.Player2H2HS3 = p2S; match.Player1H2HOldS3 = p1S; match.Player2H2HOldS3 = p2S; }
                if (s == 4) { match.Player1H2HS4 = p1S; match.Player2H2HS4 = p2S; match.Player1H2HOldS4 = p1S; match.Player2H2HOldS4 = p2S; }
            }

            var defaults = (mean: Moserware.Skills.GameInfo.DefaultGameInfo.InitialMean,
                            sd: Moserware.Skills.GameInfo.DefaultGameInfo.InitialStandardDeviation);

            var (mu1M, sd1M, mu2M, sd2M) = GetPrevTsGlobal(prev, "M", p1, p2, defaults, useGlobalMSDForAllModels);
            var (mu1SM, sd1SM, mu2SM, sd2SM) = GetPrevTsGlobal(prev, "SM", p1, p2, defaults, useGlobalMSDForAllModels);
            var (mu1G, sd1G, mu2G, sd2G) = GetPrevTsGlobal(prev, "GSM", p1, p2, defaults, useGlobalMSDForAllModels);

            match.Player1H2HTrueSkillMeanOldM = mu1M; match.Player1H2HTrueSkillStandardDeviationOldM = sd1M;
            match.Player2H2HTrueSkillMeanOldM = mu2M; match.Player2H2HTrueSkillStandardDeviationOldM = sd2M;

            match.Player1H2HTrueSkillMeanOldSM = mu1SM; match.Player1H2HTrueSkillStandardDeviationOldSM = sd1SM;
            match.Player2H2HTrueSkillMeanOldSM = mu2SM; match.Player2H2HTrueSkillStandardDeviationOldSM = sd2SM;

            match.Player1H2HTrueSkillMeanOldGSM = mu1G; match.Player1H2HTrueSkillStandardDeviationOldGSM = sd1G;
            match.Player2H2HTrueSkillMeanOldGSM = mu2G; match.Player2H2HTrueSkillStandardDeviationOldGSM = sd2G;

            if (surface is >= 1 and <= 4)
            {
                var (mu1MS, sd1MS, mu2MS, sd2MS) = GetPrevTsSurface(prev, surface, "M", p1, p2, defaults, useGlobalMSDForAllModels);
                var (mu1SMS, sd1SMS, mu2SMS, sd2SMS) = GetPrevTsSurface(prev, surface, "SM", p1, p2, defaults, useGlobalMSDForAllModels);
                var (mu1GS, sd1GS, mu2GS, sd2GS) = GetPrevTsSurface(prev, surface, "GSM", p1, p2, defaults, useGlobalMSDForAllModels);

                ApplyOldSurfaceSnapshot(match, surface, "M", mu1MS, sd1MS, mu2MS, sd2MS);
                ApplyOldSurfaceSnapshot(match, surface, "SM", mu1SMS, sd1SMS, mu2SMS, sd2SMS);
                ApplyOldSurfaceSnapshot(match, surface, "GSM", mu1GS, sd1GS, mu2GS, sd2GS);

                var tsMS = TrueSkillHelper.CalculateM(mu1MS, mu2MS, sd1MS, sd2MS);
                var tsSMS = TrueSkillHelper.CalculateSM(mu1SMS, mu2SMS, sd1SMS, sd2SMS, p1, p2, match.ResultDetails ?? "");
                var tsGS = TrueSkillHelper.CalculateGSM(mu1GS, mu2GS, sd1GS, sd2GS, p1, p2, match.ResultDetails ?? "");
                ApplyNewSurface(match, surface, "M", tsMS);
                ApplyNewSurface(match, surface, "SM", tsSMS);
                ApplyNewSurface(match, surface, "GSM", tsGS);
            }

            var tsM = TrueSkillHelper.CalculateM(mu1M, mu2M, sd1M, sd2M);
            var tsSM = TrueSkillHelper.CalculateSM(mu1SM, mu2SM, sd1SM, sd2SM, p1, p2, match.ResultDetails ?? "");
            var tsG = TrueSkillHelper.CalculateGSM(mu1G, mu2G, sd1G, sd2G, p1, p2, match.ResultDetails ?? "");

            match.Player1H2HTrueSkillMeanM = tsM.MeanP1; match.Player1H2HTrueSkillStandardDeviationM = tsM.SdP1; match.WinProbabilityPlayer1H2HM = tsM.WinProbabilityP1;
            match.Player2H2HTrueSkillMeanM = tsM.MeanP2; match.Player2H2HTrueSkillStandardDeviationM = tsM.SdP2;

            match.Player1H2HTrueSkillMeanSM = tsSM.MeanP1; match.Player1H2HTrueSkillStandardDeviationSM = tsSM.SdP1; match.WinProbabilityPlayer1H2HSM = tsSM.WinProbabilityP1;
            match.Player2H2HTrueSkillMeanSM = tsSM.MeanP2; match.Player2H2HTrueSkillStandardDeviationSM = tsSM.SdP2;

            match.Player1H2HTrueSkillMeanGSM = tsG.MeanP1; match.Player1H2HTrueSkillStandardDeviationGSM = tsG.SdP1; match.WinProbabilityPlayer1H2HGSM = tsG.WinProbabilityP1;
            match.Player2H2HTrueSkillMeanGSM = tsG.MeanP2; match.Player2H2HTrueSkillStandardDeviationGSM = tsG.SdP2;
        }

        // === POMOĆNE FUNKCIJE (ostaju iste) ===

        private static (int p1Total, int p2Total) DeriveTotalsFromPrev(Models.Match? prev, int curP1, int curP2)
        {
            if (prev == null) return (0, 0);

            int a = (prev.Player1TPId == curP1) ? (prev.Player1H2H ?? 0) : (prev.Player2H2H ?? 0);
            int b = (prev.Player1TPId == curP1) ? (prev.Player2H2H ?? 0) : (prev.Player1H2H ?? 0);

            bool prevP1Won = P1Won(prev.Result);
            bool prevSideAIsCurP1 = prev.Player1TPId == curP1;

            if (prevP1Won) { if (prevSideAIsCurP1) a++; else b++; }
            else { if (prevSideAIsCurP1) b++; else a++; }

            return (a, b);
        }

        private static (int p1Total, int p2Total) DeriveSurfaceTotalsFromPrev(Models.Match? prev, int curP1, int curP2, int s)
        {
            if (prev == null) return (0, 0);

            (int a, int b) = s switch
            {
                1 => ((prev.Player1TPId == curP1) ? (prev.Player1H2HS1 ?? 0) : (prev.Player2H2HS1 ?? 0),
                      (prev.Player1TPId == curP1) ? (prev.Player2H2HS1 ?? 0) : (prev.Player1H2HS1 ?? 0)),
                2 => ((prev.Player1TPId == curP1) ? (prev.Player1H2HS2 ?? 0) : (prev.Player2H2HS2 ?? 0),
                      (prev.Player1TPId == curP1) ? (prev.Player2H2HS2 ?? 0) : (prev.Player1H2HS2 ?? 0)),
                3 => ((prev.Player1TPId == curP1) ? (prev.Player1H2HS3 ?? 0) : (prev.Player2H2HS3 ?? 0),
                      (prev.Player1TPId == curP1) ? (prev.Player2H2HS3 ?? 0) : (prev.Player1H2HS3 ?? 0)),
                4 => ((prev.Player1TPId == curP1) ? (prev.Player1H2HS4 ?? 0) : (prev.Player2H2HS4 ?? 0),
                      (prev.Player1TPId == curP1) ? (prev.Player2H2HS4 ?? 0) : (prev.Player1H2HS4 ?? 0)),
                _ => (0, 0)
            };

            if (prev.SurfaceId == s)
            {
                bool prevP1Won = P1Won(prev.Result);
                bool prevSideAIsCurP1 = prev.Player1TPId == curP1;

                if (prevP1Won) { if (prevSideAIsCurP1) a++; else b++; }
                else { if (prevSideAIsCurP1) b++; else a++; }
            }

            return (a, b);
        }

        private static bool P1Won(string? result)
        {
            if (string.IsNullOrWhiteSpace(result)) return true;
            var m = Regex.Match(result, @"^\s*(\d)\D*(\d)");
            if (m.Success)
            {
                int a = m.Groups[1].Value[0] - '0';
                int b = m.Groups[2].Value[0] - '0';
                return a >= b;
            }
            return result is "20" or "21" or "30" or "31" or "32";
        }

        private static (double mu1, double sd1, double mu2, double sd2) GetPrevTsGlobal(
            Models.Match? prev, string model, int curP1, int curP2, (double mean, double sd) dflt, bool useGlobalMSD)
        {
            if (prev == null) return (dflt.mean, dflt.sd, dflt.mean, dflt.sd);

            bool sameOrder = prev.Player1TPId == curP1;

            if (model == "M")
            {
                double p1M = prev.Player1H2HTrueSkillMeanM ?? dflt.mean;
                double s1M = prev.Player1H2HTrueSkillStandardDeviationM ?? dflt.sd;
                double p2M = prev.Player2H2HTrueSkillMeanM ?? dflt.mean;
                double s2M = prev.Player2H2HTrueSkillStandardDeviationM ?? dflt.sd;
                return sameOrder ? (p1M, s1M, p2M, s2M) : (p2M, s2M, p1M, s1M);
            }
            else if (model == "SM")
            {
                double p1 = prev.Player1H2HTrueSkillMeanSM ?? dflt.mean;
                double p2 = prev.Player2H2HTrueSkillMeanSM ?? dflt.mean;
                double s1 = useGlobalMSD ? (prev.Player1H2HTrueSkillStandardDeviationM ?? dflt.sd)
                                         : (prev.Player1H2HTrueSkillStandardDeviationSM ?? dflt.sd);
                double s2 = useGlobalMSD ? (prev.Player2H2HTrueSkillStandardDeviationM ?? dflt.sd)
                                         : (prev.Player2H2HTrueSkillStandardDeviationSM ?? dflt.sd);
                return sameOrder ? (p1, s1, p2, s2) : (p2, s2, p1, s1);
            }
            else // GSM
            {
                double p1 = prev.Player1H2HTrueSkillMeanGSM ?? dflt.mean;
                double p2 = prev.Player2H2HTrueSkillMeanGSM ?? dflt.mean;
                double s1 = useGlobalMSD ? (prev.Player1H2HTrueSkillStandardDeviationM ?? dflt.sd)
                                         : (prev.Player1H2HTrueSkillStandardDeviationGSM ?? dflt.sd);
                double s2 = useGlobalMSD ? (prev.Player2H2HTrueSkillStandardDeviationM ?? dflt.sd)
                                         : (prev.Player2H2HTrueSkillStandardDeviationGSM ?? dflt.sd);
                return sameOrder ? (p1, s1, p2, s2) : (p2, s2, p1, s1);
            }
        }

        private static (double mu1, double sd1, double mu2, double sd2) GetPrevTsSurface(
            Models.Match? prev, int s, string model, int curP1, int curP2, (double mean, double sd) dflt, bool useGlobalMSD)
        {
            if (prev == null) return (dflt.mean, dflt.sd, dflt.mean, dflt.sd);

            bool sameOrder = prev.Player1TPId == curP1;

            double p1, sd1, p2, sd2;

            double SD_M_P1(int surf) => surf switch
            {
                1 => prev.Player1H2HTrueSkillStandardDeviationMS1 ?? dflt.sd,
                2 => prev.Player1H2HTrueSkillStandardDeviationMS2 ?? dflt.sd,
                3 => prev.Player1H2HTrueSkillStandardDeviationMS3 ?? dflt.sd,
                4 => prev.Player1H2HTrueSkillStandardDeviationMS4 ?? dflt.sd,
                _ => dflt.sd
            };
            double SD_M_P2(int surf) => surf switch
            {
                1 => prev.Player2H2HTrueSkillStandardDeviationMS1 ?? dflt.sd,
                2 => prev.Player2H2HTrueSkillStandardDeviationMS2 ?? dflt.sd,
                3 => prev.Player2H2HTrueSkillStandardDeviationMS3 ?? dflt.sd,
                4 => prev.Player2H2HTrueSkillStandardDeviationMS4 ?? dflt.sd,
                _ => dflt.sd
            };

            if (model == "M")
            {
                (p1, sd1, p2, sd2) = s switch
                {
                    1 => (prev.Player1H2HTrueSkillMeanMS1 ?? dflt.mean, prev.Player1H2HTrueSkillStandardDeviationMS1 ?? dflt.sd,
                          prev.Player2H2HTrueSkillMeanMS1 ?? dflt.mean, prev.Player2H2HTrueSkillStandardDeviationMS1 ?? dflt.sd),
                    2 => (prev.Player1H2HTrueSkillMeanMS2 ?? dflt.mean, prev.Player1H2HTrueSkillStandardDeviationMS2 ?? dflt.sd,
                          prev.Player2H2HTrueSkillMeanMS2 ?? dflt.mean, prev.Player2H2HTrueSkillStandardDeviationMS2 ?? dflt.sd),
                    3 => (prev.Player1H2HTrueSkillMeanMS3 ?? dflt.mean, prev.Player1H2HTrueSkillStandardDeviationMS3 ?? dflt.sd,
                          prev.Player2H2HTrueSkillMeanMS3 ?? dflt.mean, prev.Player2H2HTrueSkillStandardDeviationMS3 ?? dflt.sd),
                    4 => (prev.Player1H2HTrueSkillMeanMS4 ?? dflt.mean, prev.Player1H2HTrueSkillStandardDeviationMS4 ?? dflt.sd,
                          prev.Player2H2HTrueSkillMeanMS4 ?? dflt.mean, prev.Player2H2HTrueSkillStandardDeviationMS4 ?? dflt.sd),
                    _ => (dflt.mean, dflt.sd, dflt.mean, dflt.sd)
                };
            }
            else if (model == "SM")
            {
                (p1, sd1, p2, sd2) = s switch
                {
                    1 => (prev.Player1H2HTrueSkillMeanSMS1 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P1(1) : (prev.Player1H2HTrueSkillStandardDeviationSMS1 ?? dflt.sd),
                          prev.Player2H2HTrueSkillMeanSMS1 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P2(1) : (prev.Player2H2HTrueSkillStandardDeviationSMS1 ?? dflt.sd)),
                    2 => (prev.Player1H2HTrueSkillMeanSMS2 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P1(2) : (prev.Player1H2HTrueSkillStandardDeviationSMS2 ?? dflt.sd),
                          prev.Player2H2HTrueSkillMeanSMS2 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P2(2) : (prev.Player2H2HTrueSkillStandardDeviationSMS2 ?? dflt.sd)),
                    3 => (prev.Player1H2HTrueSkillMeanSMS3 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P1(3) : (prev.Player1H2HTrueSkillStandardDeviationSMS3 ?? dflt.sd),
                          prev.Player2H2HTrueSkillMeanSMS3 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P2(3) : (prev.Player2H2HTrueSkillStandardDeviationSMS3 ?? dflt.sd)),
                    4 => (prev.Player1H2HTrueSkillMeanSMS4 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P1(4) : (prev.Player1H2HTrueSkillStandardDeviationSMS4 ?? dflt.sd),
                          prev.Player2H2HTrueSkillMeanSMS4 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P2(4) : (prev.Player2H2HTrueSkillStandardDeviationSMS4 ?? dflt.sd)),
                    _ => (dflt.mean, dflt.sd, dflt.mean, dflt.sd)
                };
            }
            else // GSM
            {
                (p1, sd1, p2, sd2) = s switch
                {
                    1 => (prev.Player1H2HTrueSkillMeanGSMS1 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P1(1) : (prev.Player1H2HTrueSkillStandardDeviationGSMS1 ?? dflt.sd),
                          prev.Player2H2HTrueSkillMeanGSMS1 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P2(1) : (prev.Player2H2HTrueSkillStandardDeviationGSMS1 ?? dflt.sd)),
                    2 => (prev.Player1H2HTrueSkillMeanGSMS2 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P1(2) : (prev.Player1H2HTrueSkillStandardDeviationGSMS2 ?? dflt.sd),
                          prev.Player2H2HTrueSkillMeanGSMS2 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P2(2) : (prev.Player2H2HTrueSkillStandardDeviationGSMS2 ?? dflt.sd)),
                    3 => (prev.Player1H2HTrueSkillMeanGSMS3 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P1(3) : (prev.Player1H2HTrueSkillStandardDeviationGSMS3 ?? dflt.sd),
                          prev.Player2H2HTrueSkillMeanGSMS3 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P2(3) : (prev.Player2H2HTrueSkillStandardDeviationGSMS3 ?? dflt.sd)),
                    4 => (prev.Player1H2HTrueSkillMeanGSMS4 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P1(4) : (prev.Player1H2HTrueSkillStandardDeviationGSMS4 ?? dflt.sd),
                          prev.Player2H2HTrueSkillMeanGSMS4 ?? dflt.mean,
                          useGlobalMSD ? SD_M_P2(4) : (prev.Player2H2HTrueSkillStandardDeviationGSMS4 ?? dflt.sd)),
                    _ => (dflt.mean, dflt.sd, dflt.mean, dflt.sd)
                };
            }

            return sameOrder ? (p1, sd1, p2, sd2) : (p2, sd2, p1, sd1);
        }

        private static void ApplyOldSurfaceSnapshot(Models.Match m, int s, string model, double mu1, double sd1, double mu2, double sd2)
        {
            if (s == 1 && model == "M") { m.Player1H2HTrueSkillMeanOldMS1 = mu1; m.Player1H2HTrueSkillStandardDeviationOldMS1 = sd1; m.Player2H2HTrueSkillMeanOldMS1 = mu2; m.Player2H2HTrueSkillStandardDeviationOldMS1 = sd2; }
            if (s == 1 && model == "SM") { m.Player1H2HTrueSkillMeanOldSMS1 = mu1; m.Player1H2HTrueSkillStandardDeviationOldSMS1 = sd1; m.Player2H2HTrueSkillMeanOldSMS1 = mu2; m.Player2H2HTrueSkillStandardDeviationOldSMS1 = sd2; }
            if (s == 1 && model == "GSM") { m.Player1H2HTrueSkillMeanOldGSMS1 = mu1; m.Player1H2HTrueSkillStandardDeviationOldGSMS1 = sd1; m.Player2H2HTrueSkillMeanOldGSMS1 = mu2; m.Player2H2HTrueSkillStandardDeviationOldGSMS1 = sd2; }

            if (s == 2 && model == "M") { m.Player1H2HTrueSkillMeanOldMS2 = mu1; m.Player1H2HTrueSkillStandardDeviationOldMS2 = sd1; m.Player2H2HTrueSkillMeanOldMS2 = mu2; m.Player2H2HTrueSkillStandardDeviationOldMS2 = sd2; }
            if (s == 2 && model == "SM") { m.Player1H2HTrueSkillMeanOldSMS2 = mu1; m.Player1H2HTrueSkillStandardDeviationOldSMS2 = sd1; m.Player2H2HTrueSkillMeanOldSMS2 = mu2; m.Player2H2HTrueSkillStandardDeviationOldSMS2 = sd2; }
            if (s == 2 && model == "GSM") { m.Player1H2HTrueSkillMeanOldGSMS2 = mu1; m.Player1H2HTrueSkillStandardDeviationOldGSMS2 = sd1; m.Player2H2HTrueSkillMeanOldGSMS2 = mu2; m.Player2H2HTrueSkillStandardDeviationOldGSMS2 = sd2; }

            if (s == 3 && model == "M") { m.Player1H2HTrueSkillMeanOldMS3 = mu1; m.Player1H2HTrueSkillStandardDeviationOldMS3 = sd1; m.Player2H2HTrueSkillMeanOldMS3 = mu2; m.Player2H2HTrueSkillStandardDeviationOldMS3 = sd2; }
            if (s == 3 && model == "SM") { m.Player1H2HTrueSkillMeanOldSMS3 = mu1; m.Player1H2HTrueSkillStandardDeviationOldSMS3 = sd1; m.Player2H2HTrueSkillMeanOldSMS3 = mu2; m.Player2H2HTrueSkillStandardDeviationOldSMS3 = sd2; }
            if (s == 3 && model == "GSM") { m.Player1H2HTrueSkillMeanOldGSMS3 = mu1; m.Player1H2HTrueSkillStandardDeviationOldGSMS3 = sd1; m.Player2H2HTrueSkillMeanOldGSMS3 = mu2; m.Player2H2HTrueSkillStandardDeviationOldGSMS3 = sd2; }

            if (s == 4 && model == "M") { m.Player1H2HTrueSkillMeanOldMS4 = mu1; m.Player1H2HTrueSkillStandardDeviationOldMS4 = sd1; m.Player2H2HTrueSkillMeanOldMS4 = mu2; m.Player2H2HTrueSkillStandardDeviationOldMS4 = sd2; }
            if (s == 4 && model == "SM") { m.Player1H2HTrueSkillMeanOldSMS4 = mu1; m.Player1H2HTrueSkillStandardDeviationOldSMS4 = sd1; m.Player2H2HTrueSkillMeanOldSMS4 = mu2; m.Player2H2HTrueSkillStandardDeviationOldSMS4 = sd2; }
            if (s == 4 && model == "GSM") { m.Player1H2HTrueSkillMeanOldGSMS4 = mu1; m.Player1H2HTrueSkillStandardDeviationOldGSMS4 = sd1; m.Player2H2HTrueSkillMeanOldGSMS4 = mu2; m.Player2H2HTrueSkillStandardDeviationOldGSMS4 = sd2; }
        }

        private static void ApplyNewSurface(Models.Match m, int s, string model, TrueSkillResult ts)
        {
            if (s == 1 && model == "M") { m.Player1H2HTrueSkillMeanMS1 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationMS1 = ts.SdP1; m.Player2H2HTrueSkillMeanMS1 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationMS1 = ts.SdP2; m.WinProbabilityPlayer1H2HMS1 = ts.WinProbabilityP1; }
            if (s == 1 && model == "SM") { m.Player1H2HTrueSkillMeanSMS1 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationSMS1 = ts.SdP1; m.Player2H2HTrueSkillMeanSMS1 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationSMS1 = ts.SdP2; m.WinProbabilityPlayer1H2HSMS1 = ts.WinProbabilityP1; }
            if (s == 1 && model == "GSM") { m.Player1H2HTrueSkillMeanGSMS1 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationGSMS1 = ts.SdP1; m.Player2H2HTrueSkillMeanGSMS1 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationGSMS1 = ts.SdP2; m.WinProbabilityPlayer1H2HGSMS1 = ts.WinProbabilityP1; }

            if (s == 2 && model == "M") { m.Player1H2HTrueSkillMeanMS2 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationMS2 = ts.SdP1; m.Player2H2HTrueSkillMeanMS2 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationMS2 = ts.SdP2; m.WinProbabilityPlayer1H2HMS2 = ts.WinProbabilityP1; }
            if (s == 2 && model == "SM") { m.Player1H2HTrueSkillMeanSMS2 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationSMS2 = ts.SdP1; m.Player2H2HTrueSkillMeanSMS2 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationSMS2 = ts.SdP2; m.WinProbabilityPlayer1H2HSMS2 = ts.WinProbabilityP1; }
            if (s == 2 && model == "GSM") { m.Player1H2HTrueSkillMeanGSMS2 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationGSMS2 = ts.SdP1; m.Player2H2HTrueSkillMeanGSMS2 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationGSMS2 = ts.SdP2; m.WinProbabilityPlayer1H2HGSMS2 = ts.WinProbabilityP1; }

            if (s == 3 && model == "M") { m.Player1H2HTrueSkillMeanMS3 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationMS3 = ts.SdP1; m.Player2H2HTrueSkillMeanMS3 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationMS3 = ts.SdP2; m.WinProbabilityPlayer1H2HMS3 = ts.WinProbabilityP1; }
            if (s == 3 && model == "SM") { m.Player1H2HTrueSkillMeanSMS3 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationSMS3 = ts.SdP1; m.Player2H2HTrueSkillMeanSMS3 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationSMS3 = ts.SdP2; m.WinProbabilityPlayer1H2HSMS3 = ts.WinProbabilityP1; }
            if (s == 3 && model == "GSM") { m.Player1H2HTrueSkillMeanGSMS3 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationGSMS3 = ts.SdP1; m.Player2H2HTrueSkillMeanGSMS3 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationGSMS3 = ts.SdP2; m.WinProbabilityPlayer1H2HGSMS3 = ts.WinProbabilityP1; }

            if (s == 4 && model == "M") { m.Player1H2HTrueSkillMeanMS4 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationMS4 = ts.SdP1; m.Player2H2HTrueSkillMeanMS4 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationMS4 = ts.SdP2; m.WinProbabilityPlayer1H2HMS4 = ts.WinProbabilityP1; }
            if (s == 4 && model == "SM") { m.Player1H2HTrueSkillMeanSMS4 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationSMS4 = ts.SdP1; m.Player2H2HTrueSkillMeanSMS4 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationSMS4 = ts.SdP2; m.WinProbabilityPlayer1H2HSMS4 = ts.WinProbabilityP1; }
            if (s == 4 && model == "GSM") { m.Player1H2HTrueSkillMeanGSMS4 = ts.MeanP1; m.Player1H2HTrueSkillStandardDeviationGSMS4 = ts.SdP1; m.Player2H2HTrueSkillMeanGSMS4 = ts.MeanP2; m.Player2H2HTrueSkillStandardDeviationGSMS4 = ts.SdP2; m.WinProbabilityPlayer1H2HGSMS4 = ts.WinProbabilityP1; }
        }
    }
}