using ETMS.Entity.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Etms.Agent.WebApi.Extensions;
using ETMS.LOG;
using ETMS.IOC;
using ETMS.IBusiness;
using ETMS.Entity.EtmsManage.Common;
using ETMS.IBusiness.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.Agent.Output;

namespace Etms.Agent.WebApi.FilterAttribute
{
    /// <summary>
    /// 验证请求参数,并赋值登录信息
    /// </summary>
    public class EtmsValidateRequestAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 方法执行之前执行,验证请求的数据合法性,验证完成后赋值用户登录信息
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.Count == 1 && context.ActionArguments.First().Value is IValidate)
            {
                var validateRequest = context.ActionArguments.First().Value as IValidate;
                var errMsg = validateRequest.Validate();
                if (!string.IsNullOrEmpty(errMsg))
                {
                    context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    context.Result = new JsonResult(new ResponseBase().GetResponseBadRequest(errMsg));
                    return;
                }
                Log.Debug(validateRequest, this.GetType());
                if (context.ActionArguments.First().Value is AgentRequestBase)
                {
                    var request = context.ActionArguments.First().Value as AgentRequestBase;
                    var tokenInfoResult = context.HttpContext.Request.GetTokenInfo();
                    if (tokenInfoResult.Item1 == 0 || tokenInfoResult.Item2 == 0)
                    {
                        context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        context.Result = new JsonResult(new { msg = "登录信息过期" });
                        return;
                    }
                    request.LoginAgentId = tokenInfoResult.Item1;
                    request.LoginUserId = tokenInfoResult.Item2;
                    var agentBLL = CustomServiceLocator.GetInstance<IAgentBLL>();
                    var checkAgentResult = agentBLL.CheckAgentLogin(request).Result;
                    if (!checkAgentResult.IsResponseSuccess())
                    {
                        context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        context.Result = new JsonResult(new { msg = checkAgentResult.message });
                        return;
                    }
                    var output = (CheckAgentLoginOutput)checkAgentResult.resultData;
                    request.LoginAgentIsLimitData = output.IsRoleLimitData;
                    request.LoginUserIsLimitData = output.IsUserLimitData;
                }
            }
        }
    }
}
