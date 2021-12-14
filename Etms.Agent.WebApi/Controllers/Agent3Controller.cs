using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
using ETMS.Entity.EtmsManage.Dto.DataLog.Request;
using ETMS.Entity.EtmsManage.Dto.Explain.Request;
using ETMS.Entity.EtmsManage.Dto.SysCommon.Request;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
using ETMS.Entity.EtmsManage.Dto.User.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etms.Agent.WebApi.Controllers
{
    [Route("api/agent3/[action]")]
    [ApiController]
    [Authorize]
    public class Agent3Controller : ControllerBase
    {
        private readonly ISysSmsTemplateBLL _sysSmsTemplateBLL;

        private readonly ISysTryApplyLogBLL _sysTryApplyLogBLL;

        public Agent3Controller(ISysSmsTemplateBLL sysSmsTemplateBLL, ISysTryApplyLogBLL sysTryApplyLogBLL)
        {
            this._sysSmsTemplateBLL = sysSmsTemplateBLL;
            this._sysTryApplyLogBLL = sysTryApplyLogBLL;
        }

        public async Task<ResponseBase> SysSmsTemplateGetPaging(SysSmsTemplateGetPagingRequest request)
        {
            try
            {
                return await _sysSmsTemplateBLL.SysSmsTemplateGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysSmsTemplateHandle(SysSmsTemplateHandleRequest request)
        {
            try
            {
                return await _sysSmsTemplateBLL.SysSmsTemplateHandle(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryApplyLogGetPaging(TryApplyLogGetPagingRequest request)
        {
            try
            {
                return await _sysTryApplyLogBLL.TryApplyLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryApplyLogHandle(TryApplyLogHandleRequest request)
        {
            try
            {
                return await _sysTryApplyLogBLL.TryApplyLogHandle(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
