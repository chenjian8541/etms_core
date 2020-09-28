using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.LOG;
using ETMS.WebApi.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETMS.WebApi.Controllers
{
    [Route("api/common/[action]")]
    [ApiController]
    [Authorize]
    public class CommonController : ControllerBase
    {
        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        protected readonly IHttpContextAccessor _httpContextAccessor;

        public CommonController(IAppConfigurtaionServices appConfigurtaionServices, IHttpContextAccessor httpContextAccessor)
        {
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost, ActionName("upImg")]
        public async Task<ResponseBase> UploadImg([FromForm]IFormCollection collection)
        {
            try
            {
                var action = new UploadImgAction();
                return await action.ProcessAction(collection, _httpContextAccessor, _appConfigurtaionServices.AppSettings);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex, this.GetType());
                return new ResponseBase().GetResponseCodeError();
            }
        }
    }
}