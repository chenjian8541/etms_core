using ETMS.Entity.Common;
using ETMS.Entity.Dto.User.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/user2/[action]")]
    [ApiController]
    [Authorize]
    public class User2Controller : ControllerBase
    {
        private readonly IUser2BLL _user2BLL;

        public User2Controller(IUser2BLL user2BLL)
        {
            this._user2BLL = user2BLL;
        }

        public async Task<ResponseBase> GetAllMenusH5(RequestBase request)
        {
            try
            {
                _user2BLL.InitTenantId(request.LoginTenantId);
                return await _user2BLL.GetAllMenusH5(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetEditMenusH5(RequestBase request)
        {
            try
            {
                _user2BLL.InitTenantId(request.LoginTenantId);
                return await _user2BLL.GetEditMenusH5(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SaveHomeMenusH5(SaveHomeMenusH5Request request)
        {
            try
            {
                _user2BLL.InitTenantId(request.LoginTenantId);
                return await _user2BLL.SaveHomeMenusH5(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
