using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaskTips.Proxy
{
    public class NullHttpClientService: IHttpClientService
    {
        public T HttpGetJson<T>(string url) 
        {
            return default(T);
        }

        public Task<T> HttpGetJsonAsync<T>(string url)
        {
            return default(Task<T>);
        }

        public T HttpPostJson<T>(string url, object json)
        {
            return default(T);
        }

        public T HttpPostJson<T>(string url, string json)
        {
            return default(T);
        }

        public Task<T> HttpPostJsonAsync<T>(string url, object json)
        {
            return default(Task<T>);
        }

        public Task<T> HttpPostJsonAsync<T>(string url, string json)
        {
            return default(Task<T>);
        }

        public Task<T> HttpRequestJsonAsync<T>(string url, HttpMethod method, string json = "", string authToken = "")
        {
            return default(Task<T>);
        }
    }
}
