using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Used for making Http requests to the VISAB WebApi.
    /// </summary>
    public interface IVISABRequestHandler
    {
        /// <summary>
        /// Adds an attribute value pair to the default request header.
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <param name="value">The value of the attribute</param>
        void AddDefaultHeader(string name, string value);

        /// <summary>
        /// Makes a request to the VISAB WebApi and deserialiazes the responses content.
        /// </summary>
        /// <typeparam name="TResponse">The type to deserialize into</typeparam>
        /// <param name="httpMethod">The HttpMethod used for the request</param>
        /// <param name="relativeUrl">The relative url for the request</param>
        /// <param name="queryParameters">The query parameters for the request</param>
        /// <param name="body">The body of the request</param>
        /// <returns>
        /// A ApiResponse object with the deserialized content if sucessful. Otherwise the content
        /// has the default value of TResponse.
        /// </returns>
        Task<ApiResponse<TResponse>> GetDeserializedResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, string body = null, IReadOnlyDictionary<string, string> queryParameters = null);

        /// <summary>
        /// Makes a request to the VISAB WebApi and deserialiazes the responses content. The given
        /// body object is serialized into a json string before sending.
        /// </summary>
        /// <typeparam name="TResponse">The type to deserialize into</typeparam>
        /// <param name="httpMethod">The HttpMethod used for the request</param>
        /// <param name="relativeUrl">The relative url for the request</param>
        /// <param name="queryParameters">The query parameters for the request</param>
        /// <param name="body">The body of the request</param>
        /// <returns>
        /// A ApiResponse object with the deserialized content if sucessful. Otherwise the content
        /// has the default value of TResponse.
        /// </returns>
        Task<ApiResponse<TResponse>> GetDeserializedResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, object body, IReadOnlyDictionary<string, string> queryParameters = null);

        /// <summary>
        /// Makes a request to the VISAB WebApi.
        /// </summary>
        /// <param name="httpMethod">The HttpMethod used for the request</param>
        /// <param name="relativeUrl">The relative url for the request</param>
        /// <param name="queryParameters">The query parameters for the request</param>
        /// <param name="body">The body of the request</param>
        /// <returns>
        /// A ApiResponse object with set content if successful. Content is null if not successful.
        /// </returns>
        Task<ApiResponse<string>> GetResponseAsync(HttpMethod httpMethod, string relativeUrl, string body = null, IReadOnlyDictionary<string, string> queryParameters = null);

        /// <summary>
        /// Makes a request to the VISAB WebApi. The given body object is serialized into a json
        /// string before sending.
        /// </summary>
        /// <param name="httpMethod">The HttpMethod used for the request</param>
        /// <param name="relativeUrl">The relative url for the request</param>
        /// <param name="queryParameters">The query parameters for the request</param>
        /// <param name="body">The body of the request</param>
        /// <returns>
        /// A ApiResponse object with set content if successful. Content is null if not successful.
        /// </returns>
        Task<ApiResponse<string>> GetResponseAsync(HttpMethod httpMethod, string relativeUrl, object body, IReadOnlyDictionary<string, string> queryParameters = null);
    }
}