namespace BonzoByte.Core.Configs
{
    public class TrueskillSettings
    {
        public double InitialMean { get; set; }
        public double InitialStandardDeviation { get; set; }
        public double Beta { get; set; }
        public double DynamicsFactor { get; set; }
        public double DrawProbability { get; set; }
    }
}