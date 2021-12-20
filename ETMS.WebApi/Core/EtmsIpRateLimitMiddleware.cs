using AspNetCoreRateLimit;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.IEventProvider;
using ETMS.IOC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Core
{
    public class EtmsIpRateLimitMiddleware : IpRateLimitMiddleware
    {
        public EtmsIpRateLimitMiddleware(RequestDelegate next, IProcessingStrategy processingStrategy, IOptions<IpRateLimitOptions> options, IRateLimitCounterStore counterStore, IIpPolicyStore policyStore, IRateLimitConfiguration config, ILogger<IpRateLimitMiddleware> logger)
            : base(next, processingStrategy, options, counterStore, policyStore, config, logger)
        {
        }

        public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
        {
            httpContext.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var eventPublisher = CustomServiceLocator.GetInstance<IEventPublisher>();
            eventPublisher.Publish(new NoticeManageEvent()
            {
                Type = NoticeManageType.DangerousIp,
                MyDangerousVisitor = new DangerousVisitor()
                {
                    Url = httpContext.Request.AbsoluteUri(),
                    RemoteIpAddress = httpContext.Connection.RemoteIpAddress.ToString(),
                    LocalIpAddress = httpContext.Connection.LocalIpAddress.ToString(),
                    Time = DateTime.Now
                }
            });
            return base.ReturnQuotaExceededResponse(httpContext, rule, retryAfter);
        }
    }
}