using ETMS.Entity.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using ETMS.LOG;
using ETMS.IOC;
using ETMS.IBusiness;
using ETMS.Entity.EtmsManage.Common;
using ETMS.IBusiness.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.Agent.Output;
using ETMS.Entity.Alien.Common;
using ETMS.Alien.Webapi.Core;
using ETMS.Business.Common;
using ETMS.IBusiness.Alien;
using ETMS.Entity.Alien.Dto.User.Output;

namespace ETMS.Alien.Webapi.FilterAttribute
{
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
                if (context.ActionArguments.First().Value is AlienRequestBase)
                {
                    var request = context.ActionArguments.First().Value as AlienRequestBase;
                    var tokenInfoResult = context.HttpContext.Request.GetTokenInfo();
                    request.LoginHeadId = tokenInfoResult.Item1;
                    request.LoginUserId = tokenInfoResult.Item2;
                    request.LoginTimestamp = tokenInfoResult.Item3;
                    request.LoginClientType = RequestLib.GetUserClientType(context.HttpContext.Request);
                    var userLoginBLL = CustomServiceLocator.GetInstance<IAlienUserLoginBLL>();
                    var checkUserResult = userLoginBLL.CheckUserCanLogin(request).Result;
                    if (!checkUserResult.IsResponseSuccess())
                    {
                        context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        context.Result = new JsonResult(new { msg = checkUserResult.message });
                        return;
                    }
                    var userLoginInfo = checkUserResult.resultData as CheckUserCanLoginOutput;
                    request.AllTenants = userLoginInfo.AllTenants;
                }
            }
        }
    }
}
