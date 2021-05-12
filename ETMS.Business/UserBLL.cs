using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.User.Output;
using ETMS.Entity.Dto.User.Request;
using ETMS.Entity.Enum;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Authority;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Database.Source;
using System.Collections.Generic;
using System.Text;
using ETMS.Business.Common;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Config.Router;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;

namespace ETMS.Business
{
    public class UserBLL : IUserBLL
    {
        private readonly IUserDAL _etUserDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IUserChangePwdSmsCodeDAL _userChangePwdSmsCodeDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IRoleDAL _roleDAL;

        private readonly ISubjectDAL _subjectDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IAppAuthorityDAL _appAuthorityDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        public UserBLL(IHttpContextAccessor httpContextAccessor, IUserChangePwdSmsCodeDAL userChangePwdSmsCodeDAL,
            IAppConfigurtaionServices appConfigurtaionServices, IUserDAL etUserDAL, IUserOperationLogDAL userOperationLogDAL,
            IRoleDAL roleDAL, ISubjectDAL subjectDAL, ISysTenantDAL sysTenantDAL, IAppAuthorityDAL appAuthorityDAL,
            IEventPublisher eventPublisher, ITenantConfigDAL tenantConfigDAL)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._etUserDAL = etUserDAL;
            this._userChangePwdSmsCodeDAL = userChangePwdSmsCodeDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._roleDAL = roleDAL;
            this._subjectDAL = subjectDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._appAuthorityDAL = appAuthorityDAL;
            this._eventPublisher = eventPublisher;
            this._tenantConfigDAL = tenantConfigDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _etUserDAL, _userOperationLogDAL, _roleDAL, _subjectDAL, _tenantConfigDAL);
        }

        public async Task<ResponseBase> GetLoginInfo(GetLoginInfoRequest request)
        {
            var userInfo = await _etUserDAL.GetUser(request.LoginUserId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var myAllRouteConfigs = await _appAuthorityDAL.GetTenantRouteConfig(request.LoginTenantId);
            var config = await _tenantConfigDAL.GetTenantConfig();
            return ResponseBase.Success(new GetLoginInfoOutput()
            {
                Name = userInfo.Name,
                NickName = userInfo.NickName,
                AvatarKey = userInfo.Avatar,
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, userInfo.Avatar),
                Phone = userInfo.Phone,
                RouteConfigs = ComBusiness.GetRouteConfigs(myAllRouteConfigs, role.AuthorityValueMenu, userInfo.IsAdmin),
                OrgName = tenant.Name,
                RoleSetting = ComBusiness3.AnalyzeNoticeSetting(role.NoticeSetting, userInfo.IsAdmin),
                TenantConfig = config
            });
        }

        public async Task<ResponseBase> GetLoginInfoH5(GetLoginInfoRequest request)
        {
            var userInfo = await _etUserDAL.GetUser(request.LoginUserId);
            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            var config = await _tenantConfigDAL.GetTenantConfig();
            return ResponseBase.Success(new GetLoginInfoH5Output()
            {
                Name = userInfo.Name,
                NickName = userInfo.NickName,
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, userInfo.Avatar),
                Phone = userInfo.Phone,
                OrgName = tenant.Name,
                Permission = ComBusiness.GetPermissionOutputH5(role.AuthorityValueMenu, userInfo.IsAdmin),
                RoleSetting = ComBusiness3.AnalyzeNoticeSetting(role.NoticeSetting, userInfo.IsAdmin),
                TenantConfig = config
            });
        }

        public async Task<ResponseBase> GetLoginPermission(GetLoginPermissionRequest request)
        {
            var user = await _etUserDAL.GetUser(request.LoginUserId);
            var role = await _roleDAL.GetRole(user.RoleId);
            var myAllMenus = await _appAuthorityDAL.GetTenantMenuConfig(request.LoginTenantId);
            return ResponseBase.Success(ComBusiness.GetPermissionOutput(myAllMenus, role.AuthorityValueMenu, user.IsAdmin));
        }

        public async Task<ResponseBase> UpdateUserInfo(UpdateUserInfoRequest request)
        {
            var userInfo = await _etUserDAL.GetUser(request.LoginUserId);
            var oldAvatar = userInfo.Avatar;

            userInfo.Avatar = request.AvatarKey;
            userInfo.NickName = request.NickName;
            await _etUserDAL.EditUser(userInfo);
            if (oldAvatar != request.AvatarKey)
            {
                AliyunOssUtil.DeleteObject(oldAvatar);
            }

            await _userOperationLogDAL.AddUserLog(request, $"用户:{userInfo.Name},手机号:{userInfo.Phone}修改基本信息", EmUserOperationType.UserUpdateInfo);
            return ResponseBase.Success();
        }

        //public async Task<ResponseBase> ChangPwdSendSms(ChangPwdSendSmsRequest request)
        //{
        //    var userInfo = await _etUserDAL.GetUser(request.LoginUserId);
        //    var smsCode = RandomHelper.GetSmsCode();
        //    var smsContent = string.Format(_appConfigurtaionServices.AppSettings.SmsConfig.SmsTemplate.ChangePwd, smsCode);
        //    var isSendSms = _smsService.SendSms(userInfo.Phone, smsContent);
        //    if (!isSendSms)
        //    {
        //        return ResponseBase.CommonError("发送短信失败,请稍后再试");
        //    }
        //    this._userChangePwdSmsCodeDAL.AddUserChangePwdSmsCode(request.LoginTenantId, request.LoginUserId, smsCode);
        //    return ResponseBase.Success();
        //}

        /// <summary>
        /// 修改自己的密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase> ChangPwd(ChangPwdRequest request)
        {
            //var changePwdSms = _userChangePwdSmsCodeDAL.GetUserChangePwdSmsCode(request.LoginTenantId, request.LoginUserId);
            //if (changePwdSms == null || changePwdSms.ExpireAtTime < DateTime.Now || changePwdSms.SmsCode != request.SmsCode)
            //{
            //    return ResponseBase.CommonError("验证码错误");
            //}
            var userInfo = await _etUserDAL.GetUser(request.LoginUserId);
            userInfo.Password = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _etUserDAL.EditUser(userInfo);
            _userChangePwdSmsCodeDAL.RemoveUserChangePwdSmsCode(request.LoginTenantId, request.LoginUserId);
            await _userOperationLogDAL.AddUserLog(request, $"用户:{userInfo.Name},手机号:{userInfo.Phone}修改密码", EmUserOperationType.UserChangePwd);
            return ResponseBase.Success();
        }

        /// <summary>
        /// 修改用户的密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase> ChangUserPwd(ChangUserPwdRequest request)
        {
            var userInfo = await _etUserDAL.GetUser(request.CId);
            userInfo.Password = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _etUserDAL.EditUser(userInfo);
            await _userOperationLogDAL.AddUserLog(request, $"修改用户:{userInfo.Name},手机号:{userInfo.Phone}密码", EmUserOperationType.UserUpdateInfo);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> RoleListGet(RoleListGetRequest request)
        {
            var roles = await _roleDAL.GetRole();
            return ResponseBase.Success(new RoleListGetOutput()
            {
                RoleLists = roles.Select(p => new RoleListViewOutput()
                {
                    CId = p.Id,
                    Name = p.Name,
                    Remark = p.Remark,
                    DataLimitDesc = EmDataLimitType.GetIsDataLimit(p.AuthorityValueData) ? "是" : "否",
                    Value = p.Id,
                    Label = p.Name
                }).ToList()
            });
        }

        private string GetNoticeSetting(RoleNoticeSettingRequest request)
        {
            var mySetting = new List<int>();
            if (request.IsStudentLeaveApply)
            {
                mySetting.Add(RoleOtherSetting.StudentLeaveApply);
            }
            if (request.IsStudentContractsNotArrived)
            {
                mySetting.Add(RoleOtherSetting.StudentContractsNotArrived);
            }
            if (request.IsTryCalssApply)
            {
                mySetting.Add(RoleOtherSetting.TryCalssApply);
            }
            if (request.IsReceiveInteractiveStudent)
            {
                mySetting.Add(RoleOtherSetting.ReceiveInteractiveStudent);
            }
            if (request.IsAllowAppLogin)
            {
                mySetting.Add(RoleOtherSetting.AllowAppLogin);
            }
            if (request.IsAllowLookStatistics)
            {
                mySetting.Add(RoleOtherSetting.AllowLookStatistics);
            }
            return EtmsHelper.GetMuIds(mySetting);
        }

        public async Task<ResponseBase> RoleAdd(RoleAddRequest request)
        {
            await _roleDAL.AddRole(new EtRole()
            {
                AuthorityValueData = EmDataLimitType.GetAuthorityValueData(request.IsMyDataLimit),
                AuthorityValueMenu = GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds),
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Remark = request.Remark,
                TenantId = request.LoginTenantId,
                NoticeSetting = GetNoticeSetting(request.RoleNoticeSetting)
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加角色-{request.Name}", EmUserOperationType.RoleSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> RoleEdit(RoleEditRequest request)
        {
            var role = await _roleDAL.GetRole(request.CId);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            role.Name = request.Name;
            role.Remark = request.Remark;
            role.AuthorityValueMenu = GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds);
            role.AuthorityValueData = EmDataLimitType.GetAuthorityValueData(request.IsMyDataLimit);
            role.NoticeSetting = GetNoticeSetting(request.RoleNoticeSetting);
            await _roleDAL.EditRole(role);
            await _userOperationLogDAL.AddUserLog(request, $"编辑角色-{request.Name}", EmUserOperationType.RoleSetting);
            return ResponseBase.Success();
        }

        private string GetAuthorityValueMenu(List<int> pageMenus, List<int> actionMenus, List<int> pageRouteIds)
        {
            return $"{GetAuthorityValue(pageMenus.ToArray())}|{GetAuthorityValue(actionMenus.ToArray())}|{GetAuthorityValue(pageRouteIds.ToArray())}";
        }

        /// <summary>
        /// 通过选择的菜单ID，计算权值
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private string GetAuthorityValue(int[] ids)
        {
            if (ids == null || !ids.Any())
            {
                return string.Empty;
            }
            var authorityCore = new AuthorityCore();
            var weightSum = authorityCore.AuthoritySum(ids);
            return weightSum.ToString();
        }

        public async Task<ResponseBase> RoleGet(RoleGetRequest request)
        {
            var role = await _roleDAL.GetRole(request.CId);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            var myAuthorityValueMenu = role.AuthorityValueMenu.Split('|');
            var pageWeight = myAuthorityValueMenu[0].ToBigInteger();
            var actionWeight = myAuthorityValueMenu[1].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            var authorityCoreAction = new AuthorityCore(actionWeight);
            var myAllMenus = await _appAuthorityDAL.GetTenantMenuConfig(request.LoginTenantId);
            MenuConfigsHandle(myAllMenus, authorityCorePage, authorityCoreAction);
            return ResponseBase.Success(new RoleGetOutput()
            {
                Name = role.Name,
                Remark = role.Remark,
                Menus = GetRoleMenuViewOutputs(myAllMenus),
                IsDataLimit = EmDataLimitType.GetIsDataLimit(role.AuthorityValueData),
                RoleNoticeSetting = ComBusiness3.AnalyzeNoticeSetting(role.NoticeSetting)
            });
        }

        public async Task<ResponseBase> RoleDefaultGet(RoleDefaultGetRequest request)
        {
            var myAllMenus = await _appAuthorityDAL.GetTenantMenuConfig(request.LoginTenantId);
            return ResponseBase.Success(GetRoleMenuViewOutputs(myAllMenus));
        }

        private List<RoleMenuViewOutput> GetRoleMenuViewOutputs(List<MenuConfig> menuConfigs)
        {
            var output = new List<RoleMenuViewOutput>();
            var index = 1;
            foreach (var p in menuConfigs)
            {
                var item = new RoleMenuViewOutput()
                {
                    ActionCheck = new List<int>(),
                    ActionItems = new List<RoleMenuItem>(),
                    PageCheck = new List<int>(),
                    PageItems = new List<RoleMenuItem>(),
                    Index = index
                };
                index++;
                var thisPageItem = new RoleMenuItem()
                {
                    Children = new List<RoleMenuItem>(),
                    Id = p.Id,
                    Label = p.Name,
                    Type = p.Type
                };
                item.PageItems.Add(thisPageItem);
                if (p.IsOwner)
                {
                    item.PageCheck.Add(p.Id);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    AddChildrenPage(p.ChildrenPage, thisPageItem, item);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    AddChildrenCheck(p.ChildrenAction, item);
                }
                output.Add(item);
            }
            return output;
        }

        private void AddChildrenCheck(List<MenuConfig> ChildrenAction, RoleMenuViewOutput itemOutput)
        {
            if (itemOutput.ActionItems.Count == 0)
            {
                itemOutput.ActionItems.Add(new RoleMenuItem()
                {
                    Children = new List<RoleMenuItem>(),
                    Id = 0,
                    Label = "全选",
                    Type = MenuType.Action
                });
            }
            foreach (var p in ChildrenAction)
            {
                itemOutput.ActionItems[0].Children.Add(new RoleMenuItem()
                {
                    Id = p.ActionId,
                    Label = p.Name,
                    Type = p.Type
                });
                if (p.IsOwner)
                {
                    itemOutput.ActionCheck.Add(p.ActionId);
                }
            }
        }

        private void AddChildrenPage(List<MenuConfig> childrenPage, RoleMenuItem item, RoleMenuViewOutput itemOutput)
        {
            foreach (var p in childrenPage)
            {
                var thisRoleMenuItem = new RoleMenuItem()
                {
                    Children = new List<RoleMenuItem>(),
                    Id = p.Id,
                    Label = p.Name,
                    Type = p.Type
                };
                item.Children.Add(thisRoleMenuItem);
                if (p.IsOwner)
                {
                    itemOutput.PageCheck.Add(p.Id);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    AddChildrenPage(p.ChildrenPage, thisRoleMenuItem, itemOutput);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    AddChildrenCheck(p.ChildrenAction, itemOutput);
                }
            }
        }

        private void MenuConfigsHandle(List<MenuConfig> myMenuConfigs, AuthorityCore authorityCorePage, AuthorityCore authorityCoreAction)
        {
            foreach (var p in myMenuConfigs)
            {
                if (p.Type == MenuType.Page)
                {
                    p.IsOwner = authorityCorePage.Validation(p.Id);
                }
                else
                {
                    p.IsOwner = authorityCoreAction.Validation(p.ActionId);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    MenuConfigsHandle(p.ChildrenPage, authorityCorePage, authorityCoreAction);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    MenuConfigsHandle(p.ChildrenAction, authorityCorePage, authorityCoreAction);
                }
            }
        }

        public async Task<ResponseBase> RoleDel(RoleDelRequest request)
        {
            var isHaveUser = await _etUserDAL.ExistRole(request.CId);
            if (isHaveUser)
            {
                return ResponseBase.CommonError("此角色有对应的员工，无法删除");
            }
            await _roleDAL.DelRole(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"删除角色-{request.Name}", EmUserOperationType.RoleSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserAdd(UserAddRequest request)
        {
            if (await _etUserDAL.ExistUserPhone(request.Phone))
            {
                return ResponseBase.CommonError("手机号码已存在");
            }

            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (tenant.MaxUserCount > 0)
            {
                var userCount = await _etUserDAL.GetUserCount();
                if (userCount >= tenant.MaxUserCount)
                {
                    return ResponseBase.CommonError($"员工数量已达到最多{tenant.MaxUserCount}个的限制");
                }
            }

            var user = new EtUser()
            {
                Address = request.Address,
                IsTeacher = request.IsTeacher,
                Name = request.Name,
                Phone = request.Phone,
                Remark = request.Remark,
                RoleId = request.RoleId,
                TenantId = request.LoginTenantId,
                JobType = request.JobType
            };
            await _etUserDAL.AddUser(user);

            CoreBusiness.ProcessUserPhoneAboutAdd(user, _eventPublisher);
            await _userOperationLogDAL.AddUserLog(request, $"添加员工-名称:{user.Name},昵称:{user.NickName},手机号码:{user.Phone}", EmUserOperationType.UserSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserEdit(UserEditRequest request)
        {
            var user = await _etUserDAL.GetUser(request.CId);
            if (user == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            if (await _etUserDAL.ExistUserPhone(request.Phone, request.CId))
            {
                return ResponseBase.CommonError("手机号码已存在");
            }
            var oldPhone = user.Phone;

            user.Name = request.Name;
            user.Phone = request.Phone;
            user.RoleId = request.RoleId;
            user.Remark = request.Remark;
            user.Address = request.Address;
            user.IsTeacher = request.IsTeacher;
            user.JobType = request.JobType;
            await _etUserDAL.EditUser(user);

            CoreBusiness.ProcessUserPhoneAboutEdit(oldPhone, user, _eventPublisher);
            await _userOperationLogDAL.AddUserLog(request, $"编辑员工-名称:{user.Name},昵称:{user.NickName},手机号码:{user.Phone}", EmUserOperationType.UserSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserDel(UserDelRequest request)
        {
            var user = await _etUserDAL.GetUser(request.CId);
            if (user == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            if (user.IsAdmin)
            {
                return ResponseBase.CommonError("此用户无法删除");
            }
            if (await _userOperationLogDAL.IsUserCanNotBeDelete(request.CId))
            {
                return ResponseBase.CommonError("用户已使用，不允许删除");
            }
            if (await _userOperationLogDAL.IsUserCanNotBeDelete2(request.CId))
            {
                return ResponseBase.CommonError("此用户存在上课信息，不允许删除");
            }
            await _etUserDAL.DelUser(request.CId);
            AliyunOssUtil.DeleteObject(user.Avatar);

            CoreBusiness.ProcessUserPhoneAboutDel(user, _eventPublisher);
            await _userOperationLogDAL.AddUserLog(request, $"删除员工-名称:{user.Name},昵称:{user.NickName},手机号码:{user.Phone}", EmUserOperationType.UserSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserGetPaging(UserGetPagingRequest request)
        {
            request.OnlyShowTeacher = false;
            var userView = await _etUserDAL.GetUserPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<UserGetPagingOutput>(userView.Item2, userView.Item1.Select(p => new UserGetPagingOutput()
            {
                Address = p.Address,
                Cid = p.Id,
                IsTeacher = p.IsTeacher,
                LastLoginTimeDesc = p.LastLoginTime.EtmsToString(),
                Name = p.Name,
                Phone = p.Phone,
                Remark = p.Remark,
                RoleName = p.RoleName,
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p.Avatar),
                JobTypeDesc = EmUserJobType.GetUserJobTypeDesc(p.JobType),
                IsTeacherDesc = p.IsTeacher ? "是" : "否",
                JobType = p.JobType,
                Value = p.Id,
                Label = p.Name,
                IsBindingWechat = string.IsNullOrEmpty(p.WechatOpenid) ? EmIsBindingWechat.No : EmIsBindingWechat.Yes
            })));
        }

        public async Task<ResponseBase> TeacherGetPaging(UserGetPagingRequest request)
        {
            request.OnlyShowTeacher = true;
            var userView = await _etUserDAL.GetUserPaging(request);
            var allSubject = await _subjectDAL.GetAllSubject();
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherGetPagingOutput>(userView.Item2, userView.Item1.Select(p => new TeacherGetPagingOutput()
            {
                Address = p.Address,
                Cid = p.Id,
                Name = p.Name,
                Phone = p.Phone,
                JobAddTimeDesc = p.JobAddTime.EtmsToDateString(),
                JobTypeDesc = EmUserJobType.GetUserJobTypeDesc(p.JobType),
                NickName = p.NickName,
                TeacherCertification = p.TeacherCertification,
                SubjectsGoodAtDesc = p.SubjectsGoodAt.ToItemDesc(allSubject),
                TotalClassCount = p.TotalClassCount,
                TotalClassTimes = p.TotalClassTimes.EtmsToString(),
                Gender = p.Gender,
                GenderDesc = EmGender.GetGenderDesc(p.Gender),
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p.Avatar),
                JobType = p.JobType,
                Label = p.Name,
                Value = p.Id,
                IsBindingWechat = string.IsNullOrEmpty(p.WechatOpenid) ? EmIsBindingWechat.No : EmIsBindingWechat.Yes
            }))); ;
        }

        public async Task<ResponseBase> UserOperationLogGetPaging(UserOperationLogGetPagingRequest request)
        {
            var opLog = await _userOperationLogDAL.GetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<UserOperationLogGetPagingOutput>(opLog.Item2, opLog.Item1.Select(p => new UserOperationLogGetPagingOutput()
            {
                IpAddress = p.IpAddress,
                OpContent = p.OpContent,
                Ot = p.Ot,
                Remark = p.Remark,
                UserName = p.UserName,
                UserPhone = p.UserPhone,
                TypeDesc = EnumDataLib.GetUserOperationTypeDesc.FirstOrDefault(j => j.Value == p.Type)?.Label,
                ClientTypeDesc = EmUserOperationLogClientType.GetClientTypeDesc(p.ClientType)
            })));
        }

        public ResponseBase UserOperationLogTypeGet(RequestBase request)
        {
            return ResponseBase.Success(EnumDataLib.GetUserOperationTypeDesc);
        }

        public async Task<ResponseBase> TeacherEdit(TeacherEditRequest request)
        {
            var user = await _etUserDAL.GetUser(request.CId);
            if (user == null)
            {
                return ResponseBase.CommonError("老师不存在");
            }
            var subjectsGoodAt = string.Empty;
            if (request.SubjectsGoodAt != null && request.SubjectsGoodAt.Any())
            {
                subjectsGoodAt = string.Join(',', request.SubjectsGoodAt);
            }
            user.Gender = request.Gender;
            user.NickName = request.NickName;
            user.SubjectsGoodAt = subjectsGoodAt;
            user.TeacherCertification = request.TeacherCertification;
            await _etUserDAL.EditUser(user);
            await _userOperationLogDAL.AddUserLog(request, $"编辑老师-名称:{user.Name},昵称:{request.NickName},手机号码:{user.Phone}", EmUserOperationType.UserSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherRemove(TeacherRemoveRequest request)
        {
            var user = await _etUserDAL.GetUser(request.CId);
            if (user == null)
            {
                return ResponseBase.CommonError("老师不存在");
            }
            user.IsTeacher = false;
            await _etUserDAL.EditUser(user);
            await _userOperationLogDAL.AddUserLog(request, $"移除老师-名称:{user.Name},昵称:{user.NickName},手机号码:{user.Phone}", EmUserOperationType.UserSetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserGet(UserGetRequest request)
        {
            var user = await _etUserDAL.GetUser(request.CId);
            if (user == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            var subjectsGoodAtItems = new string[0];
            if (!string.IsNullOrEmpty(user.SubjectsGoodAt))
            {
                subjectsGoodAtItems = user.SubjectsGoodAt.Split(',');
            }
            return ResponseBase.Success(new UserGetOutput()
            {
                Address = user.Address,
                AvatarKey = user.Avatar,
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, user.Avatar),
                CId = user.Id,
                Gender = user.Gender,
                IsTeacher = user.IsTeacher,
                LastLoginTimeDesc = user.LastLoginTime.EtmsToString(),
                JobAddTimeDesc = user.JobAddTime.EtmsToDateString(),
                JobType = user.JobType,
                Name = user.Name,
                NickName = user.NickName,
                Phone = user.Phone,
                Remark = user.Remark,
                RoleId = user.RoleId,
                SubjectsGoodAt = user.SubjectsGoodAt,
                TeacherCertification = user.TeacherCertification,
                TotalClassCount = user.TotalClassCount,
                TotalClassTimes = user.TotalClassTimes.EtmsToString(),
                SubjectsGoodAtItems = subjectsGoodAtItems
            });
        }

        public async Task<ResponseBase> TeacherClassTimesGetPaging(TeacherClassTimesGetPagingRequest request)
        {
            var pagingData = await _etUserDAL.GetTeacherClassTimesPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherClassTimesGetPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p => new TeacherClassTimesGetPagingOutput()
            {
                UserName = p.UserName,
                UserPhone = p.UserPhone,
                ClassCount = p.ClassCount,
                ClassTimes = p.ClassTimes.EtmsToString(),
                DateDesc = p.FirstTime.ToString("yyyy年MM月")
            })));
        }
    }
}
