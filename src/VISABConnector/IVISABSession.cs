using System;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Class that provides access the functionality provided by a transmission session at the VISAB WebApi.
    /// </summary>
    public interface IVISABSession
    {
        /// <summary>
        /// Event that is invoked before closing the session.
        /// </summary>
        event EventHandler<ClosingEventArgs> CloseSessionEvent;

        /// <summary>
        /// The name of the game of which data will be sent.
        /// </summary>
        string Game { get; }

        /// <summary>
        /// Whether the session is active at the VISAB WebApi.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// The RequestHandler used for making the Http requests.
        /// </summary>
        IVISABRequestHandler RequestHandler { get; }

        /// <summary>
        /// The unique identifier for the current session.
        /// </summary>
        Guid SessionId { get; }

        /// <summary>
        /// Closes the session at the VISAB WebApi.
        /// </summary>
        /// <returns>An ApiResponse object</returns>
        Task<ApiResponse<string>> CloseSession();

        /// <summary>
        /// Sends a image object to the VISAB WebApi.
        /// </summary>
        /// <typeparam name="T">The type inheriting IImage</typeparam>
        /// <param name="map">The image object of type T</param>
        /// <returns>An ApiResponse object</returns>
        Task<ApiResponse<string>> SendMap<T>(T map) where T : IImage;

        /// <summary>
        /// Sends a statistics object to the VISAB WebApi.
        /// </summary>
        /// <typeparam name="T">The type inheriting IVISABStatistics</typeparam>
        /// <param name="statistics">The statistics object of type T</param>
        /// <returns>An ApiResponse object</returns>
        Task<ApiResponse<string>> SendStatistics<T>(T statistics) where T : IVISABStatistics;
    }
}