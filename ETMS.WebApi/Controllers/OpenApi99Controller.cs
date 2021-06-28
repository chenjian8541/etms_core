using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.Entity.Dto.OpenApi99.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.OpenApi99;
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
    [Route("api/openApi99/[action]")]
    [ApiController]
    public class OpenApi99Controller : ControllerBase
    {
        private readonly ITenantOpenBLL _tenantOpenBLL;

        public OpenApi99Controller(ITenantOpenBLL tenantOpenBLL)
        {
            this._tenantOpenBLL = tenantOpenBLL;
        }

        public async Task<ResponseBase> TenantInfoGet(OpenApi99Base request)
        {
            try
            {
                _tenantOpenBLL.InitTenantId(request.LoginTenantId);
                return await _tenantOpenBLL.TenantInfoGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SmsSend(SmsSendRequest request)
        {
            try
            {
                _tenantOpenBLL.InitTenantId(request.LoginTenantId);
                return await _tenantOpenBLL.SmsSend(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SmsSendValidCode(SmsSendValidCodeRequest request)
        {
            try
            {
                _tenantOpenBLL.InitTenantId(request.LoginTenantId);
                return await _tenantOpenBLL.SmsSendValidCode(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
