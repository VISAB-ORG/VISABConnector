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
            Debug.Log(file.Content);
        }

        public void ReceiveFile()
        {

        }
    }
}