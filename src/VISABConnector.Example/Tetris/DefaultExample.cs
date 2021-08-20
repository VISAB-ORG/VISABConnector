using System;
using System.Collections.Generic;
using VISABConnector.Unity;
using UnityEngine;


namespace VISABConnector.Example.Tetris
{
    public class DefaultExample : MonoBehaviour
    {
        private IVISABSession session;

        private void Awake()
        {
            var api = new VISABApi("http://localhost", 2673, 1);

            var metaInformation = new TetrisMetaInformation
            {
                PlayerNames = new List<string> { "Horst", "Dieter" }
            };

            // If we are in an asynchronous (async) method, we should use await here instead!
            ApiResponse<IVISABSession> sessionResponse = api.InitiateSessionAsync(metaInformation).Result;
            if (sessionResponse.IsSuccess)
                session = sessionResponse.Content;
            else
                throw new Exception(sessionResponse.ErrorMessage);

            // TODO: Add VISABConnector.Unity example
            var images = new TetrisImages();

            var bricks = new Dictionary<string, string>
            {
                { "Big Brick", "Prefabs/Big_Brick" },
                { "Small Brick", "Prefabs/Small_Brick" },
                { "L Shape", "Prefabs/L_Brick" },
            };

            var canvasConfig = new SnapshotConfiguration
            {
                ImageHeight = 550,
                ImageWidth = 550,
                InstantiationSettings = new InstantiationConfiguration
                {
                    PrefabPath = "Prefabs/Canvas",
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

            var canvas = ImageCreator.TakeSnapshot(canvasConfig);

            images.Canvas = canvas;

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

            foreach (var pair in bricks)
            {
                var config = defaultInstantiate(pair.Value);
                var bytes = ImageCreator.TakeSnapshot(config);

                images.OneOneOneOne = bytes;
            }

            LoopBasedSession.SendImagesAsync(images).Wait();

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
            ApiResponse<string> response = await session.SendStatisticsAsync(statistics);
            if (!response.IsSuccess)
                Debug.Log(response.ErrorMessage);
        }

        private async void OnGameEnded()
        {
            ApiResponse<string> response = await session.CloseSessionAsync();
            if (!response.IsSuccess)
                Debug.Log(response.ErrorMessage);
        }

        private void IncrementPlayerPoints(Player player)
        {
            player.Score += player.Board.CalculateScore();
        }
    }
}