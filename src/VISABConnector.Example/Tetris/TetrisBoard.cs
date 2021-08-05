using UnityEngine;

namespace VISABConnector.Example.Tetris
{
    public class TetrisBoard
    {
        public Color[,] Board { get; } = new Color[20, 10];

        public int CalculateScore()
        {
            // Exclude from sample.
            return 0;
        }
    }
}