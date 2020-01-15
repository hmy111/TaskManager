using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaskTips.Proxy
{
    public interface IHttpClientService
    {
        T HttpGetJson<T>(string url);
        T HttpPostJson<T>(string url, object json);
        T HttpPostJson<T>(string url, string json);

        Task<T> HttpGetJsonAsync<T>(string url);

        Task<T> HttpPostJsonAsync<T>(string url, object json);

        Task<T> HttpPostJsonAsync<T>(string url, string json);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="method"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        Task<T> HttpRequestJsonAsync<T>(string url, HttpMethod method, string json = "", string authToken = "");
    }
}
