using System;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// A wrapper class for communicating with the VISAB WebApi in round based games.
    /// </summary>
    public static class RoundBasedSession
    {
        /// <summary>
        /// The underylying session that is used.
        /// </summary>
        public static IVISABSession Session { get; private set; }

        /// <summary>
        /// Event that will be invoked when a log message should be written.
        /// The argument contains the message.
        /// </summary>
        public static event Action<string> MessageAddedEvent;

        /// <summary>
        /// Closes the active session.
        /// </summary>
        /// <returns>True if session was closed</returns>
        public static async Task<bool> CloseSessionAsync()
        {
            if (Session == null || !Session.IsActive)
            {
                WriteLog("Session is null or inactive. Wont have to close it.");
                return true;
            }

            // Close the VISAB api session
            WriteLog($"Closing session with VISAB WebApi.\nSessionId: {Session.SessionId}");

            var response = await Session.CloseSession().ConfigureAwait(false);
            if (response.IsSuccess)
                WriteLog("Closed session at VISAB WebApi.");
            else
                WriteLog($"Failed to close session at VISAB WebApi.\nErrorMessage: {response.ErrorMessage}");

            return response.IsSuccess;
        }

        /// <summary>
        /// Receives the written json file for the session.
        /// </summary>
        /// <returns>The written json file if successfully retrieved, empty string else</returns>
        public static async Task<string> GetFileAsync()
        {
            if (Session == null)
            {
                WriteLog("Session is null. Cant get file.");
                return "";
            }

            WriteLog("Making request to receive file from VISAB WebApi.");
            var response = await Session.GetCreatedFile().ConfigureAwait(false);
            if (response.IsSuccess)
                WriteLog($"Received file from VISAB WebApi.\n {response.Content}");
            else
                WriteLog($"Failed to receive file from VISAB WebApi.\nErrorMessage: {response.ErrorMessage}");

            return response.Content;
        }

        /// <summary>
        /// Sends images using the current session.
        /// </summary>
        /// <param name="image">The image to send</param>
        /// <returns>True if successfully sent</returns>
        public static async Task<bool> SendImagesAsync(IImage image)
        {
            if (Session == null || !Session.IsActive)
            {
                WriteLog("Session was null or inactive. Wont send images.");
                return false;
            }

            var response = await Session.SendImage(image).ConfigureAwait(false);
            if (response.IsSuccess)
                WriteLog($"Send images to VISAB!");
            else
                WriteLog($"Failed to send images to VISAB.\nError message: {response.ErrorMessage}");

            return response.IsSuccess;
        }

        /// <summary>
        /// Sends statistics using the current session.
        /// </summary>
        /// <param name="statistics">The statistics to send</param>
        /// <returns>True if successfully sent</returns>
        public static async Task<bool> SendStatisticsAsync(IStatistics statistics)
        {
            if (Session == null || !Session.IsActive)
            {
                WriteLog("Session was null or inactive. Wont send statistics.");
                return false;
            }

            var response = await Session.SendStatistics(statistics).ConfigureAwait(false);
            if (response.IsSuccess)
                WriteLog($"Send statistics to VISAB!");
            else
                WriteLog($"Failed to send statistics to VISAB.\nError message: {response.ErrorMessage}");

            return response.IsSuccess;
        }

        /// <summary>
        /// Starts a VISAB WebApi session.
        /// </summary>
        /// <param name="metaInformation">The metainformation with which to initialize the session</param>
        /// <param name="hostAdress">The hostAdress of the VISAB WebApi</param>
        /// <param name="port">The port of the VISAB WebApi</param>
        /// <param name="requestTimeout">The request timeout to use</param>
        /// <returns>True if session was started</returns>
        public static async Task<bool> StartSessionAsync(IMetaInformation metaInformation, string hostAdress = Default.HostAdress, int port = Default.Port, int requestTimeout = Default.RequestTimeout)
        {
            WriteLog($"Instantiating VISABApi with hostAdress: {hostAdress}, port: {port}, requestTimeout: {requestTimeout}.");
            var visabApi = new VISABApi(hostAdress, port, requestTimeout);

            WriteLog("Starting to initalize Session with VISAB WebApi.");
            // Initializes the VISAB transmission session
            var response = await visabApi.InitiateSession(metaInformation).ConfigureAwait(false);
            if (response.IsSuccess)
            {
                Session = response.Content;
                WriteLog($"Initialized Session with VISAB WebApi.\nSessionId: {Session.SessionId}");

                return true;
            }

            WriteLog($"Failed to initiate session with VISAB WebApi.\nErrorMessage: {response.ErrorMessage}");

            return false;
        }

        /// <summary>
        /// Starts a VISAB WebApi session.
        /// </summary>
        /// <param name="game">The game for which to start the session</param>
        /// <param name="hostAdress">The hostAdress of the VISAB WebApi</param>
        /// <param name="port">The port of the VISAB WebApi</param>
        /// <param name="requestTimeout">The request timeout to use</param>
        /// <returns>True if session was started</returns>
        public static async Task<bool> StartSessionAsync(string game, string hostAdress = Default.HostAdress, int port = Default.Port, int requestTimeout = Default.RequestTimeout)
        {
            var metaInformation = new BasicMetaInformation { Game = game };

            return await StartSessionAsync(metaInformation, hostAdress, port, requestTimeout).ConfigureAwait(false);
        }

        private static void WriteLog(string message)
        {
            MessageAddedEvent?.Invoke($"[RoundBasedSession]:{message}");
        }
    }
}