using System;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Class that embodies a transmission session at the VISAB WebApi
    /// </summary>
    public interface IVISABSession
    {
        /// <summary>
        /// Event that is invoked before closing the session
        /// </summary>
        event EventHandler<ClosingEventArgs> CloseSessionEvent;

        /// <summary>
        /// The name of the game of which data will be sent
        /// </summary>
        string Game { get; }

        /// <summary>
        /// Whether the session is active at the VISAB WebApi
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// The RequestHandler used for making the Http requests
        /// </summary>
        IVISABRequestHandler RequestHandler { get; }

        /// <summary>
        /// The unique identifier for the current session
        /// </summary>
        Guid SessionId { get; }

        /// <summary>
        /// Closes the session at the VISAB WebApi
        /// </summary>
        /// <returns>True if the session was succesfully closed, false else</returns>
        Task<ApiResponse> CloseSession();

        /// <summary>
        /// Sends a map object to the VISAB WebApi
        /// </summary>
        /// <typeparam name="T">The type inheriting IUnityMap</typeparam>
        /// <param name="map">The map object of type T</param>
        /// <returns>True if the map was received by VISAB, false else</returns>
        Task<ApiResponse> SendMap<T>(T map) where T : IUnityMap;

        /// <summary>
        /// Sends a map information object to the VISAB WebApi
        /// </summary>
        /// <typeparam name="T">The type inheriting IUnityMapInformation</typeparam>
        /// <param name="mapInformation">The map information object of type T</param>
        /// <returns>True if the map information was received by VISAB, false else</returns>
        Task<ApiResponse> SendMapInformation<T>(T mapInformation) where T : IUnityMapInformation;

        /// <summary>
        /// Sends a statistics object to the VISAB WebApi
        /// </summary>
        /// <typeparam name="T">The type inheriting IVISABStatistics</typeparam>
        /// <param name="statistics">The statistics object of type T</param>
        /// <returns>True if the statistics were received by VISAB, false else</returns>
        Task<ApiResponse> SendStatistics<T>(T statistics) where T : IVISABStatistics;
    }
}