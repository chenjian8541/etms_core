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

        public UserController(IAlienUserLoginBLL alienUserBLL)
        {
            this._alienUserBLL = alienUserBLL;
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
    }
}
