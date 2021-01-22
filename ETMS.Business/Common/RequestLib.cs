using ETMS.Entity.Enum;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Business.Common
{
    public class RequestLib
    {
        public static int GetUserClientType(HttpRequest httpRequest)
        {
            if (httpRequest.Headers.TryGetValue("ClientType", out var apiKeyHeaderValues))
            {
                var temp = apiKeyHeaderValues.FirstOrDefault();
                if (!string.IsNullOrEmpty(temp))
                {
                    return apiKeyHeaderValues.ToInt();
                }
            }
            return EmUserOperationLogClientType.PC;
        }
    }
}
