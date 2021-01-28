using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Shared.BaseApi
{
    public class BaseApiClient : IBaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseApiClient(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<T>> GetListAsync<T>(string url, string clientName, string baseApiUrl,
            bool requiredLogin = false)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.BaseAddress = new Uri(baseApiUrl);
            if (requiredLogin)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            var data = (List<T>) JsonConvert.DeserializeObject(body, typeof(List<T>));
            return data;
        }

        public async Task<T> GetAsync<T>(string url, string clientName, string baseApiUrl, bool requiredLogin = false)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.BaseAddress = new Uri(baseApiUrl);
            if (requiredLogin)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<T>(body);
            return data;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, string clientName, string baseApiUrl,
            TRequest requestContent,
            bool requiredLogin = true)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.BaseAddress = new Uri(baseApiUrl);
            StringContent httpContent = null;
            if (requestContent != null)
            {
                var json = JsonConvert.SerializeObject(requestContent);
                httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            }

            if (requiredLogin)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.PostAsync(url, httpContent);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return JsonConvert.DeserializeObject<TResponse>(body);

            throw new Exception(body);
        }

        public async Task<bool> PutAsync<TRequest, TResponse>(string url, TRequest requestContent, string clientName,
            string baseApiUrl,
            bool requiredLogin = true)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.BaseAddress = new Uri(baseApiUrl);
            HttpContent httpContent = null;
            if (requestContent != null)
            {
                var json = JsonConvert.SerializeObject(requestContent);
                httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            }

            if (requiredLogin)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.PutAsync(url, httpContent);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return true;

            throw new Exception(body);
        }
    }
}