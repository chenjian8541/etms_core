using ETMS.Business.Common;
using ETMS.Entity.Alien.Dto.User.Output;
using ETMS.Entity.Alien.Dto.User.Request;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.Alien;
using ETMS.IBusiness.Alien;
using ETMS.IDataAccess.Alien;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.DataAccess.Alien.Lib;
using ETMS.Entity.Alien.Common;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Entity.Dto.Common.Output;

namespace ETMS.Business.Alien
{
    public class AlienUserLoginBLL : IAlienUserLoginBLL
    {
        private readonly IMgHeadDAL _mgHeadDAL;

        private readonly IMgUserDAL _mgUserDAL;

        private readonly IMgUserOpLogDAL _mgUserOpLogDAL;

        private readonly IMgUserLoginFailedRecordDAL _mgUserLoginFailedRecordDAL;

        private readonly IMgRoleDAL _mgRoleDAL;

        private readonly IMgTempDataCacheDAL _mgTempDataCacheDAL;

        private readonly IMgTenantsDAL _mgTenantsDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysTenantUserDAL _sysTenantUserDAL;

        public AlienUserLoginBLL(IMgHeadDAL mgHeadDAL, IMgUserDAL mgUserDAL, IMgUserOpLogDAL mgUserOpLogDAL,
            IMgUserLoginFailedRecordDAL mgUserLoginFailedRecordDAL, IMgRoleDAL mgRoleDAL,
            IMgTempDataCacheDAL mgTempDataCacheDAL, IMgTenantsDAL mgTenantsDAL, ISysTenantDAL sysTenantDAL,
            ISysTenantUserDAL sysTenantUserDAL)
        {
            this._mgHeadDAL = mgHeadDAL;
            this._mgUserDAL = mgUserDAL;
            this._mgUserOpLogDAL = mgUserOpLogDAL;
            this._mgUserLoginFailedRecordDAL = mgUserLoginFailedRecordDAL;
            this._mgRoleDAL = mgRoleDAL;
            this._mgTempDataCacheDAL = mgTempDataCacheDAL;
            this._mgTenantsDAL = mgTenantsDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysTenantUserDAL = sysTenantUserDAL;
        }

        public void InitHeadId(int headId)
        {
            this.InitDataAccess(headId, _mgUserDAL, _mgUserOpLogDAL, _mgRoleDAL, _mgTenantsDAL);
        }

        public async Task<ResponseBase> UserLogin(AlUserLoginRequest request)
        {
            var response = new ResponseBase().GetResponseBadRequest("账号信息错误");
            if (!CheckUserLoginFailedRecord(request.Code, request.Phone))
            {
                return response.GetResponseError("登录失败次数超过限制", StatusCode.Login20003);
            }
            var myMgHead = await _mgHeadDAL.GetMgHead(request.Code);
            if (myMgHead == null)
            {
                return response;
            }
            if (!this.CheckHeadCanLogin(myMgHead, out var myMsg))
            {
                return response.GetResponseError(myMsg);
            }
            this.InitHeadId(myMgHead.Id);
            var userInfo = await _mgUserDAL.GetUser(request.Phone);
            if (userInfo == null)
            {
                return response;
            }
            if (userInfo.MgRoleId == null)
            {
                return response.GetResponseError("无法登录");
            }
            if (string.IsNullOrEmpty(userInfo.Password))
            {
                return response.GetResponseError("未开通密码登录，请先设置密码");
            }
            var pwd = CryptogramHelper.Encrypt3DES(request.Pwd, SystemConfig.CryptogramConfig.Key);
            if (!userInfo.Password.Equals(pwd))
            {
                _mgUserLoginFailedRecordDAL.AddUserLoginFailedRecord(request.Code, request.Phone);
                return response;
            }
            if (!CheckUserCanLogin(userInfo, out var msg))
            {
                return response.GetResponseError(msg);
            }
            var role = await _mgRoleDAL.GetRole(userInfo.MgRoleId.Value);
            return response.GetResponseSuccess(await LoginSuccessProcess(userInfo, request.IpAddress,
                request.Code, request.Phone, request.ClientType, role));
        }

        /// <summary>
        /// 账户是否允许登录
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private bool CheckUserLoginFailedRecord(string code, string phone)
        {
            var record = _mgUserLoginFailedRecordDAL.GetUserLoginFailedRecord(code, phone);
            if (record == null)
            {
                return true;
            }
            if (record.ExpireAtTime > DateTime.Now)
            {
                return false;
            }
            return true;
        }

        private bool CheckHeadCanLogin(MgHead myHead, out string msg)
        {
            msg = string.Empty;
            if (myHead == null)
            {
                msg = "企业不存在,无法登陆";
                return false;
            }
            if (myHead.Status == EmMgHeadStatus.IsLock)
            {
                msg = "企业已锁定,无法登陆";
                return false;
            }
            return true;
        }

        private bool CheckUserCanLogin(MgUser user, out string msg)
        {
            msg = string.Empty;
            if (user.IsAdmin == EmBool.True)
            {
                return true;
            }
            if (user == null)
            {
                msg = "账户不存在,请重新登陆";
                return false;
            }
            if (user.IsLock == EmBool.True)
            {
                msg = "账户已锁定,无法登陆";
                return false;
            }
            return true;
        }

