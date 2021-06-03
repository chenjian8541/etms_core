using ETMS.Business.EtmsManage.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.User.Output;
using ETMS.Entity.EtmsManage.Dto.User.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Authority;
using ETMS.DataAccess.EtmsManage.Lib;
using ETMS.Entity.Config.Menu;
using ETMS.DataAccess.Lib;

namespace ETMS.Business.EtmsManage
{
    public class SysUserBLL : ISysUserBLL
    {
        private readonly ISysAgentDAL _sysAgentDAL;

        private readonly ISysUserDAL _sysUserDAL;

        private readonly ISysUserRoleDAL _sysUserRoleDAL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        private readonly ISysRoleDAL _sysRoleDAL;

        private const string DefaultRole = "默认角色";

        public SysUserBLL(ISysAgentDAL sysAgentDAL, ISysUserDAL sysUserDAL, ISysUserRoleDAL sysUserRoleDAL, ISysAgentLogDAL sysAgentLogDAL,
            ISysRoleDAL sysRoleDAL)
        {
            this._sysAgentDAL = sysAgentDAL;
            this._sysUserDAL = sysUserDAL;
            this._sysUserRoleDAL = sysUserRoleDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
            this._sysRoleDAL = sysRoleDAL;
        }

        public async Task<ResponseBase> UserGet(UserGetRequest request)
        {
            var p = await _sysUserDAL.GetUser(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            return ResponseBase.Success(new UserGetOutput()
            {
                Address = p.Address,
                AgentId = p.AgentId,
                Id = p.Id,
                IsLock = p.IsLock == EmSysAgentIsLock.IsLock,
                Name = p.Name,
                Phone = p.Phone,
                Remark = p.Remark,
                UserRoleId = p.UserRoleId
            });
        }

        public async Task<ResponseBase> UserAdd(UserAddRequest request)
        {
            var existUser = await _sysUserDAL.ExistSysUserByPhone(request.LoginAgentId, request.Phone);
            if (existUser != null)
            {
                return ResponseBase.CommonError("手机号码已存在");
            }
            var newUser = new SysUser()
            {
                IsAdmin = EmBool.False,
                IsDeleted = EmIsDeleted.Normal,
                IsLock = EmSysAgentIsLock.Normal,
                LastLoginOt = null,
                CreatedAgentId = request.LoginAgentId,
                Name = request.Name,
                Address = request.Address,
                AgentId = request.LoginAgentId,
                Ot = DateTime.Now,
                Password = CryptogramHelper.Encrypt3DES("88888888", SystemConfig.CryptogramConfig.Key),
                Phone = request.Phone,
                Remark = request.Remark,
                UserRoleId = request.UserRoleId
            };
            await _sysUserDAL.AddUser(newUser);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"新增员工:名称:{newUser.Name},手机号码,{newUser.Phone}", EmSysAgentOpLogType.UserMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserEdit(UserEditRequest request)
        {
            var userInfo = await _sysUserDAL.GetUser(request.Id);
            if (userInfo == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            var existUser = await _sysUserDAL.ExistSysUserByPhone(request.LoginAgentId, request.Phone, request.Id);
            if (existUser != null)
            {
                return ResponseBase.CommonError("手机号码已存在");
            }
            userInfo.UserRoleId = request.UserRoleId;
            userInfo.Name = request.Name;
            userInfo.Phone = request.Phone;
            userInfo.IsLock = request.IsLock ? EmSysAgentIsLock.IsLock : EmSysAgentIsLock.Normal;
            userInfo.Remark = request.Remark;
            userInfo.Address = request.Address;

            await _sysUserDAL.EditUser(userInfo);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"编辑员工:名称:{userInfo.Name},手机号码,{userInfo.Phone}", EmSysAgentOpLogType.UserMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserDel(UserDelRequest request)
        {
            var userInfo = await _sysUserDAL.GetUser(request.Id);
            if (userInfo == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            if (userInfo.IsAdmin == EmBool.True)
            {
                return ResponseBase.CommonError("无法删除主账号");
            }

            await _sysUserDAL.DelUser(request.Id);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"删除员工:名称:{userInfo.Name},手机号码,{userInfo.Phone}", EmSysAgentOpLogType.UserMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserGetPaging(UserGetPagingRequest request)
        {
            var output = new List<UserGetPagingOutput>();
            var pagingData = await _sysUserDAL.GetPaging(request);
            if (pagingData.Item1.Count() > 0)
            {
                var tempBoxAgent = new AgentDataTempBox<SysAgent>();
                var tempBoxRole = new AgentDataTempBox<SysUserRole>();
                foreach (var p in pagingData.Item1)
                {
                    var agent = await AgentComBusiness.GetAgent(tempBoxAgent, _sysAgentDAL, p.AgentId);
                    var role = DefaultRole;
                    if (p.UserRoleId > 0)
                    {
                        var myrole = await AgentComBusiness.GetUserRole(tempBoxRole, _sysUserRoleDAL, p.UserRoleId);
                        role = myrole?.Name;
                    }
                    output.Add(new UserGetPagingOutput()
                    {
                        UserRoleId = p.UserRoleId,
                        AgentId = p.AgentId,
                        Address = p.Address,
                        AgentName = agent?.Name,
                        Name = p.Name,
                        Id = p.Id,
                        IsLock = p.IsLock,
                        Phone = p.Phone,
                        Remark = p.Remark,
                        UserRoleName = role,
                        IsLockDesc = EmSysAgentIsLock.GetSysAgentIsLockDesc(p.IsLock),
                        Label = p.Name,
                        Value = p.Id,
                        IsAdmin = p.IsAdmin
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<UserGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> UserMyRoleListGet(UserMyRoleListGetRequest request)
        {
            var myRoleList = await _sysUserRoleDAL.GetMyRoles(request.LoginAgentId);
            var output = new List<UserMyRoleListGetOutput>() {
            new UserMyRoleListGetOutput(){
             Label = DefaultRole,
              Value = 0
            }
            };
            if (myRoleList.Count > 0)
            {
                foreach (var p in myRoleList)
                {
                    output.Add(new UserMyRoleListGetOutput()
                    {
                        Value = p.Id,
                        Label = p.Name
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> UserRoleAdd(UserRoleAddRequest request)
        {
            await _sysUserRoleDAL.AddRole(new SysUserRole()
            {
                AuthorityValueData = EmDataLimitType.GetAuthorityValueData(request.IsMyDataLimit),
                AuthorityValueMenu = AgentComBusiness.GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds),
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Remark = request.Remark,
                AgentId = request.LoginAgentId
            });

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"添加员工角色:{request.Name}", EmSysAgentOpLogType.UserMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserRoleEdit(UserRoleEditRequest request)
        {
            var role = await _sysUserRoleDAL.GetRole(request.Id);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            role.Name = request.Name;
            role.Remark = request.Remark;
            role.AuthorityValueMenu = AgentComBusiness.GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds);
            role.AuthorityValueData = EmDataLimitType.GetAuthorityValueData(request.IsMyDataLimit);
            await _sysUserRoleDAL.EditRole(role);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"编辑员工角色:{request.Name}", EmSysAgentOpLogType.UserMgr);
            return ResponseBase.Success();
        }

        private async Task<List<MenuConfig>> GetAgentAllMenuConfig(int agentId)
        {
            var agent = await _sysAgentDAL.GetAgent(agentId);
            var agentRole = await _sysRoleDAL.GetRole(agent.SysAgent.RoleId);
            var myAuthorityValueMenu = agentRole.AuthorityValueMenu.Split('|');
            var pageWeight = myAuthorityValueMenu[2].ToBigInteger();
            var actionWeight = myAuthorityValueMenu[1].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            var authorityCoreAction = new AuthorityCore(actionWeight);
            var allMenuConfig = EtmsHelper.DeepCopy(AgentPermissionData.MenuConfigs);
            return PermissionData.GetChildMenus(authorityCorePage, authorityCoreAction, allMenuConfig);
        }

        public async Task<ResponseBase> UserRoleGet(UserRoleGetRequest request)
        {
            var role = await _sysUserRoleDAL.GetRole(request.Id);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            var myAuthorityValueMenu = role.AuthorityValueMenu.Split('|');
            var pageWeight = myAuthorityValueMenu[0].ToBigInteger();
            var actionWeight = myAuthorityValueMenu[1].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            var authorityCoreAction = new AuthorityCore(actionWeight);
            var myMenuConfigs = await GetAgentAllMenuConfig(request.LoginAgentId);
            AgentComBusiness.MenuConfigsHandle(myMenuConfigs, authorityCorePage, authorityCoreAction);
            return ResponseBase.Success(new UserRoleGetOutput()
            {
                Name = role.Name,
                Remark = role.Remark,
                Menus = AgentComBusiness.GetSysMenuViewOutputs(myMenuConfigs),
                IsDataLimit = EmDataLimitType.GetIsDataLimit(role.AuthorityValueData),
                Id = role.Id
            });

        }

        public async Task<ResponseBase> UserRoleDefaultGet(UserRoleDefaultGetRequest request)
        {
            var myMenuConfigs = await GetAgentAllMenuConfig(request.LoginAgentId);
            return ResponseBase.Success(AgentComBusiness.GetSysMenuViewOutputs(myMenuConfigs));
        }

        public async Task<ResponseBase> UserRoleDel(UserRoleDelRequest request)
        {
            var role = await _sysUserRoleDAL.GetRole(request.Id);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            if (await _sysUserRoleDAL.IsCanNotDel(request.Id))
            {
                return ResponseBase.CommonError("此角色有对应的员工，无法删除");
            }
            await _sysUserRoleDAL.DelRole(request.Id);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"删除员工角色:{role.Name}", EmSysAgentOpLogType.UserMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserRoleGetPaging(UserRoleGetPagingRequest request)
        {
            var output = new List<UserRoleGetPagingOutput>();
            var pagingData = await _sysUserRoleDAL.GetPaging(request);
            if (pagingData.Item1.Count() > 0)
            {
                var tempBoxAgent = new AgentDataTempBox<SysAgent>();
                foreach (var p in pagingData.Item1)
                {
                    var agent = await AgentComBusiness.GetAgent(tempBoxAgent, _sysAgentDAL, p.AgentId);
                    output.Add(new UserRoleGetPagingOutput()
                    {
                        AgentName = agent?.Name,
                        Id = p.Id,
                        Name = p.Name,
                        DataLimitDesc = EmDataLimitType.GetIsDataLimit(p.AuthorityValueData) ? "是" : "否",
                        Remark = p.Remark
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<UserRoleGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
