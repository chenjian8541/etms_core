using ETMS.Entity.Alien.Common;
using ETMS.Entity.Alien.Dto.User.Request;
using ETMS.Entity.Common;
using ETMS.IBusiness.Alien;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETMS.Alien.Webapi.Controllers
{
    [Route("api/user/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IAlienUserLoginBLL _alienUserBLL;

        private readonly IAlienUserBLL _alienUserBLL1;

        public UserController(IAlienUserLoginBLL alienUserBLL, IAlienUserBLL alienUserBLL1)
        {
            this._alienUserBLL = alienUserBLL;
            this._alienUserBLL1 = alienUserBLL1;
        }

        [AllowAnonymous]
        public async Task<ResponseBase> UserLogin(AlUserLoginRequest request)
        {
            try
            {
                return await _alienUserBLL.UserLogin(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserLoginGet(AlienRequestBase request)
        {
            try
            {
                _alienUserBLL.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL.UserLoginGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserLoginPermissionGet(AlienRequestBase request)
        {
            try
            {
                _alienUserBLL.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL.UserLoginPermissionGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ChangPwd(ChangPwdRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.ChangPwd(request);
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
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.ChangUserPwd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> RoleListGet(RoleListGetRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.RoleListGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> RoleAdd(RoleAddRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.RoleAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> RoleEdit(RoleEditRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.RoleEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> RoleGet(RoleGetRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.RoleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase RoleDefaultGet(RoleDefaultGetRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return _alienUserBLL1.RoleDefaultGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> RoleDel(RoleDelRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.RoleDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrgGetAll(OrgGetAllRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.OrgGetAll(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrgAdd(OrgAddRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.OrgAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrgEdit(OrgEditRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.OrgEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrgDel(OrgDelRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.OrgDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserGetPaging(UserGetPagingRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.UserGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserGet(UserGetRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.UserGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserGetSelf(AlienRequestBase request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.UserGetSelf(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserAdd(UserAddRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.UserAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserDel(UserDelRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.UserDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserEdit(UserEditRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.UserEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserOperationLogGetPaging(UserOperationLogGetPagingRequest request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL1.UserOperationLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase UserLogTypeGet(AlienRequestBase request)
        {
            try
            {
                _alienUserBLL1.InitHeadId(request.LoginHeadId);
                return _alienUserBLL1.UserLogTypeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantLogin(TenantLoginRequest request)
        {
            try
            {
                _alienUserBLL.InitHeadId(request.LoginHeadId);
                return await _alienUserBLL.TenantLogin(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
