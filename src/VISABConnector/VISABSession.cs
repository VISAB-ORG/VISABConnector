using System;
using System.Net.Http;
using System.Threading.Tasks;
using VISABConnector.Http;

namespace VISABConnector
{
    ///<inheritdoc cref="IVISABSession"/>
    public class VISABSession : IVISABSession
    {
        #region VISAB WebApi endpoints

        /// <summary>
        /// Relative endpoint for sending maps in VISAB API
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

        internal VISABSession(string game, Guid sessionId)
        {
            Game = game;
            SessionId = sessionId;
            RequestHandler = new VISABRequestHandler(game, sessionId);
        }

        ///<inheritdoc/>
        public event EventHandler<ClosingEventArgs> CloseSessionEvent;

        ///<inheritdoc/>
        public string Game { get; }

        ///<inheritdoc/>
        public bool IsActive { get; internal set; }

        ///<inheritdoc/>
        public IVISABRequestHandler RequestHandler { get; }

        ///<inheritdoc/>
        public Guid SessionId { get; }

        ///<inheritdoc/>
        public async Task<bool> CloseSession()
        {
            await Task.Run(() => CloseSessionEvent?.Invoke(this, new ClosingEventArgs { RequestHandler = RequestHandler })).ConfigureAwait(false);

            var closed = await RequestHandler.GetSuccessResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_CLOSE, null, null).ConfigureAwait(false);

            if (closed)
                IsActive = false;

            return closed;
        }

        ///<inheritdoc/>
        public Task<bool> SendMap<T>(T map) where T : IUnityMap
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public async Task<bool> SendStatistics<T>(T statistics) where T : IVISABStatistics
        {
            return await RequestHandler.GetSuccessResponseAsync(HttpMethod.Post, ENDPOINT_STATISTICS, null, statistics).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        internal async Task<bool> OpenSession()
        {
            return await RequestHandler.GetSuccessResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_OPEN, null, null).ConfigureAwait(false);
        }
    }
}