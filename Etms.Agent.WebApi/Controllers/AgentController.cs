using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
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
    [Route("api/agent/[action]")]
    [ApiController]
    [Authorize]
    public class AgentController : ControllerBase
    {
        private readonly IAgentBLL _agentBLL;

        private readonly ISysTenantBLL _sysTenantBLL;

        private readonly ISysUpgradeMsgBLL _sysUpgradeMsgBLL;

        private readonly ISysExplainBLL _sysExplainBLL;

        private readonly ISysCommonBLL _sysCommonBLL;

        public AgentController(IAgentBLL agentBLL, ISysTenantBLL sysTenantBLL, ISysUpgradeMsgBLL sysUpgradeMsgBLL, ISysExplainBLL sysExplainBLL,
            ISysCommonBLL sysCommonBLL)
        {
            this._agentBLL = agentBLL;
            this._sysTenantBLL = sysTenantBLL;
            this._sysUpgradeMsgBLL = sysUpgradeMsgBLL;
            this._sysExplainBLL = sysExplainBLL;
            this._sysCommonBLL = sysCommonBLL;
        }

        [AllowAnonymous]
        public async Task<ResponseBase> AgentLogin(AgentLoginRequest request)
        {
            try
            {
                return await _agentBLL.AgentLogin(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentLoginInfoGet(AgentLoginInfoGetRequest request)
        {
            try
            {
                return await _agentBLL.AgentLoginInfoGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentLoginInfoGetBasc(AgentLoginInfoGetBascRequest request)
        {
            try
            {
                return await _agentBLL.AgentLoginInfoGetBasc(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentLoginPermissionGet(AgentLoginPermissionGetRequest request)
        {
            try
            {
                return await _agentBLL.AgentLoginPermissionGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentChangPwd(AgentChangPwdRequest request)
        {
            try
            {
                return await _agentBLL.AgentChangPwd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentAdd(AgentAddRequest request)
        {
            try
            {
                return await _agentBLL.AgentAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentGet(AgentGetRequest request)
        {
            try
            {
                return await _agentBLL.AgentGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentGetView(AgentGetViewRequest request)
        {
            try
            {
                return await _agentBLL.AgentGetView(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentEdit(AgentEditRequest request)
        {
            try
            {
                return await _agentBLL.AgentEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentSetUser(AgentSetUserRequest request)
        {
            try
            {
                return await _agentBLL.AgentSetUser(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentDel(AgentDelRequest request)
        {
            try
            {
                return await _agentBLL.AgentDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentPaging(AgentPagingRequest request)
        {
            try
            {
                return await _agentBLL.AgentPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentChangeSmsCount(AgentChangeSmsCountRequest request)
        {
            try
            {
                return await _agentBLL.AgentChangeSmsCount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentChangeEtmsCount(AgentChangeEtmsCountRequest request)
        {
            try
            {
                return await _agentBLL.AgentChangeEtmsCount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentOpLogPaging(AgentOpLogPagingRequest request)
        {
            try
            {
                return await _agentBLL.AgentOpLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentEtmsAccountLogPaging(AgentEtmsAccountLogPagingRequest request)
        {
            try
            {
                return await _agentBLL.AgentEtmsAccountLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AgentSmsLogPaging(AgentSmsLogPagingRequest request)
        {
            try
            {
                return await _agentBLL.AgentSmsLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> VersionAdd(VersionAddRequest request)
        {
            try
            {
                return await _agentBLL.VersionAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> VersionEdit(VersionEditRequest request)
        {
            try
            {
                return await _agentBLL.VersionEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> VersionDel(VersionDelRequest request)
        {
            try
            {
                return await _agentBLL.VersionDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> VersionGet(VersionGetRequest request)
        {
            try
            {
                return await _agentBLL.VersionGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase VersionDefaultGet(VersionDefaultGetRequest request)
        {
            try
            {
                return _agentBLL.VersionDefaultGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> VersionGetAll(VersionGetAllRequest request)
        {
            try
            {
                return await _agentBLL.VersionGetAll(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysRoleListGet(SysRoleListGetRequest request)
        {
            try
            {
                return await _agentBLL.SysRoleListGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysRoleAdd(SysRoleAddRequest request)
        {
            try
            {
                return await _agentBLL.SysRoleAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysRoleEdit(SysRoleEditRequest request)
        {
            try
            {
                return await _agentBLL.SysRoleEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysRoleGet(SysRoleGetRequest request)
        {
            try
            {
                return await _agentBLL.SysRoleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase SysRoleDefaultGet(SysRoleDefaultGetRequest request)
        {
            try
            {
                return _agentBLL.SysRoleDefaultGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysRoleDel(SysRoleDelRequest request)
        {
            try
            {
                return await _agentBLL.SysRoleDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase TenantNewCodeGet(TenantNewCodeGetRequest request)
        {
            try
            {
                return _sysTenantBLL.TenantNewCodeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantGetPaging(TenantGetPagingRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantGet(TenantGetRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantUseStatisticsGet(TenantUseStatisticsGetRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantUseStatisticsGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantGetView(TenantGetViewRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantGetView(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantEdit(TenantEditRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantEditImportant(TenantEditImportantRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantEditImportant(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantSetUser(TenantSetUserRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantSetUser(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantSetExDate(TenantSetExDateRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantSetExDate(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantDel(TenantDelRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantAdd(TenantAddRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantEtmsAccountLogPaging(TenantEtmsAccountLogPagingRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantEtmsAccountLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantSmsLogPaging(TenantSmsLogPagingRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantSmsLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantChangeSms(TenantChangeSmsRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantChangeSms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantChangeEtms(TenantChangeEtmsRequest request)
        {
            try
            {
                return await _sysTenantBLL.TenantChangeEtms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysUpgradeMsgAdd(SysUpgradeMsgAddRequest request)
        {
            try
            {
                return await _sysUpgradeMsgBLL.SysUpgradeMsgAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysUpgradeMsgDel(SysUpgradeMsgDelRequest request)
        {
            try
            {
                return await _sysUpgradeMsgBLL.SysUpgradeMsgDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysUpgradeMsgPaging(SysUpgradeMsgPagingRequest request)
        {
            try
            {
                return await _sysUpgradeMsgBLL.SysUpgradeMsgPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysExplainAdd(SysExplainAddRequest request)
        {
            try
            {
                return await _sysExplainBLL.SysExplainAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysExplainEdit(SysExplainEditRequest request)
        {
            try
            {
                return await _sysExplainBLL.SysExplainEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysExplainDel(SysExplainDelRequest request)
        {
            try
            {
                return await _sysExplainBLL.SysExplainDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysExplainPaging(SysExplainPagingRequest request)
        {
            try
            {
                return await _sysExplainBLL.SysExplainPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> EtmsGlobalConfigGet(EtmsGlobalConfigGetRequest request)
        {
            try
            {
                return await _sysCommonBLL.EtmsGlobalConfigGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> EtmsGlobalConfigSave(EtmsGlobalConfigSaveRequest request)
        {
            try
            {
                return await _sysCommonBLL.EtmsGlobalConfigSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase UploadConfigGet(AgentRequestBase request)
        {
            try
            {
                return _sysCommonBLL.UploadConfigGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> LiveTeachingConfigGet(AgentRequestBase request)
        {
            try
            {
                return await _sysCommonBLL.LiveTeachingConfigGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> LiveTeachingConfigSave(LiveTeachingConfigSaveRequest request)
        {
            try
            {
                return await _sysCommonBLL.LiveTeachingConfigSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
