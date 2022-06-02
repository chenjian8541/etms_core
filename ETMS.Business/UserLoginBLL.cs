using ETMS.Authority;
using ETMS.Business.Common;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Config.Router;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.User;
using ETMS.Entity.Dto.User.Output;
using ETMS.Entity.Dto.User.Request;
using ETMS.Entity.Enum;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.IBusiness.Wechart;
using ETMS.LOG;
using ETMS.Business.WxCore;
using System.Linq;

namespace ETMS.Business
{
    public class UserLoginBLL : WeChatAccessBLL, IUserLoginBLL
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IUserDAL _etUserDAL;

        private readonly IUserOperationLogDAL _etUserOperationLogDAL;

        private readonly IUserLoginFailedRecordDAL _userLoginFailedRecordDAL;

        private readonly ISmsService _smsService;

        private readonly IUserLoginSmsCodeDAL _userLoginSmsCodeDAL;

        private readonly IRoleDAL _roleDAL;

        private readonly IAppAuthorityDAL _appAuthorityDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        private readonly IUserWechatDAL _userWechatDAL;

        private readonly ISysTenantUserDAL _sysTenantUserDAL;

        private readonly ISysVersionDAL _sysVersionDAL;
        public UserLoginBLL(ISysTenantDAL sysTenantDAL, IUserDAL etUserDAL, IUserOperationLogDAL etUserOperationLogDAL,
            IUserLoginFailedRecordDAL userLoginFailedRecordDAL, IAppConfigurtaionServices appConfigurtaionServices,
            ISmsService smsService, IUserLoginSmsCodeDAL userLoginSmsCodeDAL, IRoleDAL roleDAL,
            IAppAuthorityDAL appAuthorityDAL, ITempDataCacheDAL tempDataCacheDAL, IComponentAccessBLL componentAccessBLL,
            IUserWechatDAL userWechatDAL, ISysTenantUserDAL sysTenantUserDAL, ISysVersionDAL sysVersionDAL)
            : base(componentAccessBLL, appConfigurtaionServices)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._etUserDAL = etUserDAL;
            this._etUserOperationLogDAL = etUserOperationLogDAL;
            this._userLoginFailedRecordDAL = userLoginFailedRecordDAL;
            this._smsService = smsService;
            this._userLoginSmsCodeDAL = userLoginSmsCodeDAL;
            this._roleDAL = roleDAL;
            this._appAuthorityDAL = appAuthorityDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
            this._userWechatDAL = userWechatDAL;
            this._sysTenantUserDAL = sysTenantUserDAL;
            this._sysVersionDAL = sysVersionDAL;
        }

        public async Task<ResponseBase> UserGetAuthorizeUrl(UserGetAuthorizeUrlRequest request)
        {
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            return await GetAuthorizeUrl(tenantId, request.SourceUrl, request.State);
        }

        public async Task<ResponseBase> UserGetAuthorizeUrl2(UserGetAuthorizeUrl2Request request)
        {
            return await GetAuthorizeUrl(request.LoginTenantId, request.SourceUrl, request.State);
        }

        public async Task<ResponseBase> UserBindingWeChat(UserBindingWeChatRequest request)
        {
            _etUserDAL.InitTenantId(request.LoginTenantId);
            var user = await _etUserDAL.GetUser(request.LoginUserId);
            await SaveUserWechat(user.Phone, request.LoginUserId, request.LoginTenantId, request.Code);
            return ResponseBase.Success();
        }

        /// <summary>
        ///账号密码登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase> UserLogin(UserLoginRequest request)
        {
            var response = new ResponseBase().GetResponseBadRequest("账号信息错误");
            if (!CheckUserLoginFailedRecord(request.Code, request.Phone))
            {
                return response.GetResponseError("登录失败次数超过限制", StatusCode.Login20003);
            }
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.Code);
            if (sysTenantInfo == null)
            {
                return response;
            }
            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return response.GetResponseError(myMsg);
            }
            var sysVersion = await _sysVersionDAL.GetVersion(sysTenantInfo.VersionId);
            if (sysVersion == null)
            {
                return ResponseBase.CommonError("系统版本信息错误");
            }
            if (!ComBusiness2.CheckSysVersionCanLogin(sysVersion, request.ClientType))
            {
                return ResponseBase.CommonError("机构未开通此模块");
            }

            _etUserDAL.InitTenantId(sysTenantInfo.Id);
            _etUserOperationLogDAL.InitTenantId(sysTenantInfo.Id);
            var userInfo = await _etUserDAL.GetUser(request.Phone);
            if (userInfo == null)
            {
                return response;
            }
            if (string.IsNullOrEmpty(userInfo.Password))
            {
                return response.GetResponseError("未开通密码登录，请先设置密码");
            }
            var pwd = CryptogramHelper.Encrypt3DES(request.Pwd, SystemConfig.CryptogramConfig.Key);
            if (!userInfo.Password.Equals(pwd))
            {
                _userLoginFailedRecordDAL.AddUserLoginFailedRecord(request.Code, request.Phone);
                return response;
            }
            if (!ComBusiness2.CheckUserCanLogin(userInfo, out var msg))
            {
                return response.GetResponseError(msg);
            }
            _roleDAL.InitTenantId(userInfo.TenantId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            var roleSetting = ComBusiness3.AnalyzeNoticeSetting(role.NoticeSetting, userInfo.IsAdmin);
            if (!userInfo.IsAdmin)
            {
                if (!ComBusiness2.CheckRoleCanLogin(roleSetting, request.ClientType, out var msgRoleLimit))
                {
                    return response.GetResponseError(msgRoleLimit);
                }
            }
            var userLoginOutput = await LoginSuccessProcess(userInfo, request.IpAddress, request.Code, request.Phone, request.ClientType,
                role, roleSetting);
            return response.GetResponseSuccess(userLoginOutput);
        }

        /// <summary>
        /// 账户是否允许登录
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private bool CheckUserLoginFailedRecord(string code, string phone)
        {
            var record = _userLoginFailedRecordDAL.GetUserLoginFailedRecord(code, phone);
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

        public async Task<ResponseBase> UserLoginSendSms(UserLoginSendSmsRequest request)
        {
            var response = new ResponseBase().GetResponseBadRequest("账号信息错误");
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.Code);
            if (sysTenantInfo == null)
            {
                return response;
            }
            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return response.GetResponseError(myMsg);
            }
            var sysVersion = await _sysVersionDAL.GetVersion(sysTenantInfo.VersionId);
            if (sysVersion == null)
            {
                return ResponseBase.CommonError("系统版本信息错误");
            }
            if (!ComBusiness2.CheckSysVersionCanLogin(sysVersion, EmUserOperationLogClientType.PC))
            {
                return ResponseBase.CommonError("机构未开通此模块");
            }

            _etUserDAL.InitTenantId(sysTenantInfo.Id);
            _etUserOperationLogDAL.InitTenantId(sysTenantInfo.Id);
            var userInfo = await _etUserDAL.GetUser(request.Phone);
            if (userInfo == null)
            {
                return response;
            }
            var smsCode = RandomHelper.GetSmsCode();
            var sendSmsRes = await _smsService.UserLogin(new SmsUserLoginRequest(sysTenantInfo.Id)
            {
                Phone = request.Phone,
                ValidCode = smsCode
            });
            if (!sendSmsRes.IsSuccess)
            {
                return response.GetResponseError("发送短信失败,请稍后再试");
            }
            this._userLoginSmsCodeDAL.AddUserLoginSmsCode(request.Code, request.Phone, smsCode);
            return response.GetResponseSuccess();
        }

        public async Task<ResponseBase> UserLoginSendSmsCodeSafe(UserLoginSendSmsCodeSafeRequest request)
        {
            var myVerificationCodeBucket = _tempDataCacheDAL.GetPhoneVerificationCodeBucket(request.Phone);
            if (myVerificationCodeBucket == null || myVerificationCodeBucket.VerificationCode != request.VerificationCode)
            {
                return ResponseBase.CommonError("校验码错误");
            }
            var response = new ResponseBase().GetResponseBadRequest("账号信息错误");
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.Code);
            if (sysTenantInfo == null)
            {
                return response;
            }
            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return response.GetResponseError(myMsg);
            }
            _etUserDAL.InitTenantId(sysTenantInfo.Id);
            _etUserOperationLogDAL.InitTenantId(sysTenantInfo.Id);
            var userInfo = await _etUserDAL.GetUser(request.Phone);
            if (userInfo == null)
            {
                return response;
            }
            var smsCode = RandomHelper.GetSmsCode();
            var sendSmsRes = await _smsService.UserLogin(new SmsUserLoginRequest(sysTenantInfo.Id)
            {
                Phone = request.Phone,
                ValidCode = smsCode
            });
            if (!sendSmsRes.IsSuccess)
            {
                return response.GetResponseError("发送短信失败,请稍后再试");
            }
            this._userLoginSmsCodeDAL.AddUserLoginSmsCode(request.Code, request.Phone, smsCode);
            _tempDataCacheDAL.RemovePhoneVerificationCodeBucket(request.Phone);
            return response.GetResponseSuccess();
        }

        /// <summary>
        /// 使用短信验证码登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase> UserLoginBySms(UserLoginBySmsRequest request)
        {
            var response = new ResponseBase().GetResponseBadRequest("账号信息错误");
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.Code);
            if (sysTenantInfo == null)
            {
                return response;
            }
            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return response.GetResponseError(myMsg);
            }
            var sysVersion = await _sysVersionDAL.GetVersion(sysTenantInfo.VersionId);
            if (sysVersion == null)
            {
                return ResponseBase.CommonError("系统版本信息错误");
            }
            if (!ComBusiness2.CheckSysVersionCanLogin(sysVersion, request.ClientType))
            {
                return ResponseBase.CommonError("机构未开通此模块");
            }

            _etUserDAL.InitTenantId(sysTenantInfo.Id);
            _etUserOperationLogDAL.InitTenantId(sysTenantInfo.Id);
            var userInfo = await _etUserDAL.GetUser(request.Phone);
            if (userInfo == null)
            {
                return response;
            }
            var loginSms = _userLoginSmsCodeDAL.GetUserLoginSmsCode(request.Code, request.Phone);
            if (loginSms == null || loginSms.ExpireAtTime < DateTime.Now || loginSms.SmsCode != request.SmsCode)
            {
                return response.GetResponseError("验证码错误");
            }
            if (!ComBusiness2.CheckUserCanLogin(userInfo, out var msg))
            {
                return response.GetResponseError(msg);
            }
            _roleDAL.InitTenantId(userInfo.TenantId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            var roleSetting = ComBusiness3.AnalyzeNoticeSetting(role.NoticeSetting, userInfo.IsAdmin);
            if (!userInfo.IsAdmin)
            {
                if (!ComBusiness2.CheckRoleCanLogin(roleSetting, request.ClientType, out var msgRoleLimit))
                {
                    return response.GetResponseError(msgRoleLimit);
                }
            }
            var userLoginOutput = await LoginSuccessProcess(userInfo, request.IpAddress, request.Code, request.Phone, request.ClientType, role,
                roleSetting);
            return response.GetResponseSuccess(userLoginOutput);
        }

        public async Task<ResponseBase> UserLoginBySmsH5(UserLoginBySmsH5Request request)
        {
            var res = await UserLoginBySms(new UserLoginBySmsRequest()
            {
                Code = request.Code,
                IpAddress = request.IpAddress,
                Phone = request.Phone,
                SmsCode = request.SmsCode,
                ClientType = EmUserOperationLogClientType.WeChat
            });
            if (res.IsResponseSuccess())
            {
                var result = (UserLoginOutput)res.resultData;
                if (!string.IsNullOrEmpty(request.WechatCode))
                {
                    await SaveUserWechat(request.Phone, result.UId, result.TId, request.WechatCode);
                }
                return ResponseBase.Success(new UserLoginBySmsH5Output()
                {
                    ExpiresTime = result.ExpiresTime,
                    Token = result.Token,
                    IsBindWeChatOfficialAccount = await CheckIsBindWeChatOfficialAccount(result.TId)
                });
            }
            return res;
        }

        public async Task<ResponseBase> UserLoginByH5(UserLoginByH5Request request)
        {
            var res = await UserLogin(new UserLoginRequest()
            {
                Code = request.Code,
                IpAddress = request.IpAddress,
                Phone = request.Phone,
                Pwd = request.Pwd,
                ClientType = EmUserOperationLogClientType.WeChat
            });
            if (res.IsResponseSuccess())
            {
                var result = (UserLoginOutput)res.resultData;
                if (!string.IsNullOrEmpty(request.WechatCode))
                {
                    await SaveUserWechat(request.Phone, result.UId, result.TId, request.WechatCode);
                }
                return ResponseBase.Success(new UserLoginBySmsH5Output()
                {
                    ExpiresTime = result.ExpiresTime,
                    Token = result.Token,
                    IsBindWeChatOfficialAccount = await CheckIsBindWeChatOfficialAccount(result.TId)
                });
            }
            return res;
        }

        private async Task SaveUserWechat(string phone, long userId, int tenantId, string wechatCode)
        {
            try
            {
                var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(tenantId);
                if (tenantWechartAuth == null)
                {
                    Log.Error($"[SaveUserWechat]未找到机构授权信息,tenantId:{tenantId}", this.GetType());
                    return;
                }
                var authToken = this.GetAuthAccessToken(tenantWechartAuth.AuthorizerAppid, wechatCode);
                var userInfo = this.GetUserInfo(authToken.access_token, authToken.openid);
                _userWechatDAL.InitTenantId(tenantId);
                await _userWechatDAL.SaveUserWechat(new EtUserWechat()
                {
                    Headimgurl = userInfo.headimgurl,
                    IsDeleted = EmIsDeleted.Normal,
                    Nickname = userInfo.nickname,
                    Phone = phone,
                    Remark = DateTime.Now.EtmsToString(),
                    TenantId = tenantId,
                    UserId = userId,
                    WechatOpenid = userInfo.openid,
                    WechatUnionid = userInfo.unionid
                });
                _etUserDAL.InitTenantId(tenantId);
                await _etUserDAL.UserEditWx(userId, userInfo.openid, userInfo.unionid);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[SaveUserWechat]保存用户微信信息错误,tenantId:{tenantId},phone:{phone},userId:{userId},wechatCode:{wechatCode}", ex, this.GetType());
            }
        }

        private async Task<UserLoginOutput> LoginSuccessProcess(EtUser userInfo, string ipAddress,
            string code, string phone, int clientType, EtRole role, RoleNoticeSettingOutput roleSetting)
        {
            var time = DateTime.Now;
            var nowTimestamp = time.EtmsGetTimestamp().ToString();
            var token = JwtHelper.GenerateToken(userInfo.TenantId, userInfo.Id, nowTimestamp, out var exTime);
            await _etUserDAL.UpdateUserLastLoginTime(userInfo.Id, time);
            _userLoginFailedRecordDAL.RemoveUserLoginFailedRecord(code, phone);
            _userLoginSmsCodeDAL.RemoveUserLoginSmsCode(code, phone);
            await _etUserOperationLogDAL.AddUserLog(new Entity.Database.Source.EtUserOperationLog()
            {
                IpAddress = ipAddress,
                Ot = time,
                TenantId = userInfo.TenantId,
                UserId = userInfo.Id,
                OpContent = $"用户:{userInfo.Name},手机号:{userInfo.Phone}在{time.EtmsToString()}登录",
                Type = (int)EmUserOperationType.Login,
                ClientType = clientType
            });
            var myAllMenus = await _appAuthorityDAL.GetTenantMenuConfig(userInfo.TenantId);
            _tempDataCacheDAL.SetUserLoginOnlineBucket(userInfo.TenantId, userInfo.Id, nowTimestamp, clientType);
            return new UserLoginOutput()
            {
                Token = token,
                ExpiresTime = exTime,
                Permission = ComBusiness.GetPermissionOutput(myAllMenus, role.AuthorityValueMenu, userInfo.IsAdmin),
                UId = userInfo.Id,
                TId = userInfo.TenantId,
                RoleSetting = roleSetting
            };
        }

        public async Task<ResponseBase> CheckUserCanLogin(RequestBase request)
        {
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            var sysVersion = await _sysVersionDAL.GetVersion(sysTenantInfo.VersionId);
            if (sysVersion == null)
            {
                return ResponseBase.CommonError("系统版本信息错误");
            }
            if (!ComBusiness2.CheckSysVersionCanLogin(sysVersion, request.LoginClientType))
            {
                return ResponseBase.CommonError("机构未开通此模块");
            }

            _etUserDAL.InitTenantId(request.LoginTenantId);
            var userInfo = await _etUserDAL.GetUser(request.LoginUserId);
            if (!ComBusiness2.CheckUserCanLogin(userInfo, out var msg))
            {
                return ResponseBase.CommonError(msg);
            }

            var userLoginOnlineBucket = _tempDataCacheDAL.GetUserLoginOnlineBucket(request.LoginTenantId, request.LoginUserId, request.LoginClientType);
            if (userLoginOnlineBucket != null && userLoginOnlineBucket.LoginTime != request.LoginTimestamp)
            {
                return ResponseBase.CommonError("您的账号已在其他设备登录，请重新登录！");
            }

            _roleDAL.InitTenantId(userInfo.TenantId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            var roleSetting = ComBusiness3.AnalyzeNoticeSetting(role.NoticeSetting, userInfo.IsAdmin);
            if (!userInfo.IsAdmin)
            {
                if (!ComBusiness2.CheckRoleCanLogin(roleSetting, request.LoginClientType, out var msgRoleLimit))
                {
                    return ResponseBase.CommonError(msgRoleLimit);
                }
            }

            return ResponseBase.Success(new CheckUserCanLoginOutput()
            {
                IsDataLimit = EmDataLimitType.GetIsDataLimit(role.AuthorityValueData),
                SecrecyType = role.SecrecyType,
                AgtPayType = sysTenantInfo.AgtPayType
            });
        }

        //public async Task<bool> GetUserDataLimit(RequestBase request)
        //{
        //    _etUserDAL.InitTenantId(request.LoginTenantId);
        //    var user = await _etUserDAL.GetUser(request.LoginUserId);
        //    _roleDAL.InitTenantId(request.LoginTenantId);
        //    var role = await _roleDAL.GetRole(user.RoleId);
        //    return EmDataLimitType.GetIsDataLimit(role.AuthorityValueData);
        //}

        public ResponseBase UserCheck(UserCheckRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserGetCurrentTenant(RequestBase request)
        {
            _etUserDAL.InitTenantId(request.LoginTenantId);
            var user = await _etUserDAL.GetUser(request.LoginUserId);
            var myTenants = await _sysTenantUserDAL.GetTenantUser(user.Phone);
            var thisTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            return ResponseBase.Success(new UserGetCurrentTenantOutput()
            {
                CurrentTenantCode = thisTenant.TenantCode,
                CurrentTenantName = thisTenant.Name,
                CurrentTenantId = thisTenant.Id,
                IsHasMultipleTenant = myTenants.Count > 1
            });
        }

        public async Task<ResponseBase> UserGetTenants(RequestBase request)
        {
            _etUserDAL.InitTenantId(request.LoginTenantId);
            var user = await _etUserDAL.GetUser(request.LoginUserId);
            var myTenants = await _sysTenantUserDAL.GetTenantUser(user.Phone);
            var allVersions = await _sysVersionDAL.GetVersions();
            var output = new List<UserGetTenantsOutput>();
            foreach (var p in myTenants)
            {
                var thisTenant = await _sysTenantDAL.GetTenant(p.TenantId);
                if (thisTenant == null)
                {
                    Log.Error($"[UserGetTenants]机构不存在，TenantId:{p.TenantId}", this.GetType());
                    continue;
                }
                if (!ComBusiness2.CheckTenantCanLogin(thisTenant, out var myMsg))
                {
                    continue;
                }
                var myVersion = allVersions.FirstOrDefault(j => j.Id == thisTenant.VersionId);
                if (myVersion == null)
                {
                    continue;
                }
                if (!ComBusiness2.CheckSysVersionCanLogin(myVersion, request.LoginClientType))
                {
                    continue;
                }

                output.Add(new UserGetTenantsOutput()
                {
                    TenantCode = thisTenant.TenantCode,
                    TenantName = thisTenant.Name,
                    TenantId = thisTenant.Id,
                    IsCurrentLogin = p.TenantId == request.LoginTenantId
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> UserTenantEntrance(UserTenantEntranceRequest request)
        {
            if (request.TenantId == request.LoginTenantId)
            {
                return ResponseBase.CommonError("已登录此机构");
            }
            var thisTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (thisTenant == null)
            {
                Log.Error($"[UserTenantEntrance]机构不存在，TenantId:{request.TenantId}", this.GetType());
                return ResponseBase.CommonError("机构不存在");
            }
            if (!ComBusiness2.CheckTenantCanLogin(thisTenant, out var myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            var sysVersion = await _sysVersionDAL.GetVersion(thisTenant.VersionId);
            if (sysVersion == null)
            {
                return ResponseBase.CommonError("系统版本信息错误");
            }
            if (!ComBusiness2.CheckSysVersionCanLogin(sysVersion, request.LoginClientType))
            {
                return ResponseBase.CommonError("机构未开通此模块");
            }

            _etUserDAL.InitTenantId(request.LoginTenantId);
            var userInfo = await _etUserDAL.GetUser(request.LoginUserId);

            _etUserDAL.ResetTenantId(thisTenant.Id);
            var thisUser = await _etUserDAL.GetUser(userInfo.Phone);
            if (thisUser == null)
            {
                return ResponseBase.CommonError("用户不存在");
            }
            if (!ComBusiness2.CheckUserCanLogin(thisUser, out var msg))
            {
                return ResponseBase.CommonError(msg);
            }

            await _sysTenantUserDAL.UpdateTenantUserOpTime(thisTenant.Id, thisUser.Phone, DateTime.Now);
            _etUserOperationLogDAL.InitTenantId(thisTenant.Id);

            _roleDAL.InitTenantId(userInfo.TenantId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            var roleSetting = ComBusiness3.AnalyzeNoticeSetting(role.NoticeSetting, userInfo.IsAdmin);
            if (!userInfo.IsAdmin)
            {
                if (!ComBusiness2.CheckRoleCanLogin(roleSetting, request.LoginClientType, out var msgRoleLimit))
                {
                    return ResponseBase.CommonError(msgRoleLimit);
                }
            }
            var result = await LoginSuccessProcess(thisUser, request.IpAddress, thisTenant.TenantCode, thisUser.Phone, request.LoginClientType, role,
                roleSetting);
            return ResponseBase.Success(new UserLoginBySmsH5Output()
            {
                ExpiresTime = result.ExpiresTime,
                Token = result.Token,
                IsBindWeChatOfficialAccount = await CheckIsBindWeChatOfficialAccount(thisTenant.Id)
            });
        }

        public async Task<ResponseBase> UserTenantEntrancePC(UserTenantEntrancePCRequest request)
        {
            var thisTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (thisTenant == null)
            {
                Log.Error($"[UserTenantEntrance]机构不存在，TenantId:{request.TenantId}", this.GetType());
                return ResponseBase.CommonError("机构不存在");
            }
            if (!ComBusiness2.CheckTenantCanLogin(thisTenant, out var myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            var sysVersion = await _sysVersionDAL.GetVersion(thisTenant.VersionId);
            if (sysVersion == null)
            {
                return ResponseBase.CommonError("系统版本信息错误");
            }
            if (!ComBusiness2.CheckSysVersionCanLogin(sysVersion, request.LoginClientType))
            {
                return ResponseBase.CommonError("机构未开通此模块");
            }

            _etUserDAL.InitTenantId(request.TenantId);

            var userInfo = await _etUserDAL.GetUser(request.Phone);
            if (userInfo == null)
            {
                return ResponseBase.CommonError("用户不存在");
            }
            if (!ComBusiness2.CheckUserCanLogin(userInfo, out var msg))
            {
                return ResponseBase.CommonError(msg);
            }

            await _sysTenantUserDAL.UpdateTenantUserOpTime(thisTenant.Id, userInfo.Phone, DateTime.Now);
            _etUserOperationLogDAL.InitTenantId(thisTenant.Id);

            _roleDAL.InitTenantId(request.TenantId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            var roleSetting = ComBusiness3.AnalyzeNoticeSetting(role.NoticeSetting, userInfo.IsAdmin);
            if (!userInfo.IsAdmin)
            {
                if (!ComBusiness2.CheckRoleCanLogin(roleSetting, request.LoginClientType, out var msgRoleLimit))
                {
                    return ResponseBase.CommonError(msgRoleLimit);
                }
            }
            var result = await LoginSuccessProcess(userInfo, request.IpAddress, thisTenant.TenantCode, userInfo.Phone, request.LoginClientType, role,
                roleSetting);
            return ResponseBase.Success(result);
        }

        public ResponseBase UserTenantEntrancePCGate(UserTenantEntrancePCGateRequest request)
        {
            var loginNoDecrypt = TenantLib.GetTenantEntranceDecrypt(request.LoginNo);
            var loginInfo = _tempDataCacheDAL.GetUserTenantEntrancePCBucket(loginNoDecrypt.TenantId, loginNoDecrypt.UserId);
            if (loginInfo == null || loginInfo.MyUserLoginOutput == null)
            {
                return ResponseBase.CommonError("登录信息已过期");
            }
            if (loginInfo.LoginTimestamp != loginNoDecrypt.NowTimestamp)
            {
                return ResponseBase.CommonError("无效的登录");
            }
            _tempDataCacheDAL.RemoveUserTenantEntrancePCBucket(loginNoDecrypt.TenantId, loginNoDecrypt.UserId);
            return ResponseBase.Success(loginInfo.MyUserLoginOutput);
        }
    }
}
