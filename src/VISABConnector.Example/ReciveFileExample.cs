using System;
using System.Threading;
using UnityEngine;

namespace VISABConnector.Example
{
    public class ReciveFileExample : MonoBehaviour
    {
        private IVISABSession session;

        private async void OnEndGame_Default()
        {
            await session.CloseSessionAsync();
            ApiResponse<string> file = await session.GetCreatedFileAsync();
            Debug.Log(file.Content);
        }

        private async void OnEndGame_RoundBased()
        {
            await RoundBasedSession.CloseSessionAsync();
            string file = await RoundBasedSession.GetFileAsync();
            Debug.Log(file);
        }

        private void Awake()
        {
            // Exclude from sample.
            var cts = new CancellationTokenSource();

            // Initiate session

            Action<string> fileReceivedHandler = json => System.IO.File.WriteAllText("MyVISABFile.visab2", json);
            LoopBasedSession.StartStatisticsLoopAsync(GetStatistics, () => true, 100, cts.Token, queryFile: true);
        }

        // Exclude from sample.
        private IStatistics GetStatistics()
        {
            return null;
        }
    }
}