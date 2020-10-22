using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Student.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using ETMS.WebApi.Controllers.Common;
using ETMS.WebApi.FilterAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/parentCommon/[action]")]
    [ApiController]
    [EtmsSignatureAuthorize]
    public class ParentCommonController
    {
        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        protected readonly IHttpContextAccessor _httpContextAccessor;

        public ParentCommonController(IAppConfigurtaionServices appConfigurtaionServices, IHttpContextAccessor httpContextAccessor)
        {
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AllowAnonymous]
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

        /// <summary>
        /// 上传视频文件
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, ActionName("upVideo")]
        public async Task<ResponseBase> UploadVideo([FromForm]IFormCollection collection)
        {
            try
            {
                var action = new UploadVideoAction();
                return await action.ProcessAction(collection, _httpContextAccessor, _appConfigurtaionServices.AppSettings);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex, this.GetType());
                return new ResponseBase().GetResponseCodeError();
            }
        }

        /// <summary>
        /// 上传音频文件
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, ActionName("upAudio")]
        public async Task<ResponseBase> UploadAudio([FromForm]IFormCollection collection)
        {
            try
            {
                var action = new UploadAudioAction();
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
