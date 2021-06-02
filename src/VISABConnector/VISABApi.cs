using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using VISABConnector.Http;

namespace VISABConnector
{
    /// <summary>
    /// Class for initializing transmission sessions and making requests independant of sessions to the VISAB WebApi.
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
        /// The host name on which the VISAB WebApi is running.
        /// </summary>
        public string HostName { get; }

        /// <summary>
        /// The port on which the VISAB WebApi is running.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// The full base adress of the VISAB WebApi.
        /// </summary>
        public string BaseAdress { get; }

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
            SessionIndependantRequestHandler = new VISABRequestHandler(BaseAdress, null, Guid.Empty);
        }

        /// <summary>
        /// Calls VISABApi(string hostName, int port) with default host name and port.
        /// </summary>
        public VISABApi() : this(Default.HostName, Default.Port)
        {
        }

        /// <summary>
        /// Request handler used for making Http requests that are independant of sessions.
        /// </summary>
        private IVISABRequestHandler SessionIndependantRequestHandler { get; }

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

            if (supportedGames != default)
                return supportedGames.Contains(game);

            return false;
        }

        /// <summary>
        /// Returns a list of the sessionIds for all active sessions
        /// </summary>
        /// <returns>A list of the sessionIds for all active sessions</returns>
        public async Task<IList<Guid>> GetActiveSessions()
        {
            return await SessionIndependantRequestHandler.GetDeserializedResponseAsync<IList<Guid>>(HttpMethod.Get, ENDPOINT_SESSION_LIST, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the current Version of the VISABConnector
        /// </summary>
        /// <returns>The Version of the VISABConnector</returns>
        public static Version GetConnectorVersion() => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// Gets the session status of for a given sessionId
        /// </summary>
        /// <param name="sessionId">The sessionId to check</param>
        /// <returns></returns>
        public async Task<ApiResponse> GetSessionStatus(Guid sessionId)
        {
            var queryParameters = new List<string>
            {
                $"sessionid={sessionId}"
            };

            return await SessionIndependantRequestHandler.GetApiResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_STATUS, queryParameters, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a IVISABSession object, which simultaneously opens a transmission session at the
        /// VISAB WebApi
        /// </summary>
        /// <param name="game">The game of which to sent data</param>
        /// <returns>A IVISABSession object if a transmission session was openend, else null</returns>
        public async Task<IVISABSession> InitiateSession(string game)
        {
            var pingResult = await IsApiReachable().ConfigureAwait(false);
            if (!pingResult.IsSuccess)
                return default;

            if (!await GameIsSupported(game).ConfigureAwait(false))
                throw new Exception($"Game[{game}] is not supported by the VISAB Api!");

            var session = new VISABSession(BaseAdress, game, Guid.NewGuid());

            var response = await session.OpenSession().ConfigureAwait(false);
            if (response.IsSuccess)
            {
                session.IsActive = true;

                return session;
            }

            return default;
        }

        /// <summary>
        /// Indicates if the VISAB WebApi is running
        /// </summary>
        /// <returns>True if VISAB WebApi is reachable, false else</returns>
        public async Task<ApiResponse> IsApiReachable()
        {
            return await SessionIndependantRequestHandler.GetApiResponseAsync(HttpMethod.Get, ENDPOINT_PING_TEST, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts the VISAB jar
        /// </summary>
        /// <param name="pathToVisab">The path to the jar file</param>
        public static void StartVISAB(string pathToVisab)
        {
            // TODO: Start VISAB. Can be done via setting environment variable.
            throw new NotImplementedException();
        }
    }
}