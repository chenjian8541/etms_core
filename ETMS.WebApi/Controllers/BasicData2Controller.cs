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

        private readonly ITenantBLL _tenantBLL;

        private readonly ILibMediaFliesBLL _libMediaFliesBLL;

        public BasicData2Controller(ISysSmsTemplate2BLL sysSmsTemplate2BLL, IBascDataInfoBLL bascDataInfoBLL,
            ITenantBLL tenantBLL, ILibMediaFliesBLL libMediaFliesBLL)
        {
            this._sysSmsTemplate2BLL = sysSmsTemplate2BLL;
            this._bascDataInfoBLL = bascDataInfoBLL;
            this._tenantBLL = tenantBLL;
            this._libMediaFliesBLL = libMediaFliesBLL;
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

        public async Task<ResponseBase> PageBascDataGet(RequestBase request)
        {
            try
            {
                this._tenantBLL.InitTenantId(request.LoginTenantId);
                return await _tenantBLL.PageBascDataGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ImageGetPaging(ImageGetPagingRequest request)
        {
            try
            {
                this._libMediaFliesBLL.InitTenantId(request.LoginTenantId);
                return await _libMediaFliesBLL.ImageGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ImageAdd(ImageAddRequest request)
        {
            try
            {
                this._libMediaFliesBLL.InitTenantId(request.LoginTenantId);
                return await _libMediaFliesBLL.ImageAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ImageDel(ImageDelRequest request)
        {
            try
            {
                this._libMediaFliesBLL.InitTenantId(request.LoginTenantId);
                return await _libMediaFliesBLL.ImageDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ImageListGet(ImageListGetRequest request)
        {
            try
            {
                this._libMediaFliesBLL.InitTenantId(request.LoginTenantId);
                return await _libMediaFliesBLL.ImageListGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AudioGetPaging(AudioGetPagingRequest request)
        {
            try
            {
                this._libMediaFliesBLL.InitTenantId(request.LoginTenantId);
                return await _libMediaFliesBLL.AudioGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AudioAdd(AudioAddRequest request)
        {
            try
            {
                this._libMediaFliesBLL.InitTenantId(request.LoginTenantId);
                return await _libMediaFliesBLL.AudioAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AudioDel(AudioDelRequest request)
        {
            try
            {
                this._libMediaFliesBLL.InitTenantId(request.LoginTenantId);
                return await _libMediaFliesBLL.AudioDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AudioListGet(AudioListGetRequest request)
        {
            try
            {
                this._libMediaFliesBLL.InitTenantId(request.LoginTenantId);
                return await _libMediaFliesBLL.AudioListGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
