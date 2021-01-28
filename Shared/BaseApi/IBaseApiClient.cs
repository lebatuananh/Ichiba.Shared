using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.BaseApi
{
    public interface IBaseApiClient
    {
        Task<List<T>> GetListAsync<T>(string url, string clientName, string baseApiUrl,
            bool requiredLogin = false);
        Task<T> GetAsync<T>(string url, string clientName, string baseApiUrl, bool requiredLogin = false);
        Task<TResponse> PostAsync<TRequest, TResponse>(string url, string clientName, string baseApiUrl,
            TRequest requestContent,
            bool requiredLogin = true);
        Task<bool> PutAsync<TRequest, TResponse>(string url, TRequest requestContent, string clientName,
            string baseApiUrl,
            bool requiredLogin = true);
    }
}