using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace VISABConnector.Http
{
    ///<inheritdoc cref="IVISABRequestHandler"/>
    internal class VISABRequestHandler : RequestHandlerBase, IVISABRequestHandler
    {
        private readonly JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// </summary>
        /// <param name="baseAdress">The base adress of the VISAB WebApi</param>
        /// <param name="requestTimeout">The time in seconds until requests are timeouted</param>
        public VISABRequestHandler(string baseAdress, int requestTimeout) : base(baseAdress, requestTimeout)
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new IgnorePropertyContractResolver<DontSerialize>(),
                Formatting = Formatting.Indented
            };
        }

        public void AddDefaultHeader(string name, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }

        public async Task<ApiResponse<TResponse>> GetDeserializedResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, string body = null, IReadOnlyDictionary<string, string> queryParameters = null)
        {
            var response = await GetResponseAsync(httpMethod, relativeUrl, body, queryParameters).ConfigureAwait(false);

            if (response.IsSuccess)
            {
                return new ApiResponse<TResponse>
                {
                    IsSuccess = true,
                    Content = await Task.Run(() => JsonConvert.DeserializeObject<TResponse>(response.Content)).ConfigureAwait(false)
                };
            }

            return new ApiResponse<TResponse>
            {
                IsSuccess = false,
                ErrorMessage = response.ErrorMessage
            };
        }

        public async Task<ApiResponse<TResponse>> GetDeserializedResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, object body, IReadOnlyDictionary<string, string> queryParameters = null)
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(body, _serializerSettings)).ConfigureAwait(false);

            return await GetDeserializedResponseAsync<TResponse>(httpMethod, relativeUrl, json, queryParameters).ConfigureAwait(false);
        }

        public async Task<ApiResponse<string>> GetResponseAsync(HttpMethod httpMethod, string relativeUrl, string body = null, IReadOnlyDictionary<string, string> queryParameters = null)
        {
            var response = await GetHttpResponseAsync(httpMethod, relativeUrl, body, queryParameters).ConfigureAwait(false);
            var content = await GetResponseContentAsync(response).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<string>
                {
                    IsSuccess = true,
                    Content = content
                };
            }

            return new ApiResponse<string>
            {
                IsSuccess = false,
                ErrorMessage = content
            };
        }

        public async Task<ApiResponse<string>> GetResponseAsync(HttpMethod httpMethod, string relativeUrl, object body, IReadOnlyDictionary<string, string> queryParameters = null)
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(body, _serializerSettings)).ConfigureAwait(false);

            return await GetResponseAsync(httpMethod, relativeUrl, json, queryParameters).ConfigureAwait(false);
        }
    }
}