using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VISABConnector.Http
{
    /// <summary>
    /// Base class used for making Http requests. Classes making Http requests should inherit from this.
    /// </summary>
    internal abstract class RequestHandlerBase
    {
        /// <summary>
        /// The used HttpClient.
        /// </summary>
        protected readonly HttpClient _httpClient;

        /// <summary>
        /// The timeout for requests in seconds.
        /// </summary>
        protected int _requestTimeout;

        /// <summary>
        /// </summary>
        /// <param name="baseAdress">The base adress for the HttpClient</param>
        /// <param name="requestTimeout">The timeout for requests in seconds</param>
        protected RequestHandlerBase(string baseAdress, int requestTimeout)
        {
            // Fix wrong baseAdress: https://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            var _baseAdress = baseAdress.EndsWith("/") ? baseAdress : baseAdress + '/';

            _httpClient = new HttpClient { BaseAddress = new Uri(_baseAdress) };

            _requestTimeout = requestTimeout;
        }

        /// <summary>
        /// Makes a http request and gets the HttpResponseMessage object.
        /// </summary>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>The HttpResponseMessage object</returns>
        protected async Task<HttpResponseMessage> GetHttpResponseAsync(HttpMethod httpMethod, string relativeUrl, string body = null, IReadOnlyDictionary<string, string> queryParameters = null)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(_requestTimeout));

            var request = PrepareRequest(httpMethod, relativeUrl, body, queryParameters);
            try
            {
                return await _httpClient.SendAsync(request, cts.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage
                {
                    RequestMessage = request,
                    Content = new StringContent($"Request to VISAB WebApi at {_httpClient.BaseAddress} was timed out after {_requestTimeout} seconds. Likely the VISAB WebApi isnt running or the adress is incorrect. Exception:\n{e}"),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        /// <summary>
        /// Builds a parametrized url from the given query parameters.
        /// </summary>
        /// <param name="relativeUrl">The relative url without the parameters</param>
        /// <param name="queryParameters">The query parameters to add</param>
        /// <returns>A parametrized url</returns>
        protected static string BuildParameterizedUrl(string relativeUrl, IReadOnlyDictionary<string, string> queryParameters)
        {
            if (queryParameters == null)
                return relativeUrl;

            var parametrizedUrl = relativeUrl + "?";
            foreach (var pair in queryParameters)
                parametrizedUrl += $"{pair.Key}={pair.Value}&";

            return parametrizedUrl;
        }

        /// <summary>
        /// Reads the content of a HttpResponseMessage.
        /// </summary>
        /// <param name="httpResponse">The HttpResponseMessage to read the content from</param>
        /// <returns>The content as a string, empty string if content was null</returns>
        protected static async Task<string> GetResponseContentAsync(HttpResponseMessage httpResponse)
        {
            if (httpResponse.Content == null)
                return "";

            var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            return content ?? "";
        }

        /// <summary>
        /// Builds a HttpRequestMessage object.
        /// </summary>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>The built HttpRequestMessage object</returns>
        protected static HttpRequestMessage PrepareRequest(HttpMethod httpMethod, string relativeUrl, string body = null, IReadOnlyDictionary<string, string> queryParameters = null)
        {
            // Fix wrong relativeUrl: https://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            var url = relativeUrl.StartsWith("/") ? relativeUrl.Remove(0, 1) : relativeUrl;

            if (queryParameters != null)
                url = BuildParameterizedUrl(url, queryParameters);

            var request = new HttpRequestMessage(httpMethod, url);

            if (!string.IsNullOrWhiteSpace(body))
                request.Content = new StringContent(body, Default.Encoding, Default.ContentMediaType);

            return request;
        }
    }
}
