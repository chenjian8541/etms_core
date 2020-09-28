using ETMS.Business.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using ETMS.Utility;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace ETMS.WebApi.FilterAttribute
{
    /// <summary>
    /// 接口签名授权验证
    /// 家长端使用签名验证用户信息Headers信息：
    /// etms-s:签名信息
    /// etms-l:登录信息
    /// </summary>
    public class EtmsSignatureAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor != null && actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault() != null)
            {
                return;
            }
            var strSignature = context.HttpContext.Request.Headers["etms-s"];
            var strLoginInfo = context.HttpContext.Request.Headers["etms-l"];
            if (string.IsNullOrEmpty(strSignature) || string.IsNullOrEmpty(strLoginInfo))
            {
                Unauthorized(context);
                return;
            }
            if (!ParentSignatureLib.CheckSignature(strLoginInfo, strSignature))
            {
                Unauthorized(context);
                return;
            }
        }

        /// <summary>
        ///  无权限
        /// </summary>
        /// <param name="context"></param>
        private void Unauthorized(AuthorizationFilterContext context)
        {
            context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            context.Result = new JsonResult(new { msg = "无权限" });
        }
    }
}
