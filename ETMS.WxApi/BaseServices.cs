using ETMS.Utility;
using Newtonsoft.Json;
using System;
using WxApi;
using WxApi.ReceiveEntity;

namespace ETMS.WxApi
{
    public class BaseServices
    {
        /// <summary>
        /// 接入验证
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool ValidUrl(string token, string signature, string timestamp, string nonce, string echostr)
        {
            string[] temp = { token, timestamp, nonce };
            Array.Sort(temp);
            var tempstr = string.Join("", temp);
            var tempsign = CryptogramHelper.EncryptSHA1(tempstr).ToLower();
            LOG.Log.Info($"[验证消息的确来自微信服务器]tempsign:{tempsign}", typeof(BaseServices));
            if (tempsign == signature)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static AccessToken GetAccessToken(string appid, string appSecret)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, appSecret);
            LOG.Log.Info($"[获取AccessToken]url:{url}", typeof(BaseServices));
            var result = Utils.GetResult<AccessToken>(url);
            LOG.Log.Info($"[获取AccessToken]result:{JsonConvert.SerializeObject(result)}", typeof(BaseServices));
            return result;
        }
    }
}
