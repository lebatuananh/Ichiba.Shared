using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using Shared.Common;

namespace Shared.ResilientHttp
{
    /// <summary>
    ///     HttpClient wrapper that integrates Retry and Circuit
    ///     breaker policies when invoking HTTP services.
    ///     Based on Polly library: https://github.com/App-vNext/Polly
    /// </summary>
    public class ResilientHttpClient : IHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly Func<string, IEnumerable<IAsyncPolicy>> _policyCreator;
        private readonly ConcurrentDictionary<string, PolicyWrap> _policyWrappers;

        public ResilientHttpClient(Func<string, IEnumerable<Policy>> policyCreator, ILogger<ResilientHttpClient> logger)
        {
            _client = HttpClientConnection.Current.GetConnection;
            _logger = logger;
            _policyCreator = policyCreator;
            _policyWrappers = new ConcurrentDictionary<string, PolicyWrap>();
        }

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public Task<HttpResponseMessage> PostAsync(string uri, MultipartFormDataContent content)
        {
            return DoPostPutAsync(HttpMethod.Post, uri, content);
        }

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item)
        {
            return DoPostPutAsync(HttpMethod.Post, uri, item);
        }

        public Task<HttpResponseMessage> PostAsync(string uri, string content)
        {
            return DoPostPutAsync(HttpMethod.Post, uri, content);
        }


        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

                if (!string.IsNullOrWhiteSpace(authorizationToken)
                    && !string.IsNullOrWhiteSpace(authorizationMethod))
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                else if (!string.IsNullOrWhiteSpace(authorizationToken))
                    requestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationToken);

                if (requestId != null) requestMessage.Headers.Add("x-requestid", requestId);

                return await _client.SendAsync(requestMessage);
            });
        }

        public Task<string> GetStringAsync(string uri, string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                if (!string.IsNullOrWhiteSpace(authorizationToken)
                    && !string.IsNullOrWhiteSpace(authorizationMethod))
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                else if (!string.IsNullOrWhiteSpace(authorizationToken))
                    requestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationToken);

                var response = await _client.SendAsync(requestMessage);

                // raise exception if HttpResponseCode 500 
                // needed for circuit breaker to track fails

                if (response.StatusCode == HttpStatusCode.InternalServerError) throw new HttpRequestException();

                return await response.Content.ReadAsStringAsync();
            });
        }

        private Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method, string uri, string item)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(method, uri)
                {
                    Content = new StringContent(item, Encoding.UTF8, "application/json")
                };

                var response = await _client.SendAsync(requestMessage);

                // raise exception if HttpResponseCode 500 
                // needed for circuit breaker to track fails

                if (response.StatusCode == HttpStatusCode.InternalServerError) throw new HttpRequestException();

                return response;
            });
        }

        private Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item,
            string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            try
            {
                if (method != HttpMethod.Post && method != HttpMethod.Put)
                    throw new ArgumentException("Value must be either post or put.", nameof(method));
                // a new StringContent must be created for each retry 
                // as it is disposed after each call
                var origin = GetOriginFromUri(uri);

                return HttpInvoker(origin, async () =>
                {
                    var requestMessage = new HttpRequestMessage(method, uri)
                    {
                        Content = new StringContent(Serialize.JsonSerializeObject(item), Encoding.UTF8,
                            "application/json")
                    };

                    if (!string.IsNullOrWhiteSpace(authorizationToken)
                        && !string.IsNullOrWhiteSpace(authorizationMethod))
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                    else if (!string.IsNullOrWhiteSpace(authorizationToken))
                        requestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationToken);

                    if (requestId != null) requestMessage.Headers.Add("x-requestid", requestId);

                    var response = await _client.SendAsync(requestMessage);

                    // raise exception if HttpResponseCode 500 
                    // needed for circuit breaker to track fails

                    if (response.StatusCode == HttpStatusCode.InternalServerError) throw new HttpRequestException();

                    return response;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method, string uri,
            MultipartFormDataContent content)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(method, uri)
                {
                    Content = content
                };
                var response = await _client.SendAsync(requestMessage);
                if (response.StatusCode == HttpStatusCode.InternalServerError) throw new HttpRequestException();

                return response;
            });
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out var policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            // Executes the action applying all 
            // the policies defined in the wrapper
            //return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin));
            return await policyWrap.ExecuteAsync(action);
        }

        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);

            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }
    }
}