using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shared.Common;

namespace Shared.ResilientHttp
{
    public class StandardHttpClient : IHttpClient
    {
        private readonly HttpClient _client;
        private ILogger<StandardHttpClient> _logger;

        public StandardHttpClient(ILogger<StandardHttpClient> logger)
        {
            _client = HttpClientConnection.Current.GetConnection;
            _logger = logger;
        }

        public async Task<string> GetStringAsync(string uri, string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            if (authorizationToken != null)
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

            var response = await _client.SendAsync(requestMessage);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            return await DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public async Task<HttpResponseMessage> PostAsync(string uri, MultipartFormDataContent content)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> PostAsync(string uri, string content)
        {
            return await DoPostPutAsync(HttpMethod.Post, uri, content);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            return await DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            if (authorizationToken != null)
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

            if (requestId != null) requestMessage.Headers.Add("x-requestid", requestId);

            return await _client.SendAsync(requestMessage);
        }

        private async Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method, string uri, string item)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
                throw new ArgumentException("Value must be either post or put.", nameof(method));

            // a new StringContent must be created for each retry
            // as it is disposed after each call

            var requestMessage = new HttpRequestMessage(method, uri);

            requestMessage.Content =
                new StringContent(Serialize.JsonSerializeObject(item), Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(requestMessage);

            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError) throw new HttpRequestException();

            return response;
        }

        private async Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item,
            string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
                throw new ArgumentException("Value must be either post or put.", nameof(method));

            // a new StringContent must be created for each retry
            // as it is disposed after each call

            var requestMessage = new HttpRequestMessage(method, uri);

            requestMessage.Content =
                new StringContent(Serialize.JsonSerializeObject(item), Encoding.UTF8, "application/json");

            if (authorizationToken != null)
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

            if (requestId != null) requestMessage.Headers.Add("x-requestid", requestId);

            var response = await _client.SendAsync(requestMessage);

            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError) throw new HttpRequestException();

            return response;
        }
    }
}