using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace VISABConnector
{
    ///<inheritdoc cref="IVISABSession"/>
    internal class VISABSession : IVISABSession
    {
        #region VISAB WebApi endpoints

        /// <summary>
        /// Relative endpoint for closing session to VISAB API
        /// </summary>
        private const string EndpointCloseSession = "session/close";

        /// <summary>
        /// Relative endpoint for getting the created file at VISAB Api.
        /// </summary>
        private const string EndpointGetFile = "file/get";

        /// <summary>
        /// Relative endpoint for sending images to VISAB API.
        /// </summary>
        private const string EndpointSendImage = "send/image";

        /// <summary>
        /// Relative endpoint for sending statistics to VISAB API
        /// </summary>
        private const string EndpointSendStatistics = "send/statistics";

        #endregion VISAB WebApi endpoints

        private const string SessionAlreadyClosedResponse = "SESSIONALREADYCLOSED";

        /// <summary>
        /// </summary>
        /// <param name="game">The game of the session</param>
        /// <param name="sessionId">The id of session</param>
        /// <param name="requestHandler">The request handler that will be used by the session</param>
        internal VISABSession(string game, Guid sessionId, IVISABRequestHandler requestHandler)
        {
            Game = game;
            SessionId = sessionId;
            RequestHandler = requestHandler;
            IsActive = true;
        }

        public event EventHandler<ClosingEventArgs> CloseSessionEvent;

        public string Game { get; }

        public bool IsActive { get; private set; }

        public IVISABRequestHandler RequestHandler { get; }

        public Guid SessionId { get; }

        public async Task<ApiResponse<string>> CloseSession()
        {
            CloseSessionEvent?.Invoke(this, new ClosingEventArgs { RequestHandler = RequestHandler });

            var response = await RequestHandler.GetResponseAsync(HttpMethod.Get, EndpointCloseSession, null, null).ConfigureAwait(false);

            if (response.IsSuccess)
                IsActive = false;

            return response;
        }

        public async Task<ApiResponse<string>> GetCreatedFile()
        {
            var @params = new List<string> { $"sessionid={SessionId}" };

            return await RequestHandler.GetResponseAsync(HttpMethod.Get, EndpointGetFile, @params, null).ConfigureAwait(false);
        }

        public async Task<ApiResponse<string>> SendImage(IImage image)
        {
            var response = await RequestHandler.GetResponseAsync(HttpMethod.Get, EndpointSendImage, null, image).ConfigureAwait(false);
            if (!response.IsSuccess && response.ErrorMessage.Contains(SessionAlreadyClosedResponse))
                IsActive = false;

            return response;
        }

        public async Task<ApiResponse<string>> SendStatistics(IVISABStatistics statistics)
        {
            var response = await RequestHandler.GetResponseAsync(HttpMethod.Post, EndpointSendStatistics, null, statistics).ConfigureAwait(false);
            if (!response.IsSuccess && response.ErrorMessage.Contains(SessionAlreadyClosedResponse))
                IsActive = false;

            return response;
        }
    }
}