using System.Collections.Generic;

namespace VISABConnector.Example
{
    public class TetrisStatistics : IStatistics
    {
        public int Turn { get; set; }

        public IDictionary<string, int> PlayerPoints { get; set; }
    }
}