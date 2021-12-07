using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Com.Fubei.OpenApi.Sdk.Models;
using Com.Fubei.OpenApi.Sdk.Extensions;

namespace Com.Fubei.OpenApi.Sdk.Utils
{
    /// <summary>
    /// Http请求类封装
    /// 请仔细阅读以下文章，确保能够正确使用HttpClient：
    /// YOU'RE USING HTTPCLIENT WRONG AND IT IS DESTABILIZING YOUR SOFTWARE | https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
    /// .NET HttpClient，存在缺陷让开发人员沮丧 | http://www.oschina.net/news/77036/httpclient
    /// </summary>
    public class HttpUtil
    {
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30)};

        private const string MimeType = "application/json";
        
        public static async Task<string> PostRequest(string url, IBaseModel requestParam)
        {
            var httpClient = HttpClient;
            // 设置http content-type头
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeType));
            var content = new StringContent(requestParam.SerializeAsJson(), Encoding.UTF8, MimeType);
            
            var httpResponseMessage = await httpClient.PostAsync(url, content);
            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}