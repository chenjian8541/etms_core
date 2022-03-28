using ETMS.Entity.Alien.Common;
using ETMS.Entity.Alien.Dto.User.Request;
using ETMS.Entity.Common;
using ETMS.IBusiness.Alien;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETMS.Alien.Webapi.Controllers
{
    [Route("api/head/[action]")]
    [ApiController]
    [Authorize]
    public class HeadController : ControllerBase
    {
        private readonly IAlienHeadBLL _alienHeadBLL;

        public HeadController(IAlienHeadBLL alienHeadBLL)
        {
            this._alienHeadBLL = alienHeadBLL;
        }

        public async Task<ResponseBase> HeadAllGet(AlienRequestBase request)
        {
            try
            {
                _alienHeadBLL.InitHeadId(request.LoginHeadId);
                return await _alienHeadBLL.HeadAllGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
