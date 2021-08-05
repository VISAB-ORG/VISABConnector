using System;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Interface that represents a transmission session at the VISAB WebApi and provides functionality for it.    
    /// </summary>
    public interface IVISABSession
    {
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
        /// Queries the file that was created by VISAB for the session. Only works if the session
        /// was closed before.
        /// </summary>
        /// <returns>An ApiResponse object with the file as a json string in the content</returns>
        Task<ApiResponse<string>> GetCreatedFile();

        /// <summary>
        /// Sends a image object to the VISAB WebApi.
        /// </summary>
        /// <param name="image">The image object of type T</param>
        /// <returns>An ApiResponse object</returns>
        Task<ApiResponse<string>> SendImage(IImageContainer image);

        /// <summary>
        /// Sends a statistics object to the VISAB WebApi.
        /// </summary>
        /// <param name="statistics">The statistics object of type T</param>
        /// <returns>An ApiResponse object</returns>
        Task<ApiResponse<string>> SendStatistics(IStatistics statistics);
    }
}