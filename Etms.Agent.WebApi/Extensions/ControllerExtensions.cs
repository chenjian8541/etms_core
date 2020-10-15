using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using ETMS.Entity.Config;
using ETMS.Utility;
using ETMS.Business.Common;

namespace Etms.Agent.WebApi.Extensions
{
    /// <summary>
    /// Controller扩展类
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// 代理商信息
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static int GetTokenInfo(this HttpRequest @this)
        {
            var tokenValue = @this.HttpContext.User?.Claims?.FirstOrDefault(p => p.Type.Equals(SystemConfig.AuthenticationConfig.ClaimType))?.Value;
            return tokenValue.ToInt();
        }
    }
}
