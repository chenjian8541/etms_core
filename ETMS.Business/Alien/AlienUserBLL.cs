using ETMS.Authority;
using ETMS.Business.Common;
using ETMS.Business.EtmsManage.Common;
using ETMS.DataAccess.Alien.Lib;
using ETMS.Entity.Alien.Dto.User.Output;
using ETMS.Entity.Alien.Dto.User.Request;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.Alien;
using ETMS.IBusiness.Alien;
using ETMS.IDataAccess.Alien;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using ETMS.Business.Alien.Common;
using ETMS.Entity.Alien.Common;
using ETMS.Entity.View.Alien;
using System.Text;

namespace ETMS.Business.Alien
{
    public class AlienUserBLL : IAlienUserBLL
    {
        private readonly IMgHeadDAL _mgHeadDAL;

        private readonly IMgUserDAL _mgUserDAL;

        private readonly IMgUserOpLogDAL _mgUserOpLogDAL;

        private readonly IMgRoleDAL _mgRoleDAL;

        private readonly IMgOrganizationDAL _mgOrganizationDAL;

        private readonly ISysTenantDAL _sysTenantDAL;
        public AlienUserBLL(IMgHeadDAL mgHeadDAL, IMgUserDAL mgUserDAL, IMgUserOpLogDAL mgUserOpLogDAL,
            IMgRoleDAL mgRoleDAL, IMgOrganizationDAL mgOrganizationDAL, ISysTenantDAL sysTenantDAL)
        {
            this._mgHeadDAL = mgHeadDAL;
            this._mgUserDAL = mgUserDAL;
            this._mgUserOpLogDAL = mgUserOpLogDAL;
            this._mgRoleDAL = mgRoleDAL;
            this._mgOrganizationDAL = mgOrganizationDAL;
            this._sysTenantDAL = sysTenantDAL;
        }

        public void InitHeadId(int headId)
        {
            this.InitDataAccess(headId, _mgUserDAL, _mgUserOpLogDAL, _mgRoleDAL,
                _mgOrganizationDAL);
        }

