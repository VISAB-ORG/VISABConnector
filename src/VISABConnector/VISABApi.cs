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
        private const string ENDPOINT_GAME_SUPPORTED = "games";

        /// <summary>
        /// Relative endpoint for ping testing the VISAB API
        /// </summary>
        private const string ENDPOINT_PING_TEST = "ping";

        /// <summary>
        /// Relative endpoints for listing the currently active sessions in VISAB API
        /// </summary>
        private const string ENDPOINT_SESSION_LIST = "session/list";

        /// <summary>
        /// Relative endpoints for checking session status in VISAB API
        /// </summary>
        private const string ENDPOINT_SESSION_STATUS = "session/status";

        #endregion VISAB WebApi endpoints

        /// <summary>
        /// </summary>
        /// <param name="hostName">The hostname of the VISAB WebAPi</param>
        /// <param name="port">The port of the VISAB WebApi</param>
        public VISABApi(string hostName, int port)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentException($"An empty hostname if not allowed! A valid hostname is {Default.HostName}");

            if (!hostName.Contains("http"))
                throw new ArgumentException($"Hostnames have to contain the full hostname - including http://. A valid hostname is {Default.HostName}");

            HostName = hostName;
            Port = port;
            BaseAdress = hostName.EndsWith(":") ? HostName + Port : HostName + ":" + Port;
            SessionIndependantRequestHandler = new VISABRequestHandler(BaseAdress, null);
        }

        /// <summary>
        /// Calls VISABApi(string hostName, int port) with default host name and port.
        /// </summary>
        public VISABApi() : this(Default.HostName, Default.Port)
        {
        }

        /// <summary>
        /// The current Version of the VISABConnector.
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
        /// Indicates if the VISAB WebApi can receive data for the given game
        /// </summary>
        /// <param name="game">The game to check</param>
        /// <returns>True if game is supported, false else</returns>
        public async Task<bool> GameIsSupported(string game)
        {
            var supportedGames = await SessionIndependantRequestHandler
                                        .GetDeserializedResponseAsync<List<string>>(HttpMethod.Get, ENDPOINT_GAME_SUPPORTED, null, null)
                                        .ConfigureAwait(false);

            if (supportedGames.IsSuccess)
                return supportedGames.Content.Contains(game);
            else
                return false;
        }

        /// <summary>
        /// Returns a list of the sessionIds for all active sessions
        /// </summary>
        /// <returns>A list of the sessionIds for all active sessions</returns>
        public async Task<IList<Guid>> GetActiveSessions()
        {
            var response = await SessionIndependantRequestHandler.GetDeserializedResponseAsync<IList<Guid>>(HttpMethod.Get, ENDPOINT_SESSION_LIST, null, null).ConfigureAwait(false);
            if (response.IsSuccess)
                return response.Content;
            else
                return new List<Guid>();
        }

        /// <summary>
        /// Gets the session status of for a given sessionId.
        /// TODO: Should have pojo that visab has here also.
        /// </summary>
        /// <param name="sessionId">The sessionId to check</param>
        /// <returns></returns>
        public async Task<string> GetSessionStatus(Guid sessionId)
        {
            var queryParameters = new List<string>
            {
                $"sessionid={sessionId}"
            };

            var response = await SessionIndependantRequestHandler.GetResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_STATUS, queryParameters, null).ConfigureAwait(false);

            if (response.IsSuccess)
                return response.Content;
            else
                return "";
        }

        /// <summary>
        /// Creates a IVISABSession object and opens a transmission session at the VISAB WebApi.
        /// </summary>
        /// <param name="game">The game of which to sent data</param>
        /// <returns>A IVISABSession object if a transmission session was openend, else null</returns>
        public async Task<IVISABSession> InitiateSession(string game)
        {
            var pingResult = await IsApiReachable().ConfigureAwait(false);
            if (!pingResult.IsSuccess)
                return default;

            if (!await GameIsSupported(game).ConfigureAwait(false))
                throw new Exception($"Game {game} is not supported by the VISAB WebApi!");

            var session = new VISABSession(BaseAdress, game);

            var sessionOpened = await session.OpenSession().ConfigureAwait(false);
            if (sessionOpened)
                return session;
            else
                return null;
        }

        /// <summary>
        /// Indicates if the VISAB WebApi is running. If the request was successful, it is running.
        /// </summary>
        /// <returns>An ApiResponse object</returns>
        public async Task<ApiResponse<string>> IsApiReachable()
        {
            return await SessionIndependantRequestHandler.GetResponseAsync(HttpMethod.Get, ENDPOINT_PING_TEST, null, null).ConfigureAwait(false);
        }
    }
}