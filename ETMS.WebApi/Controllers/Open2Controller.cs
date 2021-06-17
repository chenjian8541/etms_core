using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Open2.Request;
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
    [Route("api/open2/[action]")]
    [ApiController]
    public class Open2Controller : ControllerBase
    {
        private readonly IOpenBLL _openBLL;

        public Open2Controller(IOpenBLL openBLL)
        {
            this._openBLL = openBLL;
        }

        public async Task<ResponseBase> TenantInfoGet(Open2Base request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.TenantInfoGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebHomeGet(Open2Base request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebHomeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnSingleArticleGet(MicroWebColumnSingleArticleGetRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebColumnSingleArticleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebArticleGet(MicroWebArticleGetRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebArticleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebArticleGetPaging(MicroWebArticleGetPagingRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebArticleGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetJsSdkUiPackage(GetJsSdkUiPackageRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.GetJsSdkUiPackage(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryCalssApply(TryCalssApplyRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.TryCalssApply(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryCalssApplySupplement(TryCalssApplySupplementRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.TryCalssApplySupplement(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
