using ETMS.Entity.Alien.Common;
using ETMS.Entity.Alien.Dto.User.Request;
using ETMS.Entity.Common;
using ETMS.IBusiness.Alien;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETMS.Alien.Webapi.Controllers
{
    [Route("api/basc/[action]")]
    [ApiController]
    [Authorize]
    public class BascController : ControllerBase
    {
        private readonly IAlienBascBLL _alienBascBLL;

        public BascController(IAlienBascBLL alienBascBLL)
        {
            this._alienBascBLL = alienBascBLL;
        }

        public ResponseBase UploadConfigGet(AlienRequestBase request)
        {
            try
            {
                _alienBascBLL.InitHeadId(request.LoginHeadId);
                return _alienBascBLL.UploadConfigGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
