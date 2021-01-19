using System.Net.Http;
using System.Threading.Tasks;

namespace Shared.ResilientHttp
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string uri, string authorizationToken = null,
            string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PostAsync(string uri, MultipartFormDataContent content);
        Task<HttpResponseMessage> PostAsync<T>(string uri, T item);

        Task<HttpResponseMessage> PostAsync(string uri, string content);

        Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null,
            string authorizationMethod = "Bearer");

        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearer");
    }
}