        public async Task<ResponseBase> ChangPwd(ChangPwdRequest request)
        {
            var userInfo = await _mgUserDAL.GetUser(request.LoginUserId);
            userInfo.Password = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _mgUserDAL.EditUser(userInfo);
            await _mgUserOpLogDAL.AddUserLog(request, $"员工:{userInfo.Name},手机号:{userInfo.Phone}修改密码", EmMgUserOperationType.UserChangePwd);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ChangUserPwd(ChangUserPwdRequest request)
        {
            var userInfo = await _mgUserDAL.GetUser(request.CId);
            userInfo.Password = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _mgUserDAL.EditUser(userInfo);
            await _mgUserOpLogDAL.AddUserLog(request, $"修改员工:{userInfo.Name},手机号:{userInfo.Phone}密码", EmMgUserOperationType.UserMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> RoleListGet(RoleListGetRequest request)
        {
            var roles = await _mgRoleDAL.GetRoles();
            return ResponseBase.Success(new RoleListGetOutput()
            {
                RoleLists = roles.Select(p => new RoleListView()
                {
                    CId = p.Id,
                    Name = p.Name,
                    Remark = p.Remark,
                    Value = p.Id,
                    Label = p.Name,
                    DataLimitDesc = EmDataLimitType.GetIsDataLimit(p.AuthorityValueData) ? "是" : "否"
                }).ToList()
            });
        }

        public async Task<ResponseBase> RoleAdd(RoleAddRequest request)
        {
            await _mgRoleDAL.AddRole(new MgRole()
            {
                AuthorityValueData = EmDataLimitType.GetAuthorityValueData(request.IsMyDataLimit),
                AuthorityValueMenu = ComBusiness4.GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds),
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Remark = request.Remark,
                HeadId = request.LoginHeadId
            });
            await _mgUserOpLogDAL.AddUserLog(request, $"添加角色-{request.Name}", EmMgUserOperationType.RoleMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> RoleEdit(RoleEditRequest request)
        {
            var role = await _mgRoleDAL.GetRole(request.CId);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            role.Name = request.Name;
            role.Remark = request.Remark;
            role.AuthorityValueMenu = ComBusiness4.GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds);
            role.AuthorityValueData = EmDataLimitType.GetAuthorityValueData(request.IsMyDataLimit);
            await _mgRoleDAL.EditRole(role);
            await _mgUserOpLogDAL.AddUserLog(request, $"编辑角色-{request.Name}", EmMgUserOperationType.RoleMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> RoleGet(RoleGetRequest request)
        {
            var role = await _mgRoleDAL.GetRole(request.CId);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            var myAuthorityValueMenu = role.AuthorityValueMenu.Split('|');
            var pageWeight = myAuthorityValueMenu[0].ToBigInteger();
            var actionWeight = myAuthorityValueMenu[1].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            var authorityCoreAction = new AuthorityCore(actionWeight);
            var myAllMenus = EtmsHelper.DeepCopy(AlienPermissionData.MenuConfigs);
            ComBusiness4.MenuConfigsHandle(myAllMenus, authorityCorePage, authorityCoreAction);
            return ResponseBase.Success(new RoleGetOutput()
            {
                Name = role.Name,
                Remark = role.Remark,
                Menus = ComBusiness4.GetRoleMenuViewOutputs(myAllMenus),
                IsDataLimit = EmDataLimitType.GetIsDataLimit(role.AuthorityValueData)
            });
        }

        public ResponseBase RoleDefaultGet(RoleDefaultGetRequest request)
        {
            var myAllMenus = EtmsHelper.DeepCopy(AlienPermissionData.MenuConfigs);
            return ResponseBase.Success(ComBusiness4.GetRoleMenuViewOutputs(myAllMenus));
        }

        public async Task<ResponseBase> RoleDel(RoleDelRequest request)
        {
            var isHaveUser = await _mgUserDAL.ExistRole(request.CId);
            if (isHaveUser)
            {
                return ResponseBase.CommonError("此角色有对应的员工，无法删除");
            }
            await _mgRoleDAL.DelRole(request.CId);
            await _mgUserOpLogDAL.AddUserLog(request, $"删除角色-{request.Name}", EmMgUserOperationType.RoleMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> OrgGetAll(OrgGetAllRequest request)
        {
            var myHead = await _mgHeadDAL.GetMgHead(request.LoginHeadId);
            var firstHead = new MgOrganizationView()
            {
                Id = 0,
                Label = myHead.Name,
                Name = myHead.Name,
                ParentId = 0,
                ParentsAll = string.Empty,
                Remark = string.Empty,
                UserCount = 0,
                Children = new List<MgOrganizationView>()
            };
            var allOrgBucket = await _mgOrganizationDAL.GetOrganizationBucket();
            if (allOrgBucket != null && allOrgBucket.MgOrganizationView != null)
            {
                firstHead.Children = allOrgBucket.MgOrganizationView;
            }
            return ResponseBase.Success(new List<MgOrganizationView>() { firstHead });
        }

        public async Task<ResponseBase> OrgAdd(OrgAddRequest request)
        {
            var parentsAll = string.Empty;
            if (request.ParentId > 0)
            {
                var parentData = await _mgOrganizationDAL.GetOrganization(request.ParentId);
                if (parentData == null)
                {
                    return ResponseBase.CommonError("上级组织不存在");
                }
                if (string.IsNullOrEmpty(parentData.ParentsAll))
                {
                    parentsAll = $",{request.ParentId},";
                }
                else
                {
                    parentsAll = $"{parentData.ParentsAll}{request.ParentId},";
                }
            }
            var entity = new MgOrganization()
            {
                HeadId = request.LoginHeadId,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                ParentsAll = parentsAll,
                ParentId = request.ParentId,
                Remark = request.Remark,
                UserCount = 0
            };
            await _mgOrganizationDAL.AddOrganization(entity);

            await _mgUserOpLogDAL.AddUserLog(request, $"添加组织-{request.Name}", EmMgUserOperationType.OrgMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> OrgEdit(OrgEditRequest request)
        {
            var myOrg = await _mgOrganizationDAL.GetOrganization(request.CId);
            if (myOrg == null)
            {
                return ResponseBase.CommonError("组织不存在");
            }
            myOrg.Name = request.Name;
            myOrg.Remark = request.Remark;
            await _mgOrganizationDAL.EditOrganization(myOrg);

            await _mgUserOpLogDAL.AddUserLog(request, $"编辑组织-{request.Name}", EmMgUserOperationType.OrgMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> OrgDel(OrgDelRequest request)
        {
            var myOrg = await _mgOrganizationDAL.GetOrganization(request.CId);
            if (myOrg == null)
            {
                return ResponseBase.CommonError("组织不存在");
            }
            await _mgOrganizationDAL.DelOrganization(request.CId);

            await _mgUserOpLogDAL.AddUserLog(request, $"删除组织-{request.Name}", EmMgUserOperationType.OrgMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserGetPaging(UserGetPagingRequest request)
        {
            var pagingData = await _mgUserDAL.GetPaging(request);
            var output = new List<UserGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var allOrgBucket = await _mgOrganizationDAL.GetOrganizationBucket();
                var allOrg = allOrgBucket.AllOrganization;
                var allRole = await _mgRoleDAL.GetRoles();
                var tempBoxTenant = new AgentDataTempBox<SysTenant>();
                foreach (var p in pagingData.Item1)
                {
                    var orgName = string.Empty;
                    var roleName = string.Empty;
                    if (p.OrganizationId != null)
                    {
                        var myOrg = allOrg.FirstOrDefault(j => j.Id == p.OrganizationId.Value);
                        orgName = myOrg?.Name;
                    }
                    if (p.MgRoleId != null)
                    {
                        var myRole = allRole.FirstOrDefault(j => j.Id == p.MgRoleId.Value);
                        roleName = myRole?.Name;
                    }
                    var jobAtTenantList = new List<JobAtTenantOutput>();
                    if (!string.IsNullOrEmpty(p.JobAtTenants))
                    {
                        var myAllTenants = p.JobAtTenants.Split(',');
                        foreach (var j in myAllTenants)
                        {
                            if (string.IsNullOrEmpty(j))
                            {
                                continue;
                            }
                            var myTenant = await AgentComBusiness.GetTenant(tempBoxTenant, _sysTenantDAL, j.ToInt());
                            if (myTenant == null)
                            {
                                continue;
                            }
                            jobAtTenantList.Add(new JobAtTenantOutput()
                            {
                                Name = myTenant.Name,
                                TenantId = myTenant.Id
                            });
                        }

                    }
                    output.Add(new UserGetPagingOutput()
                    {
                        Name = p.Name,
                        MgRoleId = p.MgRoleId,
                        Address = p.Address,
                        CId = p.Id,
                        Gender = p.Gender,
                        IsLock = p.IsLock,
                        LastLoginOt = p.LastLoginOt,
                        OrganizationId = p.OrganizationId,
                        OrgName = orgName,
                        RoleName = roleName,
                        Ot = p.Ot,
                        Phone = p.Phone,
                        Remark = p.Remark,
                        JobAtTenantList = jobAtTenantList,
                        JobAtTenantListDesc = jobAtTenantList.Any() ? string.Join(',', jobAtTenantList.Select(j => j.Name)) : ""
                    }); ;
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<UserGetPagingOutput>(pagingData.Item2, output));
        }

        private async Task<ResponseBase> UserGetInfo(long userId)
        {
            var myUser = await _mgUserDAL.GetUser(userId);
            if (myUser == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            var orgName = string.Empty;
            var roleName = string.Empty;
            var jobAtTenantList = new List<JobAtTenantOutput>();
            if (myUser.OrganizationId != null && myUser.OrganizationId > 0)
            {
                var myOrg = await _mgOrganizationDAL.GetOrganization(myUser.OrganizationId.Value);
                orgName = myOrg?.Name;
            }
            if (myUser.MgRoleId != null && myUser.MgRoleId > 0)
            {
                var myRole = await _mgRoleDAL.GetRole(myUser.MgRoleId.Value);
                roleName = myRole?.Name;
            }
            if (!string.IsNullOrEmpty(myUser.JobAtTenants))
            {
                var myAllTenants = myUser.JobAtTenants.Split(',');
                foreach (var j in myAllTenants)
                {
                    if (string.IsNullOrEmpty(j))
                    {
                        continue;
                    }
                    var myTenant = await _sysTenantDAL.GetTenant(j.ToInt());
                    if (myTenant == null)
                    {
                        continue;
                    }
                    jobAtTenantList.Add(new JobAtTenantOutput()
                    {
                        Name = myTenant.Name,
                        TenantId = myTenant.Id
                    });
                }
            }
            return ResponseBase.Success(new UserGetOutput()
            {
                Address = myUser.Address,
                CId = myUser.Id,
                Gender = myUser.Gender,
                IsLock = myUser.IsLock,
                JobAtTenantList = jobAtTenantList,
                LastLoginOt = myUser.LastLoginOt,
                MgRoleId = myUser.MgRoleId,
                Name = myUser.Name,
                OrganizationId = myUser.OrganizationId,
                OrgName = orgName,
                Ot = myUser.Ot,
                Phone = myUser.Phone,
                Remark = myUser.Remark,
                RoleName = roleName
            });
        }

        public async Task<ResponseBase> UserGet(UserGetRequest request)
        {
            return await UserGetInfo(request.CId);
        }

        public async Task<ResponseBase> UserGetSelf(AlienRequestBase request)
        {
            return await UserGetInfo(request.LoginUserId);
        }

        public async Task<ResponseBase> UserAdd(UserAddRequest request)
        {
            var myOldPhoneUser = await _mgUserDAL.ExistMgUserByPhone(request.Phone);
            if (myOldPhoneUser != null)
            {
                return ResponseBase.CommonError("手机号码已存在");
            }
            var orgParentsAll = string.Empty;
            if (request.OrgId > 0)
            {
                var myOgr = await _mgOrganizationDAL.GetOrganization(request.OrgId.Value);
                orgParentsAll = myOgr?.ParentsAll;
            }
            var user = new MgUser()
            {
                MgRoleId = request.RoleId,
                Address = request.Address,
                Gender = request.Gender,
                HeadId = request.LoginHeadId,
                IsAdmin = EmBool.False,
                IsDeleted = EmIsDeleted.Normal,
                IsLock = request.IsLock,
                JobAtTenants = EtmsHelper.GetMuIds(request.JobAtTenants),
                LastLoginOt = null,
                Name = request.Name,
                OrganizationId = request.OrgId,
                OrgParentsAll = orgParentsAll,
                Ot = DateTime.Now,
                Password = CryptogramHelper.Encrypt3DES(SystemConfig.ComConfig.DefaultPwd, SystemConfig.CryptogramConfig.Key),
                Phone = request.Phone,
                Remark = request.Remark
            };
            await _mgUserDAL.AddUser(user);

            await _mgUserOpLogDAL.AddUserLog(request, $"添加员工-{request.Name}", EmMgUserOperationType.UserMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserDel(UserDelRequest request)
        {
            var user = await _mgUserDAL.GetUser(request.CId);
            if (user == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            if (user.IsAdmin == EmBool.True)
            {
                return ResponseBase.CommonError("此员工无法删除");
            }
            await _mgUserDAL.DelUser(request.CId);

            await _mgUserOpLogDAL.AddUserLog(request, $"删除员工-{user.Name}", EmMgUserOperationType.UserMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserEdit(UserEditRequest request)
        {
            var user = await _mgUserDAL.GetUser(request.CId);
            if (user == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            var myOldPhoneUser = await _mgUserDAL.ExistMgUserByPhone(request.Phone, request.CId);
            if (myOldPhoneUser != null)
            {
                return ResponseBase.CommonError("手机号码已存在");
            }
            var orgParentsAll = string.Empty;
            if (request.OrgId > 0)
            {
                var myOgr = await _mgOrganizationDAL.GetOrganization(request.OrgId.Value);
                orgParentsAll = myOgr?.ParentsAll;
            }

            user.MgRoleId = request.RoleId;
            user.Address = request.Address;
            user.Gender = request.Gender;
            user.IsLock = request.IsLock;
            user.JobAtTenants = EtmsHelper.GetMuIds(request.JobAtTenants);
            user.Name = request.Name;
            user.OrganizationId = request.OrgId;
            user.OrgParentsAll = orgParentsAll;
            user.Phone = request.Phone;
            user.Remark = request.Remark;
            await _mgUserDAL.EditUser(user);

            await _mgUserOpLogDAL.AddUserLog(request, $"编辑员工-{user.Name}", EmMgUserOperationType.UserMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserOperationLogGetPaging(UserOperationLogGetPagingRequest request)
        {
            var pagingData = await _mgUserOpLogDAL.GetPaging(request);
            var output = new List<UserOperationLogGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxUser = new AlienDataTempBox<MgUser>();
                foreach (var p in pagingData.Item1)
                {
                    var myUser = await AlienComBusiness.GetUser(tempBoxUser, _mgUserDAL, p.MgUserId);
                    output.Add(new UserOperationLogGetPagingOutput()
                    {
                        ClientType = p.ClientType,
                        OpContent = p.OpContent,
                        Ot = p.Ot,
                        TypeDesc = EmMgUserOperationType.GetMgUserOperationTypeDesc(p.Type),
                        UserName = myUser?.Name,
                        UserPhone = myUser?.Phone
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<UserOperationLogGetPagingOutput>(pagingData.Item2, output));
        }

        public ResponseBase UserLogTypeGet(AlienRequestBase request)
        {
            return ResponseBase.Success(EmMgUserOperationType.AllOperationTypes);
        }
    }
}
