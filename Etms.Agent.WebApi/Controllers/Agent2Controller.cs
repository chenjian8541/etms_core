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
    [Route("api/agent2/[action]")]
    [ApiController]
    [Authorize]
    public class Agent2Controller : ControllerBase
    {
        private readonly ISysTenantBLL _sysTenantBLL;

        private readonly IDataLogBLL _dataLogBLL;

        private readonly ISysUserBLL _sysUserBLL;

        public Agent2Controller(ISysTenantBLL sysTenantBLL, IDataLogBLL dataLogBLL, ISysUserBLL sysUserBLL)
        {
            this._sysTenantBLL = sysTenantBLL;
            this._dataLogBLL = dataLogBLL;
            this._sysUserBLL = sysUserBLL;
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

        public async Task<ResponseBase> TenantOtherInfoGet(TenantOtherInfoGetRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantOtherInfoGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantOtherInfoSave(TenantOtherInfoSaveRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantOtherInfoSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantChangeMaxUserCount(TenantChangeMaxUserCountRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantChangeMaxUserCount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantEtmsLogRepeal(TenantEtmsLogRepealRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantEtmsLogRepeal(request);
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

        public async Task<ResponseBase> SysTenantExDateLogPaging(SysTenantExDateLogPagingRequest request)
        {
            try
            {
                return await _dataLogBLL.SysTenantExDateLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserFeedbackPaging(UserFeedbackPagingRequest request)
        {
            try
            {
                return await _dataLogBLL.UserFeedbackPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> DangerousIpPaging(DangerousIpPagingRequest request)
        {
            try
            {
                return await _dataLogBLL.DangerousIpPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserGet(UserGetRequest request)
        {
            try
            {
                return await _sysUserBLL.UserGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserAdd(UserAddRequest request)
        {
            try
            {
                return await _sysUserBLL.UserAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserEdit(UserEditRequest request)
        {
            try
            {
                return await _sysUserBLL.UserEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserDel(UserDelRequest request)
        {
            try
            {
                return await _sysUserBLL.UserDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserGetPaging(UserGetPagingRequest request)
        {
            try
            {
                return await _sysUserBLL.UserGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserMyRoleListGet(UserMyRoleListGetRequest request)
        {
            try
            {
                return await _sysUserBLL.UserMyRoleListGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserRoleAdd(UserRoleAddRequest request)
        {
            try
            {
                return await _sysUserBLL.UserRoleAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserRoleEdit(UserRoleEditRequest request)
        {
            try
            {
                return await _sysUserBLL.UserRoleEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserRoleGet(UserRoleGetRequest request)
        {
            try
            {
                return await _sysUserBLL.UserRoleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserRoleDefaultGet(UserRoleDefaultGetRequest request)
        {
            try
            {
                return await _sysUserBLL.UserRoleDefaultGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserRoleDel(UserRoleDelRequest request)
        {
            try
            {
                return await _sysUserBLL.UserRoleDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserRoleGetPaging(UserRoleGetPagingRequest request)
        {
            try
            {
                return await _sysUserBLL.UserRoleGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
