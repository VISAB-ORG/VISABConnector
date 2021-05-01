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
    public static class VISABConnector
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
        private static readonly IVISABRequestHandler staticRequestHandler = new VISABRequestHandler(null, Guid.Empty);

        /// <summary>
        /// Indicates if the VISAB WebApi can receive data for the given game
        /// </summary>
        /// <param name="game">The game to check</param>
        /// <returns>True if game is supported, false else</returns>
        public static async Task<bool> GameIsSupported(string game)
        {
            var supportedGames = await staticRequestHandler.GetDeserializedResponseAsync<List<string>>(HttpMethod.Get, ENDPOINT_GAME_SUPPORTED, null, null).ConfigureAwait(false);

            if (supportedGames != default)
                return await Task.Run(() => supportedGames.Contains(game)).ConfigureAwait(false);

            return false;
        }

        /// <summary>
        /// Creates a IVISABSession object, which simultaneously opens a transmission session at the
        /// VISAB WebApi
        /// </summary>
        /// <param name="game">The game of which to sent data</param>
        /// <returns>A IVISABSession object if a transmission session was openend, else null</returns>
        public static async Task<IVISABSession> InitiateSession(string game)
        {
            if (!await IsApiReachable().ConfigureAwait(false))
                return default;

            var conn = new VISABSession(game, Guid.NewGuid());
            if (await GameIsSupported(game).ConfigureAwait(false)
                && await conn.OpenSession().ConfigureAwait(false))
            {
                conn.IsActive = true;
                return conn;
            }

            if (!await GameIsSupported(game).ConfigureAwait(false))
                throw new Exception($"Game[{game}] is not supported by the VISAB Api!");

            return default;
        }

        /// <summary>
        /// Indicates if the VISAB WebApi is running
        /// </summary>
        /// <returns>True if VISAB WebApi is reachable, false else</returns>
        public static async Task<bool> IsApiReachable()
        {
            return await staticRequestHandler.GetSuccessResponseAsync(HttpMethod.Get, ENDPOINT_PING_TEST, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts the VISAB jar
        /// </summary>
        /// <param name="pathToVisab">The path to the jar file</param>
        public static void StartVISAB(string pathToVisab)
        {
            // TODO: Start VISAB
            throw new NotImplementedException();
        }
    }
}