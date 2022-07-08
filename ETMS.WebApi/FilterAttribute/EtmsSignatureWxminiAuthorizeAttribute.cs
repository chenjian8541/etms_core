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
    public class EtmsSignatureWxminiAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor != null && actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault() != null)
            {
                return;
            }
            var strSignature = context.HttpContext.Request.Headers["etms-s"].ToString();
            var strLoginInfo = context.HttpContext.Request.Headers["etms-l"].ToString();
            if (string.IsNullOrEmpty(strSignature) || string.IsNullOrEmpty(strLoginInfo))
            {
                Unauthorized(context);
                return;
            }
            if (!ParentSignatureLib.CheckOpenParent2Signature(strLoginInfo.ToLong(), strSignature))
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
