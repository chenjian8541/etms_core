using ETMS.Entity.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using ETMS.WebApi.Extensions;
using ETMS.LOG;
using ETMS.IOC;
using ETMS.IBusiness;
using System;
using ETMS.Utility;
using Newtonsoft.Json;
using ETMS.Business.Common;
using ETMS.Entity.Dto.User.Output;

namespace ETMS.WebApi.FilterAttribute
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
                //Log.Debug($"[OnActionExecuting]action:{context.ActionDescriptor.DisplayName},参数:{JsonConvert.SerializeObject(validateRequest)}", this.GetType());
                if (context.ActionArguments.First().Value is RequestBase)
                {
                    var request = context.ActionArguments.First().Value as RequestBase;
                    var userInfo = context.HttpContext.Request.GetTokenInfo();
                    request.LoginTenantId = userInfo.Item1;
                    request.LoginUserId = userInfo.Item2;
                    request.LoginTimestamp = userInfo.Item3;
                    request.IpAddress = userInfo.Item4;
                    request.LoginClientType = RequestLib.GetUserClientType(context.HttpContext.Request);
                    var userLoginBLL = CustomServiceLocator.GetInstance<IUserLoginBLL>();
                    var checkUserResult = userLoginBLL.CheckUserCanLogin(request).Result;
                    if (!checkUserResult.IsResponseSuccess())
                    {
                        context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        context.Result = new JsonResult(new { msg = checkUserResult.message });
                        return;
                    }
                    var userLoginInfo = checkUserResult.resultData as CheckUserCanLoginOutput;
                    request.IsDataLimit = userLoginInfo.IsDataLimit;
                    request.SecrecyType = userLoginInfo.SecrecyType;
                }
                if (context.ActionArguments.First().Value is ParentRequestBase)
                {
                    var request = context.ActionArguments.First().Value as ParentRequestBase;
                    var parentLoginInfo = context.HttpContext.Request.GetParentLoginInfo();
                    request.LoginTenantId = parentLoginInfo.TenantId;
                    request.LoginPhone = parentLoginInfo.Phone;
                    request.ExTime = DataTimeExtensions.StampToDateTime(parentLoginInfo.ExTimestamp);
                    if (request.ExTime.Date < DateTime.Now.Date)
                    {
                        context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                        context.Result = new JsonResult(ResponseBase.ParentTokenExTime());
                        return;
                    }
                    var parentBLL = CustomServiceLocator.GetInstance<IParentBLL>();
                    var checkParentCanLoginResult = parentBLL.CheckParentCanLogin(request).Result;
                    if (!checkParentCanLoginResult.IsResponseSuccess())
                    {
                        context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        context.Result = new JsonResult(checkParentCanLoginResult);
                        return;
                    }
                    var myStudents = parentBLL.GetMyStudent(request).Result;
                    if (myStudents == null || !myStudents.Any())
                    {
                        context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                        context.Result = new JsonResult(ResponseBase.ParentUnBindStudent());
                        return;
                    }
                    request.ParentStudentIds = myStudents.Select(p => p.Id).ToList();
                }
            }
        }
    }
}
