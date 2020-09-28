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
        /// 解密用户信息 “机构ID,用户ID”
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Tuple<int, long> GetTokenInfo(this HttpRequest @this)
        {
            var tokenValue = @this.HttpContext.User?.Claims?.FirstOrDefault(p => p.Type.Equals(SystemConfig.AuthenticationConfig.ClaimType))?.Value;
            var userInfo = tokenValue.Split(',');
            return Tuple.Create(userInfo[0].ToInt(), userInfo[1].ToLong());
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
