using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etms.Agent.WebApi.FilterAttribute
{
    public class EtmsResponseCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 方法执行完成后设置输出内容缓存
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(10)
            };
            context.HttpContext.Response.Headers[HeaderNames.Vary] = new string[] { "Accept-Encoding" };
        }
    }
}
