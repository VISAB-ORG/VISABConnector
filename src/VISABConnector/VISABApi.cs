using System;
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
        private const string _endpointGameSupported = "games";

        /// <summary>
        /// Relative endpoints for listing the currently active sessions in VISAB API
        /// </summary>
        private const string _endpointListSessions = "session/list";

        /// <summary>
        /// Relative endpoint for opening session to VISAB API
        /// </summary>
        private const string _endpointOpenSession = "session/open";

        /// <summary>
        /// Relative endpoint for ping testing the VISAB API
        /// </summary>
        private const string _endpointPing = "ping";

        /// <summary>
        /// Relative endpoints for checking session status in VISAB API
        /// </summary>
        private const string _endpointSessionStatus = "session/status";

        #endregion VISAB WebApi endpoints

        /// <summary>
        /// </summary>
        /// <param name="hostAdress">The hostadress of the VISAB WebAPi</param>
        /// <param name="port">The port of the VISAB WebApi</param>
        /// <param name="requestTimeout">The time in seconds before a request is timeouted</param>
        public VISABApi(string hostAdress = Default.HostAdress, int port = Default.Port, int requestTimeout = Default.RequestTimeout)
        {
            if (string.IsNullOrWhiteSpace(hostAdress))
                throw new ArgumentException($"An empty hostAdress if not allowed! A valid hostAdress is {Default.HostAdress}");

            if (!hostAdress.StartsWith("http://"))
                throw new ArgumentException($"HostAdresses have to contain the full adress - including http://. A valid hostAdress is {Default.HostAdress}");

            HostAdress = hostAdress;
            RequestTimeout = requestTimeout >= 1 ? requestTimeout : throw new ArgumentOutOfRangeException("Request timeout cant be negative!");
            Port = port;
            BaseAdress = hostAdress.EndsWith(":") ? HostAdress + Port : HostAdress + ":" + Port;
            RequestTimeout = requestTimeout;

            SessionIndependantRequestHandler = new VISABRequestHandler(BaseAdress, requestTimeout);
        }

        /// <summary>
        /// The current assembly version of the VISABConnector dll.
        /// </summary>
        public static Version ConnectorVersion => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// The full base adress of the VISAB WebApi.
        /// </summary>
        public string BaseAdress { get; }

        /// <summary>
        /// The host adress on which the VISAB WebApi is running.
        /// </summary>
        public string HostAdress { get; }

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
        /// Checks if the VISAB WebApi can receive data for the given game.
        /// </summary>
        /// <param name="game">The game to check</param>
        /// <returns>An ApiResponse object with Content set to true if game is supported</returns>
        public async Task<ApiResponse<bool>> GameIsSupported(string game)
        {
            var response = await SessionIndependantRequestHandler.GetDeserializedResponseAsync<List<string>>(HttpMethod.Get, _endpointGameSupported, null, null)
                                                                 .ConfigureAwait(false);
            if (response.IsSuccess)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Content = response.Content.Contains(game)
                };
            }

            return new ApiResponse<bool>
            {
                IsSuccess = false,
                ErrorMessage = response.ErrorMessage
            };
        }

        /// <summary>
        /// Returns a list of all the sessionIds for all active sessions.
        /// </summary>
        /// <returns>
        /// An ApiResponse object whose content contains a list of the sessionIds for all active
        /// sessions if successful
        /// </returns>
        public async Task<ApiResponse<IList<Guid>>> GetActiveSessions()
        {
            return await SessionIndependantRequestHandler.GetDeserializedResponseAsync<IList<Guid>>(HttpMethod.Get, _endpointListSessions, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the session status of for a given sessionId.
        /// </summary>
        /// <param name="sessionId">The sessionId to get the status for</param>
        /// <returns>An ApiResponse object containing a SessionStatus object as Content if succesful</returns>
        public async Task<ApiResponse<SessionStatus>> GetSessionStatus(Guid sessionId)
        {
            var queryParameters = new Dictionary<string, string> { { "sessionid", sessionId.ToString() } };

            return await SessionIndependantRequestHandler.GetDeserializedResponseAsync<SessionStatus>(HttpMethod.Get, _endpointSessionStatus, null, queryParameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a IVISABSession object by opening a transmission session at the VISAB WebApi.
        /// </summary>
        /// <param name="metaInformation">The meta information for the session to open</param>
        /// <returns>An ApiResponse object containing a IVISABSession if session was created</returns>
        public async Task<ApiResponse<IVISABSession>> InitiateSession(IMetaInformation metaInformation)
        {
            if (metaInformation == null)
                throw new ArgumentNullException(nameof(metaInformation));

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
            var openSessionResponse = await SessionIndependantRequestHandler.GetDeserializedResponseAsync<Guid>(HttpMethod.Post, _endpointOpenSession, metaInformation, null)
                                                                            .ConfigureAwait(false);
            if (openSessionResponse.IsSuccess)
            {
                // The sessionId that was returned by VISAB WebApi
                var sessionId = openSessionResponse.Content;

                // Create new request handler with the game and sessionId in default header
                var requestHandler = new VISABRequestHandler(BaseAdress, RequestTimeout);
                requestHandler.AddDefaultHeader("game", metaInformation.Game);
                requestHandler.AddDefaultHeader("sessionid", sessionId.ToString());

                var session = new VISABSession(metaInformation.Game, openSessionResponse.Content, requestHandler);

                return new ApiResponse<IVISABSession>
                {
                    IsSuccess = true,
                    Content = session
                };
            }

            return new ApiResponse<IVISABSession>
            {
                IsSuccess = false,
                ErrorMessage = openSessionResponse.ErrorMessage
            };
        }

        /// <summary>
        /// Indicates if the VISAB WebApi is running. If the request was successful, it is running.
        /// </summary>
        /// <returns>
        /// An ApiResponse object whose content contains the response message if request was successful
        /// </returns>
        public async Task<ApiResponse<string>> IsApiReachable()
        {
            return await SessionIndependantRequestHandler.GetResponseAsync(HttpMethod.Get, _endpointPing, null, null).ConfigureAwait(false);
        }
    }
}