﻿using Newtonsoft.Json;
using System;
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
        /// Initializes a VISABRequestHandler object
        /// </summary>
        /// <param name="gameHeader">The game for which information will be sent</param>
        /// <param name="sessionIdHeader">The sessionId of the current tranmission session</param>
        public VISABRequestHandler(string gameHeader, Guid sessionIdHeader) : base(Default.VISABBaseAdress)
        {
            httpClient.DefaultRequestHeaders.Add("game", gameHeader);
            httpClient.DefaultRequestHeaders.Add("sessionid", sessionIdHeader.ToString());

            serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new IgnorePropertyContractResolver<DontSerialize>(),
                Formatting = Formatting.Indented
            };
        }

        ///<inheritdoc/>
        public async Task<ApiResponse> GetApiResponseAsync<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body)
        {
            var json = JsonConvert.SerializeObject(body);

            return await GetApiResponseAsync(httpMethod, relativeUrl, queryParameters, json).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<ApiResponse> GetApiResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body)
        {
            var response = await GetResponseAsync(httpMethod, relativeUrl, queryParameters, body).ConfigureAwait(false);
            var message = await GetResponseContentAsync(response).ConfigureAwait(false);

            return new ApiResponse
            {
                IsSuccess = response.IsSuccessStatusCode,
                Message = message
            };
        }

        ///<inheritdoc/>
        public async Task<TResponse> GetDeserializedResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null, string body = null)
        {
            var jsonResponse = await GetJsonResponseAsync(httpMethod, relativeUrl, queryParameters, body).ConfigureAwait(false);

            if (jsonResponse != "")
                return JsonConvert.DeserializeObject<TResponse>(jsonResponse);

            return default;
        }

        ///<inheritdoc/>
        public async Task<TResponse> GetDeserializedResponseAsync<TBody, TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body)
        {
            var json = JsonConvert.SerializeObject(body, serializerSettings);

            return await GetDeserializedResponseAsync<TResponse>(httpMethod, relativeUrl, queryParameters, json).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<string> GetJsonResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body)
        {
            var response = await GetResponseAsync(httpMethod, relativeUrl, queryParameters, body).ConfigureAwait(false);

            return await GetResponseContentAsync(response).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<string> GetJsonResponseAsync<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body)
        {
            var json = JsonConvert.SerializeObject(body, serializerSettings);

            return await GetJsonResponseAsync(httpMethod, relativeUrl, queryParameters, json).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<bool> GetSuccessResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body)
        {
            var response = await GetResponseAsync(httpMethod, relativeUrl, queryParameters, body).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        ///<inheritdoc/>
        public async Task<bool> GetSuccessResponseAsync<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body)
        {
            var json = JsonConvert.SerializeObject(body, serializerSettings);

            return await GetSuccessResponseAsync(httpMethod, relativeUrl, queryParameters, json).ConfigureAwait(false);
        }
    }
}