using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WxApi.ReceiveEntity;

namespace WxApi
{
    public class JsApi
    {
        /// <summary>
        /// 获取jsapi_ticket
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static JsApiTicket GetHsJsApiTicket(string accessToken)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", accessToken);
            return Utils.GetResult<JsApiTicket>(url);
        }
    }
}
