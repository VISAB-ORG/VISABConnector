using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace VISABConnector.Http
{
    ///<inheritdoc cref="IVISABRequestHandler"/>
    internal class VISABRequestHandler : RequestHandlerBase, IVISABRequestHandler
    {
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Initializes a VISABRequestHandler object.
        /// </summary>
        /// <param name="baseAdress">The base adress of the VISAB WebApi</param>
        /// <param name="requestTimeout">The time in seconds until requests are timeouted</param>
        public VISABRequestHandler(string baseAdress, int requestTimeout) : base(baseAdress, requestTimeout)
        {
            serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new IgnorePropertyContractResolver<DontSerialize>(),
                Formatting = Formatting.Indented
            };
        }

        public void AddDefaultHeader(string name, object value)
        {
            httpClient.DefaultRequestHeaders.Add(name, value.ToString());
        }

        public async Task<ApiResponse<TResponse>> GetDeserializedResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null, string body = null)
        {
            var response = await GetResponseAsync(httpMethod, relativeUrl, queryParameters, body).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                return new ApiResponse<TResponse>
                {
                    IsSuccess = true,
                    Content = await Task.Run(() => JsonConvert.DeserializeObject<TResponse>(response.Content)).ConfigureAwait(false)
                };
            }
            else
            {
                return new ApiResponse<TResponse>
                {
                    IsSuccess = false,
                    ErrorMessage = response.ErrorMessage
                };
            }
        }

        public async Task<ApiResponse<TResponse>> GetDeserializedResponseAsync<TBody, TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body)
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(body, serializerSettings)).ConfigureAwait(false);

            return await GetDeserializedResponseAsync<TResponse>(httpMethod, relativeUrl, queryParameters, json).ConfigureAwait(false);
        }

        public async Task<ApiResponse<string>> GetResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body)
        {
            var response = await GetHttpResponseAsync(httpMethod, relativeUrl, queryParameters, body).ConfigureAwait(false);
            var content = await GetResponseContentAsync(response).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<string>
                {
                    IsSuccess = true,
                    Content = content
                };
            }
            else
            {
                return new ApiResponse<string>
                {
                    IsSuccess = false,
                    ErrorMessage = content
                };
            }
        }

        public async Task<ApiResponse<string>> GetResponseAsync<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body)
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(body, serializerSettings)).ConfigureAwait(false);

            return await GetResponseAsync(httpMethod, relativeUrl, queryParameters, json).ConfigureAwait(false);
        }
    }
}