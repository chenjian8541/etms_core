using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
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
    [Route("api/basic2/[action]")]
    [ApiController]
    [Authorize]
    public class BasicData2Controller : ControllerBase
    {
        private readonly ISysSmsTemplate2BLL _sysSmsTemplate2BLL;

        public BasicData2Controller(ISysSmsTemplate2BLL sysSmsTemplate2BLL)
        {
            this._sysSmsTemplate2BLL = sysSmsTemplate2BLL;
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
    }
}
