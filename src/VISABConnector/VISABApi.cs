﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using VISABConnector.Http;

namespace VISABConnector
{
    /// <summary>
    /// Class for initializing transmission sessions and making requests independant of sessions to
    /// the VISAB WebApi.
    /// </summary>
    public class VISABApi
    {
        #region VISAB WebApi endpoints

        /// <summary>
        /// Relative endpoint for checking useability for current game in VISAB API
        /// </summary>
        private const string EndpointGameSupported = "games";

        /// <summary>
        /// Relative endpoints for listing the currently active sessions in VISAB API
        /// </summary>
        private const string EndpointListSessions = "session/list";

        /// <summary>
        /// Relative endpoint for opening session to VISAB API
        /// </summary>
        private const string EndpointOpenSession = "session/open";

        /// <summary>
        /// Relative endpoint for ping testing the VISAB API
        /// </summary>
        private const string EndpointPing = "ping";

        /// <summary>
        /// Relative endpoints for checking session status in VISAB API
        /// </summary>
        private const string EndpointSessionStatus = "session/status";

        #endregion VISAB WebApi endpoints

        /// <summary>
        /// </summary>
        /// <param name="hostName">The hostname of the VISAB WebAPi</param>
        /// <param name="port">The port of the VISAB WebApi</param>
        /// <param name="requestTimeout">The time in seconds before a request is timeouted</param>
        public VISABApi(string hostName = Default.HostName, int port = Default.Port, int requestTimeout = Default.RequestTimeout)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentException($"An empty hostname if not allowed! A valid hostname is {Default.HostName}");

            if (!hostName.StartsWith("http://"))
                throw new ArgumentException($"Hostnames have to contain the full hostname - including http://. A valid hostname is {Default.HostName}");

            if (requestTimeout < 1)
                throw new ArgumentException("Request timeout cant be negative!");

            HostName = hostName;
            Port = port;
            BaseAdress = hostName.EndsWith(":") ? HostName + Port : HostName + ":" + Port;
            RequestTimeout = requestTimeout;

            SessionIndependantRequestHandler = new VISABRequestHandler(BaseAdress, requestTimeout);
        }

        /// <summary>
        /// The current Version of the VISABConnector dll.
        /// </summary>
        public static Version ConnectorVersion => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// The full base adress of the VISAB WebApi.
        /// </summary>
        public string BaseAdress { get; }

        /// <summary>
        /// The host name on which the VISAB WebApi is running.
        /// </summary>
        public string HostName { get; }

        /// <summary>
        /// The port on which the VISAB WebApi is running.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// The time in seconds until a request is timeouted.
        /// </summary>
        public int RequestTimeout { get; }

        /// <summary>
        /// Request handler used for making Http requests that are independant of sessions.
        /// </summary>
        public IVISABRequestHandler SessionIndependantRequestHandler { get; }

        /// <summary>
        /// Starts the VISAB jar
        /// </summary>
        /// <param name="pathToVisab">The path to the jar file</param>
        public static void StartVISAB(string pathToVisab)
        {
            // TODO: Start VISAB. Can be done via setting environment variable.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indicates if the VISAB WebApi can receive data for the given game.
        /// </summary>
        /// <param name="game">The game to check</param>
        /// <returns>Content is true if game is supported, false else</returns>
        public async Task<ApiResponse<bool>> GameIsSupported(string game)
        {
            var response = await SessionIndependantRequestHandler
                                        .GetDeserializedResponseAsync<List<string>>(HttpMethod.Get, EndpointGameSupported, null, null)
                                        .ConfigureAwait(false);
            if (response.IsSuccess)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Content = response.Content.Contains(game)
                };
            }
            else
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    ErrorMessage = response.ErrorMessage
                };
            }
        }

        /// <summary>
        /// Returns a list of the sessionIds for all active sessions.
        /// </summary>
        /// <returns>A list of the sessionIds for all active sessions</returns>
        public async Task<ApiResponse<IList<Guid>>> GetActiveSessions()
        {
            return await SessionIndependantRequestHandler.GetDeserializedResponseAsync<IList<Guid>>(HttpMethod.Get, EndpointListSessions, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the session status of for a given sessionId.
        /// TODO: Should have pojo that visab has here also.
        /// </summary>
        /// <param name="sessionId">The sessionId to check</param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> GetSessionStatus(Guid sessionId)
        {
            var queryParameters = new List<string>
            {
                $"sessionid={sessionId}"
            };

            return await SessionIndependantRequestHandler.GetResponseAsync(HttpMethod.Get, EndpointSessionStatus, queryParameters, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a IVISABSession object and opens a transmission session at the VISAB WebApi.
        /// </summary>
        /// <param name="metaInformation">The meta information for the session to open</param>
        /// <returns></returns>
        public async Task<ApiResponse<IVISABSession>> InitiateSession(IMetaInformation metaInformation)
        {
            if (metaInformation == null)
                throw new ArgumentException("Meta information is required for opening a session and therefore cant be null!");

            var pingResponse = await IsApiReachable().ConfigureAwait(false);
            if (!pingResponse.IsSuccess)
            {
                return new ApiResponse<IVISABSession>
                {
                    IsSuccess = false,
                    ErrorMessage = pingResponse.ErrorMessage
                };
            }

            var gameSupportedResponse = await GameIsSupported(metaInformation.Game).ConfigureAwait(false);
            if (gameSupportedResponse.IsSuccess && !gameSupportedResponse.Content)
            {
                throw new ArgumentException($"Game {metaInformation.Game} is not supported by the VISAB WebApi!");
            }
            else if (!gameSupportedResponse.IsSuccess)
            {
                return new ApiResponse<IVISABSession>
                {
                    IsSuccess = false,
                    ErrorMessage = gameSupportedResponse.ErrorMessage
                };
            }

            // Try to open session
            var openSessionResponse = await SessionIndependantRequestHandler.
                GetDeserializedResponseAsync<IMetaInformation, Guid>(HttpMethod.Get, EndpointOpenSession, null, metaInformation).ConfigureAwait(false);
            if (openSessionResponse.IsSuccess)
            {
                // The sessionId that was returned by VISAB WebApi
                var sessionId = openSessionResponse.Content;

                // Create new request handler with the game and sessionId in default header
                var requestHandler = new VISABRequestHandler(BaseAdress, RequestTimeout);
                requestHandler.AddDefaultHeader("game", metaInformation.Game);
                requestHandler.AddDefaultHeader("sessionid", sessionId);

                var session = new VISABSession(metaInformation.Game, openSessionResponse.Content, requestHandler);

                return new ApiResponse<IVISABSession>
                {
                    IsSuccess = true,
                    Content = session
                };
            }
            else
            {
                return new ApiResponse<IVISABSession>
                {
                    IsSuccess = false,
                    ErrorMessage = openSessionResponse.ErrorMessage
                };
            }
        }

        /// <summary>
        /// Indicates if the VISAB WebApi is running. If the request was successful, it is running.
        /// </summary>
        /// <returns>An ApiResponse object</returns>
        public async Task<ApiResponse<string>> IsApiReachable()
        {
            return await SessionIndependantRequestHandler.GetResponseAsync(HttpMethod.Get, EndpointPing, null, null).ConfigureAwait(false);
        }
    }
}