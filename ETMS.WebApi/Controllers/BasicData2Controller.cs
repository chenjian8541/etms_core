using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.EtmsManage;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/basic2/[action]")]
    [ApiController]
    [Authorize]
    public class BasicData2Controller : ControllerBase
    {
        private readonly ISysSmsTemplate2BLL _sysSmsTemplate2BLL;

        private readonly IBascDataInfoBLL _bascDataInfoBLL;

        public BasicData2Controller(ISysSmsTemplate2BLL sysSmsTemplate2BLL, IBascDataInfoBLL bascDataInfoBLL)
        {
            this._sysSmsTemplate2BLL = sysSmsTemplate2BLL;
            this._bascDataInfoBLL = bascDataInfoBLL;
        }

        public async Task<ResponseBase> SysSmsTemplateGet(SysSmsTemplateGetRequest request)
        {
            try
            {
                this._sysSmsTemplate2BLL.InitTenantId(request.LoginTenantId);
                return await _sysSmsTemplate2BLL.SysSmsTemplateGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysSmsTemplateSave(SysSmsTemplateSaveRequest request)
        {
            try
            {
                this._sysSmsTemplate2BLL.InitTenantId(request.LoginTenantId);
                return await _sysSmsTemplate2BLL.SysSmsTemplateSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysSmsTemplateDel(SysSmsTemplateResetRequest request)
        {
            try
            {
                this._sysSmsTemplate2BLL.InitTenantId(request.LoginTenantId);
                return await _sysSmsTemplate2BLL.SysSmsTemplateDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> GetRegions(GetRegionsRequrst request)
        {
            try
            {
                return await _bascDataInfoBLL.GetRegions(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> GetRegions2(GetRegionsRequrst request)
        {
            try
            {
                return await _bascDataInfoBLL.GetRegions(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> GetBanks(GetBanksRequrst request)
        {
            try
            {
                return await _bascDataInfoBLL.GetBanks(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> GetIndustry(GetIndustryRequrst request)
        {
            try
            {
                return await _bascDataInfoBLL.GetIndustry(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
