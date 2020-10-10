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

        public UserLoginBLL(ISysTenantDAL sysTenantDAL, IUserDAL etUserDAL, IUserOperationLogDAL etUserOperationLogDAL,
            IUserLoginFailedRecordDAL userLoginFailedRecordDAL, IAppConfigurtaionServices appConfigurtaionServices,
            ISmsService smsService, IUserLoginSmsCodeDAL userLoginSmsCodeDAL, IRoleDAL roleDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._etUserDAL = etUserDAL;
            this._etUserOperationLogDAL = etUserOperationLogDAL;
            this._userLoginFailedRecordDAL = userLoginFailedRecordDAL;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._smsService = smsService;
            this._userLoginSmsCodeDAL = userLoginSmsCodeDAL;
            this._roleDAL = roleDAL;
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
            var userLoginOutput = await LoginSuccessProcess(userInfo, request.IpAddress, request.Code, request.Phone);
            return response.GetResponseSuccess(userLoginOutput);
        }

        private bool CheckUserCanLogin(EtUser user, out string msg)
        {
            msg = string.Empty;
            if (user == null)
            {
                msg = "用户存在,请重新登陆";
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
            var userLoginOutput = await LoginSuccessProcess(userInfo, request.IpAddress, request.Code, request.Phone);
            return response.GetResponseSuccess(userLoginOutput);
        }

        public async Task<ResponseBase> UserLoginBySmsH5(UserLoginBySmsH5Request request)
        {
            var res = await UserLoginBySms(new UserLoginBySmsRequest()
            {
                Code = request.Code,
                IpAddress = request.IpAddress,
                Phone = request.Phone,
                SmsCode = request.SmsCode
            });
            if (res.IsResponseSuccess())
            {
                var result = (UserLoginOutput)res.resultData;
                return ResponseBase.Success(new UserLoginBySmsH5Output()
                {
                    ExpiresTime = result.ExpiresTime,
                    Token = result.Token
                });
            }
            return res;
        }

        private async Task<UserLoginOutput> LoginSuccessProcess(EtUser userInfo, string ipAddress, string code, string phone)
        {
            var token = JwtHelper.GenerateToken(userInfo.TenantId, userInfo.Id, out var exTime);
            var time = DateTime.Now;
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
                Type = (int)EmUserOperationType.Login
            });
            _roleDAL.InitTenantId(userInfo.TenantId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            return new UserLoginOutput()
            {
                Token = token,
                ExpiresTime = exTime,
                Permission = ComBusiness.GetPermissionOutput(role.AuthorityValueMenu)
            };
        }

        public async Task<ResponseBase> CheckUserCanLogin(RequestBase request)
        {
            _etUserDAL.InitTenantId(request.LoginTenantId);
            var user = await _etUserDAL.GetUser(request.LoginUserId);
            if (!CheckUserCanLogin(user, out var msg))
            {
                return ResponseBase.CommonError(msg);
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
    }
}
