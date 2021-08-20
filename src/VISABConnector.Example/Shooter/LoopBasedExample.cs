using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VISABConnector.Unity;

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
            var images = new ShooterImages();

            var players = new Dictionary<string, string>
            {
                { "Fast Player", "Prefabs/fast_character" },
                { "Strong Player", "Prefabs/strong_character" },
            };

            var MapConfig = new SnapshotConfiguration
            {
                ImageHeight = 550,
                ImageWidth = 550,
                InstantiationSettings = new InstantiationConfiguration
                {
                    PrefabPath = "Prefabs/Map",
                    SpawnLocation = new Vector3(500, 500, 500)
                },
                CameraConfiguration = new CameraConfiguration
                {
                    CameraOffset = 2f,
                    Orthographic = true,
                    CameraRotation = new Vector3(90, 0, 45),
                    OrthographicSize = 75f
                }
            };

            var map = ImageCreator.TakeSnapshot(MapConfig);

            images.Map = map;

            Func<string, SnapshotConfiguration> defaultInstantiate = (prefabPath) => new SnapshotConfiguration
            {
                ImageHeight = 1024,
                ImageWidth = 1024,
                InstantiationSettings = new InstantiationConfiguration
                {
                    PrefabPath = prefabPath,
                    SpawnLocation = new Vector3(100, 100, 100),
                },
                CameraConfiguration = new CameraConfiguration
                {
                    CameraOffset = 1.5f,
                    Orthographic = false,
                    UseAbsoluteOffset = false,
                    CameraRotation = new Vector3(90, 0, 0)
                }
            };

            foreach (var pair in players)
            {
                var config = defaultInstantiate(pair.Value);
                var bytes = ImageCreator.TakeSnapshot(config);

                images.PlayerAvatars.Add(pair.Key, bytes);
            }

            LoopBasedSession.SendImagesAsync(images).Wait();


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