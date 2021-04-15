using ETMS.AiFace.Dto.Baidu.Output;
using ETMS.Entity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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
                LOG.Log.Fatal($"[BaiduGetTokenPost]{url},{result}", typeof(HttpLib));
                throw new EtmsErrorException("人脸识别出错");
            }
            else
            {
                return result;
            }
        }

        public static T BaiduAPISendPost<T>(string url, List<KeyValuePair<string, string>> paraList, string accessToken)
            where T : OutputBase
        {
            url = $"{url}?access_token={accessToken}";
            var client = new HttpClient();
            var response = client.PostAsync(url, new FormUrlEncodedContent(paraList)).Result;
            var strResult = response.Content.ReadAsStringAsync().Result;
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strResult);
            if (result.error_code.Trim() != "0")
            {
                LOG.Log.Fatal($"[BaiduAPISendPost]{url},{result}", typeof(HttpLib));
                throw new EtmsErrorException("人脸识别出错");
            }
            else
            {
                return result;
            }
        }
    }
}
