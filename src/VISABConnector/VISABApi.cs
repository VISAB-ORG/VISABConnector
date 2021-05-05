using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VISABConnector.Http;

namespace VISABConnector
{
    /// <summary>
    /// Class for initializing transmission sessions and making static requests to the VISAB WebApi
    /// </summary>
    public static class VISABApi
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
        /// Request handler used for making Http requests
        /// </summary>
        private static readonly IVISABRequestHandler requestHandler = new VISABRequestHandler(null, Guid.Empty);

        /// <summary>
        /// Indicates if the VISAB WebApi can receive data for the given game
        /// </summary>
        /// <param name="game">The game to check</param>
        /// <returns>True if game is supported, false else</returns>
        public static async Task<bool> GameIsSupported(string game)
        {
            var supportedGames = await requestHandler
                                        .GetDeserializedResponseAsync<List<string>>(HttpMethod.Get, ENDPOINT_GAME_SUPPORTED, null, null)
                                        .ConfigureAwait(false);

            if (supportedGames != default)
                return supportedGames.Contains(game);

            return false;
        }

        /// <summary>
        /// Gets the session status of for a given sessionId
        /// </summary>
        /// <param name="sessionId">The sessionId to check</param>
        /// <returns></returns>
        public static async Task<ApiResponse> GetSessionStatus(Guid sessionId)
        {
            var queryParameters = new List<string>
            {
                $"sessionid={sessionId}"
            };

            return await requestHandler.GetApiResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_STATUS, queryParameters, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a list of the sessionIds for all active sessions
        /// </summary>
        /// <returns>A list of the sessionIds for all active sessions</returns>
        public static async Task<IList<Guid>> GetActiveSessions()
        {
            return await requestHandler.GetDeserializedResponseAsync<IList<Guid>>(HttpMethod.Get, ENDPOINT_SESSION_LIST, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a IVISABSession object, which simultaneously opens a transmission session at the
        /// VISAB WebApi
        /// </summary>
        /// <param name="game">The game of which to sent data</param>
        /// <returns>A IVISABSession object if a transmission session was openend, else null</returns>
        public static async Task<IVISABSession> InitiateSession(string game)
        {
            var pingResult = await IsApiReachable().ConfigureAwait(false);
            if (!pingResult.IsSuccess)
                return default;

            if (!await GameIsSupported(game).ConfigureAwait(false))
                throw new Exception($"Game[{game}] is not supported by the VISAB Api!");

            var session = new VISABSession(game, Guid.NewGuid());

            var response = await session.OpenSession().ConfigureAwait(false);
            if (response.IsSuccess) {
                session.IsActive = true;

                return session;
            }

            return default;
        }

        /// <summary>
        /// Indicates if the VISAB WebApi is running
        /// </summary>
        /// <returns>True if VISAB WebApi is reachable, false else</returns>
        public static async Task<ApiResponse> IsApiReachable()
        {
            return await requestHandler.GetApiResponseAsync(HttpMethod.Get, ENDPOINT_PING_TEST, null, null).ConfigureAwait(false);
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