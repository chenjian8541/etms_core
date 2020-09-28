using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Utility
{
    public class UrlHelper
    {
        public static string GetUrl(IHttpContextAccessor httpContextAccessor, string virtualPath, string urlKey)
        {
            if (string.IsNullOrEmpty(urlKey))
            {
                return string.Empty;
            }
            var host = httpContextAccessor.HttpContext.Request.Host.Host;
            var port = httpContextAccessor.HttpContext.Request.Host.Port;
            var scheme = httpContextAccessor.HttpContext.Request.Scheme;
            if (port == null)
            {
                return $"{scheme}://{host}{virtualPath}/{urlKey}";
            }
            else
            {
                return $"{scheme}://{host}:{port}{virtualPath}/{urlKey}";
            }
        }
    }
}
