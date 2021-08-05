using System;
using System.Collections.Generic;
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
            ApiResponse<IVISABSession> sessionResponse = api.InitiateSession(metaInformation).Result;
            if (sessionResponse.IsSuccess)
                session = sessionResponse.Content;
            else
                throw new Exception(sessionResponse.ErrorMessage);

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
            ApiResponse<string> response = await session.SendStatistics(statistics);
            if (!response.IsSuccess)
                Debug.Log(response.ErrorMessage);
        }

        private async void OnGameEnded()
        {
            ApiResponse<string> response = await session.CloseSession();
            if (!response.IsSuccess)
                Debug.Log(response.ErrorMessage);
        }
    }
}