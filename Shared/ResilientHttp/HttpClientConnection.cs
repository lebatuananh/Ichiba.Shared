using System.Net.Http;

namespace Shared.ResilientHttp
{
    public class HttpClientConnection
    {
        private static volatile HttpClientConnection _instance;
        public static readonly object SyncLock = new object();

        private HttpClient _httpClient;

        private HttpClientConnection()
        {
            _httpClient = Create();
        }

        public static HttpClientConnection Current
        {
            get
            {
                if (_instance == null)
                    lock (SyncLock)
                    {
                        if (_instance == null) _instance = new HttpClientConnection();
                    }

                return _instance;
            }
        }

        public HttpClient GetConnection => _httpClient ?? (_httpClient = Create());

        private HttpClient Create()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            return httpClient;
        }
    }
}