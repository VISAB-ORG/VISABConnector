using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VISABConnector.Example
{
    public class LoopBasedExample : MonoBehaviour
    {
        private IVISABSession session;

        private void Awake()
        {
            var metaInformation = new TetrisMetaInformation
            {
                PlayerNames = new List<string> { "Horst", "Dieter" }
            };

            LoopBasedSession.MessageAddedEvent += m => Debug.Log(m);
            var success = RoundBasedSession.StartSessionAsync(metaInformation, "http://localhost", 2673, 1).Result;
            if (!success)
                throw new Exception();

            session = RoundBasedSession.Session;
            // TODO: Add VISABConnector.Unity example
        }

        private async void OnTurnEnded(int nextTurn, IList<Player> players)
        {
            var playerPoints = new Dictionary<string, int>();
            foreach (var player in players)
                playerPoints[player.Name] = player.Score;

            var statistics = new TetrisStatistics
            {
                PlayerPoints = playerPoints,
                Turn = nextTurn - 1
            };

            // If this is not an asynchronous method, we have to use block by using .Result instead again.
            await RoundBasedSession.SendStatisticsAsync(statistics);
        }

        private async void OnGameEnded()
        {
            await RoundBasedSession.CloseSessionAsync();
        }
    }
}
