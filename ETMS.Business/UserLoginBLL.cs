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
using System.Linq;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.IBusiness.Wechart;
using ETMS.LOG;
using Senparc.Weixin.Open.OAuthAPIs;
using Senparc.Weixin.Open.Containers;

namespace ETMS.Business
{
    public class UserLoginBLL : IUserLoginBLL
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IUserDAL _etUserDAL;

        private readonly IUserOperationLogDAL _etUserOperationLogDAL;

        private readonly IUserLoginFailedRecordDAL _userLoginFailedRecordDAL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly ISmsService _smsService;

        private readonly IUserLoginSmsCodeDAL _userLoginSmsCodeDAL;

        private readonly IRoleDAL _roleDAL;

        private readonly IAppAuthorityDAL _appAuthorityDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        private readonly IComponentAccessBLL _componentAccessBLL;

        private readonly IUserWechatDAL _userWechatDAL;

        public UserLoginBLL(ISysTenantDAL sysTenantDAL, IUserDAL etUserDAL, IUserOperationLogDAL etUserOperationLogDAL,
            IUserLoginFailedRecordDAL userLoginFailedRecordDAL, IAppConfigurtaionServices appConfigurtaionServices,
            ISmsService smsService, IUserLoginSmsCodeDAL userLoginSmsCodeDAL, IRoleDAL roleDAL,
            IAppAuthorityDAL appAuthorityDAL, ITempDataCacheDAL tempDataCacheDAL, IComponentAccessBLL componentAccessBLL,
            IUserWechatDAL userWechatDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._etUserDAL = etUserDAL;
            this._etUserOperationLogDAL = etUserOperationLogDAL;
            this._userLoginFailedRecordDAL = userLoginFailedRecordDAL;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._smsService = smsService;
            this._userLoginSmsCodeDAL = userLoginSmsCodeDAL;
            this._roleDAL = roleDAL;
            this._appAuthorityDAL = appAuthorityDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
            this._componentAccessBLL = componentAccessBLL;
            this._userWechatDAL = userWechatDAL;
        }

        public async Task<ResponseBase> UserGetAuthorizeUrl(UserGetAuthorizeUrlRequest request)
        {
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(tenantId);
            if (tenantWechartAuth == null)
            {
                Log.Error($"[UserGetAuthorizeUrl]未找到机构授权信息,tenantId:{tenantId}", this.GetType());
                return ResponseBase.CommonError("机构绑定的微信公众号无权限");
            }
            var componentAppid = _appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
            var url = OAuthApi.GetAuthorizeUrl(tenantWechartAuth.AuthorizerAppid, componentAppid, request.SourceUrl, tenantId.ToString(),
                new[] { Senparc.Weixin.Open.OAuthScope.snsapi_userinfo, Senparc.Weixin.Open.OAuthScope.snsapi_base });
            Log.Info($"[老师端获取授权地址]{url}", this.GetType());
            return ResponseBase.Success(url);
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
            if (!CheckUserCanLogin(userInfo, out var msg))
            {
                return response.GetResponseError(msg);
            }
            var userLoginOutput = await LoginSuccessProcess(userInfo, request.IpAddress, request.Code, request.Phone, EmUserOperationLogClientType.PC);
            return response.GetResponseSuccess(userLoginOutput);
        }

        private bool CheckUserCanLogin(EtUser user, out string msg)
        {
            msg = string.Empty;
            if (user == null)
            {
                msg = "用户不存在,请重新登陆";
                return false;
            }
            if (user.JobType == EmUserJobType.Resignation)
            {
                msg = "您已离职，无法登陆";
                return false;
            }
            return true;
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

        /// <summary>
        /// 使用短信验证码登陆
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
            if (!CheckUserCanLogin(userInfo, out var msg))
            {
                return response.GetResponseError(msg);
            }
            var userLoginOutput = await LoginSuccessProcess(userInfo, request.IpAddress, request.Code, request.Phone, request.ClientType);
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
                    Token = result.Token
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
                var componentAppid = _appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
                var componentAccessToken = ComponentContainer.GetComponentAccessToken(componentAppid);
                var authToken = OAuthApi.GetAccessToken(tenantWechartAuth.AuthorizerAppid, componentAppid, componentAccessToken, wechatCode);
                var userInfo = OAuthApi.GetUserInfo(authToken.access_token, authToken.openid);
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
                Log.Error($"[SaveUserWechat]保存用户微信信息错误,tenantId:{tenantId},phone:{phone},userId:{userId},wechatCode:{wechatCode}", ex, this.GetType());
            }
        }

        private async Task<UserLoginOutput> LoginSuccessProcess(EtUser userInfo, string ipAddress,
            string code, string phone, int clientType)
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
            _roleDAL.InitTenantId(userInfo.TenantId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            var myAllMenus = await _appAuthorityDAL.GetTenantMenuConfig(userInfo.TenantId);
            _tempDataCacheDAL.SetUserLoginOnlineBucket(userInfo.TenantId, userInfo.Id, nowTimestamp, clientType);
            return new UserLoginOutput()
            {
                Token = token,
                ExpiresTime = exTime,
                Permission = ComBusiness.GetPermissionOutput(myAllMenus, role.AuthorityValueMenu, userInfo.IsAdmin),
                UId = userInfo.Id,
                TId = userInfo.TenantId
            };
        }

        public async Task<ResponseBase> CheckUserCanLogin(RequestBase request)
        {
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            _etUserDAL.InitTenantId(request.LoginTenantId);
            var user = await _etUserDAL.GetUser(request.LoginUserId);
            if (!CheckUserCanLogin(user, out var msg))
            {
                return ResponseBase.CommonError(msg);
            }

            var userLoginOnlineBucket = _tempDataCacheDAL.GetUserLoginOnlineBucket(request.LoginTenantId, request.LoginUserId, request.LoginClientType);
            if (userLoginOnlineBucket != null && userLoginOnlineBucket.LoginTime != request.LoginTimestamp)
            {
                var strLoginTenantUser = $"{request.LoginTenantId}_{request.LoginUserId}";
                if (_appConfigurtaionServices.AppSettings.UserConfig == null
                    || _appConfigurtaionServices.AppSettings.UserConfig.LoginWhitelistTenantUser == null
                    || !_appConfigurtaionServices.AppSettings.UserConfig.LoginWhitelistTenantUser.Exists(p => p == strLoginTenantUser))
                {
                    return ResponseBase.CommonError("您的账号已在其他设备登陆，请重新登录！");
                }
            }

            return ResponseBase.Success();
        }

        public async Task<bool> GetUserDataLimit(RequestBase request)
        {
            _etUserDAL.InitTenantId(request.LoginTenantId);
            var user = await _etUserDAL.GetUser(request.LoginUserId);
            _roleDAL.InitTenantId(request.LoginTenantId);
            var role = await _roleDAL.GetRole(user.RoleId);
            return EmDataLimitType.GetIsDataLimit(role.AuthorityValueData);
        }

        public ResponseBase UserCheck(UserCheckRequest request)
        {
            return ResponseBase.Success();
        }
    }
}
