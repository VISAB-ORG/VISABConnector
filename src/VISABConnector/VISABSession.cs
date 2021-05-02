﻿using System;
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
        /// Relative endpoint for sending map images in VISAB API
        /// </summary>
        private const string ENDPOINT_MAP_IMAGE = "send/map/image";

        /// <summary>
        /// Relative endpoint for sending map information in VISAB API
        /// </summary>
        private const string ENDPOINT_MAP_INFORMATION = "send/map/information";

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
        public async Task<ApiResponse> CloseSession()
        {
            CloseSessionEvent?.Invoke(this, new ClosingEventArgs { RequestHandler = RequestHandler });

            var response = await RequestHandler.GetApiResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_CLOSE, null, null).ConfigureAwait(false);

            if (response.IsSuccess)
                IsActive = false;

            return response;
        }

        ///<inheritdoc/>
        public async Task<ApiResponse> SendMap<T>(T map) where T : IUnityMap
        {
            return await RequestHandler.GetApiResponseAsync(HttpMethod.Get, ENDPOINT_MAP_IMAGE, null, map).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<ApiResponse> SendMapInformation<T>(T mapInformation) where T : IUnityMapInformation
        {
            return await RequestHandler.GetApiResponseAsync(HttpMethod.Get, ENDPOINT_MAP_INFORMATION, null, mapInformation).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<ApiResponse> SendStatistics<T>(T statistics) where T : IVISABStatistics
        {
            return await RequestHandler.GetApiResponseAsync(HttpMethod.Post, ENDPOINT_STATISTICS, null, statistics).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        internal async Task<ApiResponse> OpenSession()
        {
            return await RequestHandler.GetApiResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_OPEN, null, null).ConfigureAwait(false);
        }
    }
}