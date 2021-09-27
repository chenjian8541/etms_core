using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ETMS.LOG;
using ETMS.Entity.Pay.Lcsw.Dto.Request.Response;
using ETMS.Entity.Pay.Lcsw.Dto;

namespace ETMS.Pay.Lcsw.Utility
{
    /// <summary>
    /// Post 请求处理
    /// </summary>
    public static class Post
    {
        /// <summary>
        /// 发起Post请求
        /// </summary>
        /// <typeparam name="T">返回数据类型（Json对应的实体）</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="postData">文件流</param>
        /// <param name="useAjax">是否使用Ajax请求</param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <returns></returns>
        public static T PostGetJson<T>(string url, object postData, bool useAjax = false, int timeOut = Config.TIME_OUT)
        {
            var jsonString = JsonConvert.SerializeObject(postData);
            LOG.Log.Debug($"[利楚扫呗请求]rul:{url},postData:{jsonString}", typeof(Post));
            using (var ms = new MemoryStream())
            {
                var bytes = Encoding.UTF8.GetBytes(jsonString);
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                string returnText = HttpPost(url, ms, useAjax, timeOut);
                LOG.Log.Debug($"[利楚扫呗返回]rul:{url},postData:{returnText}", typeof(Post));
                var result = GetResult<T>(returnText);
                return result;
            }
        }

        private static string HttpPost(string url, Stream postStream = null, bool useAjax = false, int timeOut = Config.TIME_OUT)
        {
            HttpWebResponse response = HttpResponsePost(url, postStream, useAjax, timeOut);
            using (Stream responseStream = response.GetResponseStream() ?? new MemoryStream())
            {
                using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                {
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
            }
        }

        private static HttpWebResponse HttpResponsePost(string url, Stream postStream = null, bool useAjax = false, int timeOut = Config.TIME_OUT)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = timeOut;
            request.ContentLength = postStream != null ? postStream.Length : 0;
            HttpClientHeader(request, useAjax, timeOut);
            if (postStream != null)
            {
                postStream.Position = 0;
                //直接写入流
                Stream requestStream = request.GetRequestStream();
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }
                postStream.Close();//关闭文件访问
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        /// <summary>
        /// 设置HTTP头
        /// </summary>
        /// <param name="request"></param>
        /// <param name="refererUrl"></param>
        /// <param name="useAjax">是否使用Ajax</param>
        /// <param name="timeOut"></param>
        private static void HttpClientHeader(HttpWebRequest request, bool useAjax, int timeOut)
        {
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.62 Safari/537.36";
            request.Timeout = timeOut;
            request.KeepAlive = true;
            request.ContentType = "application/json";
            if (useAjax)
            {
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            }
        }


        /// <summary>
        /// 获取Post结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="returnText"></param>
        /// <returns></returns>
        private static T GetResult<T>(string returnText)
        {
            //if (returnText.Contains("return_code"))
            //{
            //    BaseResult baseresult = JsonConvert.DeserializeObject<BaseResult>(returnText);
            //    if (baseresult.return_code != ReturnCode.成功)
            //    {
            //        throw new Exception(string.Format("扫呗Post请求发生错误！错误信息：{0}", baseresult.return_msg));
            //    }
            //}
            return JsonConvert.DeserializeObject<T>(returnText);
        }
    }
}
