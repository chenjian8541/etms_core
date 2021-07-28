using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.User;
using ETMS.Entity.Dto.User.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETMS.WebApi.Controllers
{
    [Route("api/user/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserLoginBLL _userLoginBLL;

        private readonly IUserBLL _userBLL;
        public UserController(IUserLoginBLL userLoginBLL, IUserBLL userBLL)
        {
            this._userLoginBLL = userLoginBLL;
            this._userBLL = userBLL;
        }

        [AllowAnonymous]
        public async Task<ResponseBase> UserGetAuthorizeUrl(UserGetAuthorizeUrlRequest request)
        {
            try
            {
                return await _userLoginBLL.UserGetAuthorizeUrl(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserGetAuthorizeUrl2(UserGetAuthorizeUrl2Request request)
        {
            try
            {
                return await _userLoginBLL.UserGetAuthorizeUrl2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserBindingWeChat(UserBindingWeChatRequest request)
        {
            try
            {
                return await _userLoginBLL.UserBindingWeChat(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBase> Login(UserLoginRequest request)
        {
            try
            {
                return await _userLoginBLL.UserLogin(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("smsCode")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBase> LoginSendSms(UserLoginSendSmsRequest request)
        {
            try
            {
                return await _userLoginBLL.UserLoginSendSms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 短信登陆
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("smsLogin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBase> LoginBySms(UserLoginBySmsRequest request)
        {
            try
            {
                return await _userLoginBLL.UserLoginBySms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase UserCheck(UserCheckRequest request)
        {
            try
            {
                return _userLoginBLL.UserCheck(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserGetCurrentTenant(RequestBase request)
        {
            try
            {
                return await _userLoginBLL.UserGetCurrentTenant(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserGetTenants(RequestBase request)
        {
            try
            {
                return await _userLoginBLL.UserGetTenants(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserTenantEntrance(UserTenantEntranceRequest request)
        {
            try
            {
                return await _userLoginBLL.UserTenantEntrance(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 短信登陆H5
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("smsLoginH5")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBase> UserLoginBySmsH5(UserLoginBySmsH5Request request)
        {
            try
            {
                return await _userLoginBLL.UserLoginBySmsH5(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 登陆H5
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("loginH5")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseBase> UserLoginByH5(UserLoginByH5Request request)
        {
            try
            {
                return await _userLoginBLL.UserLoginByH5(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> GetOemLogin(GetOemLoginRequest request)
        {
            try
            {
                return await _userBLL.GetOemLogin(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetOemLogin2(RequestBase request)
        {
            try
            {
                return await _userBLL.GetOemLogin2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 获取登陆信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("loginInfo")]
        [HttpPost]
        public async Task<ResponseBase> GetLoginInfo(GetLoginInfoRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.GetLoginInfo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 获取登陆信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("loginInfoH5")]
        [HttpPost]
        public async Task<ResponseBase> GetLoginInfoH5(GetLoginInfoRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.GetLoginInfoH5(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetLoginPermission(GetLoginPermissionRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.GetLoginPermission(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 更新用户信息(头像，昵称)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("upUserSimple")]
        [HttpPost]
        public async Task<ResponseBase> UpdateUserInfo(UpdateUserInfoRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.UpdateUserInfo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        ///// <summary>
        ///// 修改密码发送短信
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[ActionName("changePwdSms")]
        //[HttpPost]
        //public async Task<ResponseBase> ChangPwdSenSms(ChangPwdSendSmsRequest request)
        //{
        //    try
        //    {
        //        _userBLL.InitTenantId(request.LoginTenantId);
        //        return await _userBLL.ChangPwdSendSms(request);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(request, ex, this.GetType());
        //        return ResponseBase.UnKnownError();
        //    }
        //}

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("changePwd")]
        [HttpPost]
        public async Task<ResponseBase> ChangPwd(ChangPwdRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.ChangPwd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ChangUserPwd(ChangUserPwdRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.ChangUserPwd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }


        [ActionName("roleListGet")]
        [HttpPost]
        public async Task<ResponseBase> RoleListGet(RoleListGetRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.RoleListGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("roleAdd")]
        [HttpPost]
        public async Task<ResponseBase> RoleAdd(RoleAddRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.RoleAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("roleEdit")]
        [HttpPost]
        public async Task<ResponseBase> RoleEdit(RoleEditRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.RoleEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("roleGet")]
        [HttpPost]
        public async Task<ResponseBase> RoleGet(RoleGetRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.RoleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> RoleDefaultGet(RoleDefaultGetRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.RoleDefaultGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("roleDel")]
        [HttpPost]
        public async Task<ResponseBase> RoleDel(RoleDelRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.RoleDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("userAdd")]
        [HttpPost]
        public async Task<ResponseBase> UserAdd(UserAddRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.UserAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("userEdit")]
        [HttpPost]
        public async Task<ResponseBase> UserEdit(UserEditRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.UserEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("userDel")]
        [HttpPost]
        public async Task<ResponseBase> UserDel(UserDelRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.UserDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("userGetPaging")]
        [HttpPost]
        public async Task<ResponseBase> UserGetPaging(UserGetPagingRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.UserGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("teacherGetPaging")]
        [HttpPost]
        public async Task<ResponseBase> TeacherGetPaging(UserGetPagingRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.TeacherGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("userLogGetPaging")]
        [HttpPost]
        public async Task<ResponseBase> UserOperationLogGetPaging(UserOperationLogGetPagingRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.UserOperationLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("userLogTypeGet")]
        [HttpPost]
        public ResponseBase UserOperationLogTypeGet(RequestBase request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return _userBLL.UserOperationLogTypeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("teacherEdit")]
        [HttpPost]
        public async Task<ResponseBase> TeacherEdit(TeacherEditRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.TeacherEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("teacherRemove")]
        [HttpPost]
        public async Task<ResponseBase> TeacherRemove(TeacherRemoveRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.TeacherRemove(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("userGet")]
        [HttpPost]
        public async Task<ResponseBase> UserGet(UserGetRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.UserGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherClassTimesGetPaging(TeacherClassTimesGetPagingRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.TeacherClassTimesGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserFeedback(UserFeedbackRequest request)
        {
            try
            {
                _userBLL.InitTenantId(request.LoginTenantId);
                return await _userBLL.UserFeedback(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}