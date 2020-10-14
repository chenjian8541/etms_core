using ETMS.Entity.Config;
using ETMS.Entity.Dto.Wx.Request;
using ETMS.WxApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WxController : ControllerBase
    {
        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        public WxController(IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._appConfigurtaionServices = appConfigurtaionServices;
        }

        [HttpGet]
        public string Index([FromQuery]IndexRequest request)
        {
            LOG.Log.Info($"[验证消息的确来自微信服务器]{JsonConvert.SerializeObject(request)}", this.GetType());
            var appSettings = this._appConfigurtaionServices.AppSettings;
            var result = BaseServices.ValidUrl(appSettings.WxConfig.Token, request.Signature, request.Timestamp, request.Nonce, request.Echostr);
            if (result)
            {
                return request.Echostr;
            }
            return string.Empty;
        }
    }
}
