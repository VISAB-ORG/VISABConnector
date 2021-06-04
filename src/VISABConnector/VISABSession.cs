using System;
using System.Net.Http;
using System.Threading.Tasks;
using VISABConnector.Http;

namespace VISABConnector
{
    ///<inheritdoc cref="IVISABSession"/>
    internal class VISABSession : IVISABSession
    {
        #region VISAB WebApi endpoints

        /// <summary>
        /// Relative endpoint for sending map images in VISAB API
        /// </summary>
        private const string ENDPOINT_MAP = "send/map";

        /// <summary>
        /// Relative endpoint for closing session in VISAB API
        /// </summary>
        private const string ENDPOINT_SESSION_CLOSE = "session/close";

        /// <summary>
        /// Relative endpoint for opening session in VISAB API
        /// </summary>
        private const string ENDPOINT_SESSION_OPEN = "session/open";

        /// <summary>
        /// Relative endpoint for sending statistics in VISAB API
        /// </summary>
        private const string ENDPOINT_STATISTICS = "send/statistics";

        #endregion VISAB WebApi endpoints

        /// <summary>
        /// </summary>
        /// <param name="baseAdress">The base adress of VISAB</param>
        /// <param name="game">The game of the session</param>
        internal VISABSession(string baseAdress, string game)
        {
            Game = game;
            RequestHandler = new VISABRequestHandler(baseAdress, game);
        }

        public event EventHandler<ClosingEventArgs> CloseSessionEvent;

        public string Game { get; }

        public bool IsActive { get; private set; }

        public IVISABRequestHandler RequestHandler { get; }

        public Guid SessionId { get; private set; }

        public async Task<ApiResponse<string>> CloseSession()
        {
            CloseSessionEvent?.Invoke(this, new ClosingEventArgs { RequestHandler = RequestHandler });

            var response = await RequestHandler.GetResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_CLOSE, null, null).ConfigureAwait(false);

            if (response.IsSuccess)
                IsActive = false;

            return response;
        }

        public async Task<ApiResponse<string>> SendMap<T>(T map) where T : IImage
        {
            return await RequestHandler.GetResponseAsync(HttpMethod.Get, ENDPOINT_MAP, null, map).ConfigureAwait(false);
        }

        public async Task<ApiResponse<string>> SendStatistics<T>(T statistics) where T : IVISABStatistics
        {
            return await RequestHandler.GetResponseAsync(HttpMethod.Post, ENDPOINT_STATISTICS, null, statistics).ConfigureAwait(false);
        }

        internal async Task<bool> OpenSession()
        {
            var response = await RequestHandler.GetDeserializedResponseAsync<Guid>(HttpMethod.Get, ENDPOINT_SESSION_OPEN, null, null).ConfigureAwait(false);
            if (response.IsSuccess)
            {
                IsActive = true;
                SessionId = response.Content;
                RequestHandler.AddDefaultHeader("sessionid", SessionId);
            }

            return response.IsSuccess;
        }
    }
}