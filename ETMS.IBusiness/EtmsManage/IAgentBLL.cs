using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.Agent.Output;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface IAgentBLL
    {
        Task<ResponseBase> AgentLogin(AgentLoginRequest request);

        Task<ResponseBase> CheckAgentLogin(AgentRequestBase request);

        Task<ResponseBase> AgentLoginInfoGet(AgentLoginInfoGetRequest request);

        Task<ResponseBase> AgentLoginInfoGetBasc(AgentLoginInfoGetBascRequest request);

        Task<ResponseBase> AgentLoginPermissionGet(AgentLoginPermissionGetRequest request);

        Task<ResponseBase> AgentChangPwd(AgentChangPwdRequest request);

        Task<ResponseBase> AgentAdd(AgentAddRequest request);

        Task<ResponseBase> AgentGet(AgentGetRequest request);

        Task<ResponseBase> AgentGetView(AgentGetViewRequest request);

        Task<ResponseBase> AgentEdit(AgentEditRequest request);

        Task<ResponseBase> AgentSetUser(AgentSetUserRequest request);

        Task<ResponseBase> AgentDel(AgentDelRequest request);

        Task<ResponseBase> AgentPaging(AgentPagingRequest request);

        Task<ResponseBase> AgentChangeSmsCount(AgentChangeSmsCountRequest request);

        Task<ResponseBase> AgentChangeEtmsCount(AgentChangeEtmsCountRequest request);

        Task<ResponseBase> AgentOpLogPaging(AgentOpLogPagingRequest request);

        Task<ResponseBase> AgentEtmsAccountLogPaging(AgentEtmsAccountLogPagingRequest request);

        Task<ResponseBase> AgentSmsLogPaging(AgentSmsLogPagingRequest request);

        Task<ResponseBase> VersionAdd(VersionAddRequest request);

        Task<ResponseBase> VersionEdit(VersionEditRequest request);

        Task<ResponseBase> VersionDel(VersionDelRequest request);

        Task<ResponseBase> VersionGet(VersionGetRequest request);

        ResponseBase VersionDefaultGet(VersionDefaultGetRequest request);

        Task<ResponseBase> VersionGetAll(VersionGetAllRequest request);

        Task<ResponseBase> SysRoleListGet(SysRoleListGetRequest request);

        Task<ResponseBase> SysRoleAdd(SysRoleAddRequest request);

        Task<ResponseBase> SysRoleEdit(SysRoleEditRequest request);

        Task<ResponseBase> SysRoleGet(SysRoleGetRequest request);

        ResponseBase SysRoleDefaultGet(SysRoleDefaultGetRequest request);

        Task<ResponseBase> SysRoleDel(SysRoleDelRequest request);
    }
}
