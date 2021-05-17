using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
using ETMS.Entity.EtmsManage.Dto.DataLog.Request;
using ETMS.Entity.EtmsManage.Dto.Explain.Request;
using ETMS.Entity.EtmsManage.Dto.SysCommon.Request;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
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
    [Route("api/agent2/[action]")]
    [ApiController]
    [Authorize]
    public class Agent2Controller : ControllerBase
    {
        private readonly ISysTenantBLL _sysTenantBLL;

        private readonly IDataLogBLL _dataLogBLL;

        public Agent2Controller(ISysTenantBLL sysTenantBLL, IDataLogBLL dataLogBLL)
        {
            this._sysTenantBLL = sysTenantBLL;
            this._dataLogBLL = dataLogBLL;
        }

        public async Task<ResponseBase> TenantBindCloudSave(TenantBindCloudSaveRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantBindCloudSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceBiduAccountGet(AIFaceBiduAccountGetRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceBiduAccountGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceBiduAccountGetPaging(AIFaceBiduAccountGetPagingRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceBiduAccountGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceBiduAccountAdd(AIFaceBiduAccountAddRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceBiduAccountAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceBiduAccountEdit(AIFaceBiduAccountEditRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceBiduAccountEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceBiduAccountDel(AIFaceBiduAccountDelRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceBiduAccountDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceTenantAccountGet(AIFaceTenantAccountGetRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceTenantAccountGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceTenantAccountGetGetPaging(AIFaceTenantAccountGetGetPagingRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceTenantAccountGetGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceTenantAccountAdd(AIFaceTenantAccountAddRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceTenantAccountAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceTenantAccountEdit(AIFaceTenantAccountEditRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceTenantAccountEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceTenantAccountDel(AIFaceTenantAccountDelRequest request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceTenantAccountDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AIFaceAllAccountGet(AgentRequestBase request)
        {
            try
            {
                return await _sysTenantBLL.AIFaceAllAccountGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ResetTenantAdminUserPwd(ResetTenantAdminUserPwdRequest request)
        {
            try
            {
                return await _sysTenantBLL.ResetTenantAdminUserPwd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysSmsLogPaging(SysSmsLogPagingRequest request)
        {
            try
            {
                return await _dataLogBLL.SysSmsLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysTenantOperationLogPaging(SysTenantOperationLogPagingRequest request)
        {
            try
            {
                return await _dataLogBLL.SysTenantOperationLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
