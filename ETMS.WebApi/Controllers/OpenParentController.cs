using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.Entity.Dto.OpenParent.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.Wechart;
using ETMS.LOG;
using ETMS.Utility;
using ETMS.WebApi.Controllers.Open;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Open.Containers;
using Senparc.Weixin.Open.Entities.Request;
using System;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/openParent/[action]")]
    [ApiController]
    public class OpenParentController : ControllerBase
    {
        private readonly IOpenParentBLL _openParentBLL;

        public OpenParentController(IOpenParentBLL openParentBLL)
        {
            this._openParentBLL = openParentBLL;
        }

        public async Task<ResponseBase> ParentLoginSendSms(ParentOpenLoginSendSmsRequest request)
        {
            try
            {
                return await _openParentBLL.ParentLoginSendSms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentLoginBySms(ParentOpenLoginBySmsRequest request)
        {
            try
            {
                return await _openParentBLL.ParentLoginBySms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentRegisterSendSms(ParentRegisterSendSmsRequest request)
        {
            try
            {
                return await _openParentBLL.ParentRegisterSendSms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentRegister(ParentRegisterOpenRequest request)
        {
            try
            {
                return await _openParentBLL.ParentRegister(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
