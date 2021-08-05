using System.Collections.Generic;

namespace VISABConnector.Example.Shooter
{
    public class PlayerStatistics
    {
        public int Deaths { get; set; }

        public int Kills { get; set; }
    }

    public class ShooterStatistics : IStatistics
    {
        public IDictionary<string, PlayerStatistics> PlayerStatistics { get; set; }
    }
}