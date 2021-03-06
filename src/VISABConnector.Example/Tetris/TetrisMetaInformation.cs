using System.Collections.Generic;

namespace VISABConnector.Example.Tetris
{
    public class TetrisMetaInformation : IMetaInformation
    {
        public string Game => "Tetris";

        public IList<string> PlayerNames { get; set; }

        public int PlayerCount => PlayerNames.Count;
    }
}