        private async Task<AlUserLoginOutput> LoginSuccessProcess(MgUser userInfo, string ipAddress,
            string code, string phone, int clientType, MgRole role)
        {
            var time = DateTime.Now;
            var nowTimestamp = time.EtmsGetTimestamp().ToString();
            var token = JwtHelper.AlienGenerateToken(userInfo.HeadId, userInfo.Id, nowTimestamp, out var exTime);
            await _mgUserDAL.UpdateUserLastLoginTime(userInfo.Id, time);
            _mgUserLoginFailedRecordDAL.RemoveUserLoginFailedRecord(code, phone);
            await _mgUserOpLogDAL.AddUserOpLog(new MgUserOpLog()
            {
                IpAddress = ipAddress,
                Ot = time,
                HeadId = userInfo.HeadId,
                MgUserId = userInfo.Id,
                OpContent = $"用户:{userInfo.Name},手机号:{userInfo.Phone}在{time.EtmsToString()}登录",
                Type = EmMgUserOperationType.Login,
                ClientType = clientType,
                IsDeleted = EmIsDeleted.Normal
            });
            _mgTempDataCacheDAL.SetUserLoginOnlineBucket(userInfo.HeadId, userInfo.Id, nowTimestamp, clientType);
            var allMenus = EtmsHelper.DeepCopy(AlienPermissionData.MenuConfigs);
            var isAdmin = userInfo.IsAdmin == EmBool.True;
            return new AlUserLoginOutput()
            {
                Token = token,
                ExpiresTime = exTime,
                Permission = ComBusiness.GetPermissionOutput(allMenus, role.AuthorityValueMenu, isAdmin),
                UId = userInfo.Id,
                HId = userInfo.HeadId
            };
        }

        public async Task<ResponseBase> UserLoginGet(AlienRequestBase request)
        {
            var myHead = await _mgHeadDAL.GetMgHead(request.LoginHeadId);
            var myUser = await _mgUserDAL.GetUser(request.LoginUserId);
            var myRole = await _mgRoleDAL.GetRole(myUser.MgRoleId.Value);
            var allRoutes = EtmsHelper.DeepCopy(AlienPermissionData.RouteConfigs);
            var isAdmin = myUser.IsAdmin == EmBool.True;

            var allTenantsOutput = new List<LoginTenantInfo>();
            var allTenants = await _mgTenantsDAL.GetMgTenants();
            if (allTenants != null && allTenants.Any())
            {
                var allPhoneTenants = await _sysTenantUserDAL.GetTenantUser(myUser.Phone);
                foreach (var item in allTenants)
                {
                    var p = await _sysTenantDAL.GetTenant(item.TenantId);
                    if (p == null)
                    {
                        continue;
                    }
                    var isRegister = false;
                    if (allPhoneTenants != null && allPhoneTenants.Any())
                    {
                        isRegister = allPhoneTenants.Exists(a => a.TenantId == item.TenantId);
                    }
                    allTenantsOutput.Add(new LoginTenantInfo()
                    {
                        Label = p.Name,
                        Value = p.Id,
                        IsRegister = isRegister
                    });
                }
            }
            return ResponseBase.Success(new UserLoginGetOutput()
            {
                Name = myUser.Name,
                Phone = myUser.Phone,
                HeadName = myHead.Name,
                HeadCode = myHead.HeadCode,
                TenantCount = myHead.TenantCount,
                Gender = myUser.Gender,
                RouteConfigs = ComBusiness.GetRouteConfigs(allRoutes, myRole.AuthorityValueMenu, isAdmin),
                Tenants = allTenantsOutput
            });
        }

        public async Task<ResponseBase> UserLoginPermissionGet(AlienRequestBase request)
        {
            var myUser = await _mgUserDAL.GetUser(request.LoginUserId);
            var myRole = await _mgRoleDAL.GetRole(myUser.MgRoleId.Value);
            var allMenus = EtmsHelper.DeepCopy(AlienPermissionData.MenuConfigs);
            var isAdmin = myUser.IsAdmin == EmBool.True;
            return ResponseBase.Success(ComBusiness.GetPermissionOutput(allMenus, myRole.AuthorityValueMenu, isAdmin));
        }

        public async Task<ResponseBase> CheckUserCanLogin(AlienRequestBase request)
        {
            var myHead = await _mgHeadDAL.GetMgHead(request.LoginHeadId);
            string myMsg;
            if (!CheckHeadCanLogin(myHead, out myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            _mgUserDAL.InitHeadId(request.LoginHeadId);
            var myUser = await _mgUserDAL.GetUser(request.LoginUserId);
            if (!CheckUserCanLogin(myUser, out myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            var userLoginOnlineBucket = _mgTempDataCacheDAL.GetUserLoginOnlineBucket(request.LoginHeadId, request.LoginUserId, request.LoginClientType);
            if (userLoginOnlineBucket != null && userLoginOnlineBucket.LoginTime != request.LoginTimestamp)
            {
                return ResponseBase.CommonError("您的账号已在其他设备登陆，请重新登录！");
            }

            _mgTenantsDAL.InitHeadId(request.LoginHeadId);
            var allTenant = await _mgTenantsDAL.GetMgTenants();
            if (allTenant.Count == 0)
            {
                return ResponseBase.CommonError("未查询到所关联的校区信息");
            }
            var output = new CheckUserCanLoginOutput()
            {
                AllTenants = allTenant.Select(j => j.TenantId).ToList()
            };
            return ResponseBase.Success(output);
        }
    }
}
