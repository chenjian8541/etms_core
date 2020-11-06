using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using ETMS.Entity.Config;
using ETMS.Utility;
using ETMS.Business.Common;

namespace ETMS.WebApi.Extensions
{
    /// <summary>
    /// Controller扩展类
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// 解密用户信息 “机构ID,用户ID,登录时间戳”
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Tuple<int, long, string> GetTokenInfo(this HttpRequest @this)
        {
            var tokenValue = @this.HttpContext.User?.Claims?.FirstOrDefault(p => p.Type.Equals(SystemConfig.AuthenticationConfig.ClaimType))?.Value;
            var userInfo = tokenValue.Split(',');
            var loginTimestamp = "0";
            if (userInfo.Length > 2)
            {
                loginTimestamp = userInfo[2].ToString();
            }
            return Tuple.Create(userInfo[0].ToInt(), userInfo[1].ToLong(), loginTimestamp);
        }

        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static ParentTokenConfig GetParentLoginInfo(this HttpRequest @this)
        {
            return ParentSignatureLib.GetParentLoginInfo(@this.Headers["etms-l"]);
        }
    }
}
