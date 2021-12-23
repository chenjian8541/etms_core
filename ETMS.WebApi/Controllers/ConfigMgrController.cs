using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Entity.Dto.ConfigMgr.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/configMgr/[action]")]
    [ApiController]
    [Authorize]
    public class ConfigMgrController : ControllerBase
    {
        private readonly IShareTemplateBLL _shareTemplateBLL;

        public ConfigMgrController(IShareTemplateBLL shareTemplateBLL)
        {
            this._shareTemplateBLL = shareTemplateBLL;
        }

        public async Task<ResponseBase> ShareTemplateGet(ShareTemplateGetRequest request)
        {
            try
            {
                _shareTemplateBLL.InitTenantId(request.LoginTenantId);
                return await _shareTemplateBLL.ShareTemplateGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ShareTemplateAdd(ShareTemplateAddRequest request)
        {
            try
            {
                _shareTemplateBLL.InitTenantId(request.LoginTenantId);
                return await _shareTemplateBLL.ShareTemplateAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ShareTemplateEdit(ShareTemplateEditRequest request)
        {
            try
            {
                _shareTemplateBLL.InitTenantId(request.LoginTenantId);
                return await _shareTemplateBLL.ShareTemplateEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ShareTemplateDel(ShareTemplateDelRequest request)
        {
            try
            {
                _shareTemplateBLL.InitTenantId(request.LoginTenantId);
                return await _shareTemplateBLL.ShareTemplateDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ShareTemplateChangeStatus(ShareTemplateChangeStatusRequest request)
        {
            try
            {
                _shareTemplateBLL.InitTenantId(request.LoginTenantId);
                return await _shareTemplateBLL.ShareTemplateChangeStatus(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ShareTemplateGetPaging(ShareTemplateGetPagingRequest request)
        {
            try
            {
                _shareTemplateBLL.InitTenantId(request.LoginTenantId);
                return await _shareTemplateBLL.ShareTemplateGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
