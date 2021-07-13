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
        private const string _endpointCloseSession = "session/close";

        /// <summary>
        /// Relative endpoint for getting the created file at VISAB Api.
        /// </summary>
        private const string _endpointGetFile = "file/get";

        /// <summary>
        /// Relative endpoint for sending images to VISAB API.
        /// </summary>
        private const string _endpointSendImage = "send/image";

        /// <summary>
        /// Relative endpoint for sending statistics to VISAB API
        /// </summary>
        private const string _endpointSendStatistics = "send/statistics";

        #endregion VISAB WebApi endpoints

        /// <summary>
        /// If a response message when sending images or statistics returned this, the session was
        /// already closed.
        /// </summary>
        private const string _sessionAlreadyClosedResponse = "SESSION_ALREADY_CLOSED";

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
        }

        public string Game { get; }

        public bool IsActive { get; private set; } = true;

        public IVISABRequestHandler RequestHandler { get; }

        public Guid SessionId { get; }

        public async Task<ApiResponse<string>> CloseSession()
        {
            var response = await RequestHandler.GetResponseAsync(HttpMethod.Get, _endpointCloseSession, null, null).ConfigureAwait(false);

            if (response.IsSuccess)
                IsActive = false;

            return response;
        }

        public async Task<ApiResponse<string>> GetCreatedFile()
        {
            var queryParameters = new Dictionary<string, string> { { "sessionid", SessionId.ToString() } };

            return await RequestHandler.GetResponseAsync(HttpMethod.Get, _endpointGetFile, null, queryParameters).ConfigureAwait(false);
        }

        public async Task<ApiResponse<string>> SendImage(IImageContainer image)
        {
            var response = await RequestHandler.GetResponseAsync(HttpMethod.Get, _endpointSendImage, image, null).ConfigureAwait(false);
            if (!response.IsSuccess && response.ErrorMessage.Contains(_sessionAlreadyClosedResponse))
                IsActive = false;

            return response;
        }

        public async Task<ApiResponse<string>> SendStatistics(IVISABStatistics statistics)
        {
            var response = await RequestHandler.GetResponseAsync(HttpMethod.Post, _endpointSendStatistics, statistics, null).ConfigureAwait(false);
            if (!response.IsSuccess && response.ErrorMessage.Contains(_sessionAlreadyClosedResponse))
                IsActive = false;

            return response;
        }
    }
}