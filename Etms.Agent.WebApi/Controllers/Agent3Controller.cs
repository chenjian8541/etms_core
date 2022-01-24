using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
using ETMS.Entity.EtmsManage.Dto.Agent3.Request;
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

        private readonly ISysExternalConfigBLL _sysExternalConfigBLL;

        public Agent3Controller(ISysSmsTemplateBLL sysSmsTemplateBLL, ISysTryApplyLogBLL sysTryApplyLogBLL,
            ISysExternalConfigBLL sysExternalConfigBLL)
        {
            this._sysSmsTemplateBLL = sysSmsTemplateBLL;
            this._sysTryApplyLogBLL = sysTryApplyLogBLL;
            this._sysExternalConfigBLL = sysExternalConfigBLL;
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

        public async Task<ResponseBase> SysExternalConfigAdd(SysExternalConfigAddRequest request)
        {
            try
            {
                return await _sysExternalConfigBLL.SysExternalConfigAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysExternalConfigEdit(SysExternalConfigEditRequest request)
        {
            try
            {
                return await _sysExternalConfigBLL.SysExternalConfigEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysExternalConfigDel(SysExternalConfigDelRequest request)
        {
            try
            {
                return await _sysExternalConfigBLL.SysExternalConfigDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysExternalConfigPaging(SysExternalConfigPagingRequest request)
        {
            try
            {
                return await _sysExternalConfigBLL.SysExternalConfigPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysExternalConfigGetList(AgentRequestBase request)
        {
            try
            {
                return await _sysExternalConfigBLL.SysExternalConfigGetList(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
