using ETMS.AiFace.Dto.Baidu.Output;
using ETMS.Entity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace ETMS.AiFace.Common
{
    public class HttpLib
    {
        public static TokenOutput BaiduGetTokenPost(string url, List<KeyValuePair<string, string>> paraList)
        {
            var client = new HttpClient();
            var response = client.PostAsync(url, new FormUrlEncodedContent(paraList)).Result;
            var strResult = response.Content.ReadAsStringAsync().Result;
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenOutput>(strResult);
            if (!string.IsNullOrEmpty(result.error))
            {
                LOG.Log.Fatal($"[BaiduGetTokenPost]{url},{strResult}", typeof(HttpLib));
                throw new EtmsErrorException("人脸识别出错");
            }
            else
            {
                return result;
            }
        }

        public static T BaiduAPISendPost<T>(string url, dynamic postData, string accessToken)
            where T : OutputBase
        {
            url = $"{url}?access_token={accessToken}";
            var encoding = Encoding.Default;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "post";
            request.KeepAlive = true;
            var buffer = encoding.GetBytes(JsonConvert.SerializeObject(postData));
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            var strResult = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<T>(strResult);
            var errorCode = result.error_code.Trim();
            if (errorCode != "0")
            {
                if (errorCode != "223101")//group is already exist 已知错误
                {
                    LOG.Log.Fatal($"[BaiduAPISendPost]{url},{strResult}", typeof(HttpLib));
                }
                throw new EtmsErrorException("人脸识别出错");
            }
            else
            {
                return result;
            }
        }
    }
}
