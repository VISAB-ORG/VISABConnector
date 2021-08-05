using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace VISABConnector.Example.Shooter
{
    public class LoopBasedExample : MonoBehaviour
    {
        private IVISABSession session;
        private CancellationTokenSource cts;
        private List<Player> players;
        private bool isGamePaused;

        private void Awake()
        {
            var metaInformation = new ShooterMetaInformation
            {
                PlayerNames = new List<string> { "Soos", "Saas" }
            };

            LoopBasedSession.MessageAddedEvent += m => Debug.Log(m);
            bool success = LoopBasedSession.StartSessionAsync(metaInformation, "http://localhost", 2673, 1).Result;
            if (!success)
                throw new Exception();

            session = LoopBasedSession.Session;

            // TODO: Add VISABConnector.Unity example

            cts = new CancellationTokenSource();
            Func<bool> shouldSend = () => isGamePaused;

            LoopBasedSession.StartStatisticsLoopAsync(StatisticsFunc, shouldSend, 100, cts.Token);
        }

        // Will be called every 100 miliseconds.
        private ShooterStatistics StatisticsFunc()
        {
            var playerStatistics = new Dictionary<string, PlayerStatistics>();
            foreach (var player in players)
                playerStatistics[player.Name] = new PlayerStatistics { Kills = player.Kills, Deaths = player.Deaths };

            return new ShooterStatistics { PlayerStatistics = playerStatistics };
        }

        private async void OnGameEnded()
        {
            // Stop the infinite statistics loop
            cts.Cancel();

            await LoopBasedSession.CloseSessionAsync();
        }
    }
}