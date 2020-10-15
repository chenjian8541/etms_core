using ETMS.LOG;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etms.Agent.WebApi.FilterAttribute
{
    public class EtmsExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            Log.Error("全局捕获的异常", context.Exception, this.GetType());
            base.OnException(context);
        }
    }
}
