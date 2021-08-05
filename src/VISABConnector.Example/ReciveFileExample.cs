using UnityEngine;

namespace VISABConnector.Example
{
    public class ReciveFileExample : MonoBehaviour
    {
        private IVISABSession session;

        private async void OnEndGame()
        {
            await session.CloseSession();
            string file = await session.GetCreatedFile();
        }

        public void ReceiveFile()
        {

        }
    }
}