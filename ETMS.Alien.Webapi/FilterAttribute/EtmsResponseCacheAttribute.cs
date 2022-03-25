using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace ETMS.Alien.Webapi.FilterAttribute
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
