using ETMS.Utility;
using System;

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
            LOG.Log.Debug($"[验证消息的确来自微信服务器]tempsign:{tempsign}", typeof(BaseServices));
            if (tempsign == signature)
            {
                return true;
            }
            return false;
        }
    }
}
