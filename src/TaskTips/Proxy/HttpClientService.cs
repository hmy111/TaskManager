using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskTips.Proxy;

namespace TaskTips
{
    public class HttpClientService: IHttpClientService
    {
        public HttpClient httpClient;
        private readonly ILogger _logger;
        public HttpClientService(HttpClient httpClient, ILogger<HttpClientService> logger)
        {
            httpClient.DefaultRequestHeaders.Add("Accept",
                "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "YH.Etms.Transportation.Api");
            this.httpClient = httpClient;
            _logger = logger;
        }

        public T HttpGetJson<T>(string url)
        {
            return HttpGetJsonAsync<T>(url).GetAwaiter().GetResult();
        }
        public T HttpPostJson<T>(string url, object json)
        {
            return HttpPostJsonAsync<T>(url, json).GetAwaiter().GetResult();
        }
        public T HttpPostJson<T>(string url, string json)
        {
            return HttpPostJsonAsync<T>(url, json).GetAwaiter().GetResult();
        }

        public async Task<T> HttpGetJsonAsync<T>(string url)
        {
            return await HttpRequestJsonAsync<T>(url, HttpMethod.Get);
        }

        public async Task<T> HttpPostJsonAsync<T>(string url, object json)
        {
            return await HttpPostJsonAsync<T>(url, JsonConvert.SerializeObject(json));
        }

        public async Task<T> HttpPostJsonAsync<T>(string url, string json)
        {
            return await HttpRequestJsonAsync<T>(url, HttpMethod.Post, json);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="method"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public async Task<T> HttpRequestJsonAsync<T>(string url, HttpMethod method, string json = "", string authToken = "")
        {
            try
            {
                HttpRequestMessage request = FactoryHttpRequestMethod(url, method, json, authToken);
                var response = await httpClient.SendAsync(request);
                return await ResponseToEntityAsync<T>(response);
            }
            catch (System.Exception ex)
            {
                string msg = $"请求接口：{url},Meth:Post,Content:{JsonConvert.SerializeObject(json)};具体描述：{ex}";
                throw new Exception(msg);
            }
        }

        private static async Task<T> ResponseToEntityAsync<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            T result = default(T);
            switch (typeof(T).Name)
            {
                case "Stream":
                    result = (T)Convert.ChangeType(await response.Content.ReadAsByteArrayAsync(), typeof(T));
                    break;
                case "String":
                    result = (T)Convert.ChangeType(await response.Content.ReadAsStringAsync(), typeof(T));
                    break;
                case "Byte[]":
                    result = (T)Convert.ChangeType(await response.Content.ReadAsByteArrayAsync(), typeof(T));
                    break;
                default:
                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<T>(content);
                    break;
            }
            return result;
        }

        private HttpRequestMessage FactoryHttpRequestMethod(string url, HttpMethod method, string json, string AuthToken)
        {
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrWhiteSpace(json))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            if (!string.IsNullOrWhiteSpace(AuthToken))
            {
                request.Headers.Add("authorization", AuthToken);
            }
            return request;
        }




    }
}
