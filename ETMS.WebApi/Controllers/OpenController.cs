using ETMS.Entity.Config;
using ETMS.IBusiness.Wechart;
using ETMS.LOG;
using ETMS.Utility;
using ETMS.WebApi.Controllers.Open;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Open.Containers;
using Senparc.Weixin.Open.Entities.Request;
using System;
using System.Threading.Tasks;
using Senparc.Weixin.MP;

namespace ETMS.WebApi.Controllers
{
    [Route("Open/[action]")]
    [ApiController]
    [Authorize]
    public class OpenController : ControllerBase
    {
        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        public OpenController(IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._appConfigurtaionServices = appConfigurtaionServices;
        }

        /// <summary>
        /// 授权事件接收URL
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ComponentNotice([FromQuery] PostModel postModel)
        {
            try
            {
                Log.Debug($"[ComponentNotice]postModel:{JsonConvert.SerializeObject(postModel)}", this.GetType());
                var appSettings = this._appConfigurtaionServices.AppSettings;
                postModel.Token = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentToken;
                postModel.EncodingAESKey = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentEncodingAESKey;
                postModel.AppId = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
                var messageHandler = new EtmsThirdPartyMessageHandler(Request.GetRequestMemoryStream(), postModel);
                messageHandler.Execute();
                return Content(messageHandler.ResponseMessageText);
            }
            catch (Exception ex)
            {
                Log.Error(postModel, ex, this.GetType());
                return Content("success");
            }
        }

        /// <summary>
        /// 消息与事件接收URL
        /// </summary>
        /// <param name="postModel"></param>
        /// <returns></returns>
        [HttpPost("{appId}")]
        [AllowAnonymous]
        public async Task<ActionResult> ComponentCallback([FromQuery] Senparc.Weixin.MP.Entities.Request.PostModel postModel, string appId)
        {
            try
            {
                var appSettings = this._appConfigurtaionServices.AppSettings;
                if (appSettings.SenparcConfig.CheckPublish)
                {
                    postModel.Token = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentToken;
                    postModel.EncodingAESKey = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentEncodingAESKey;
                    postModel.AppId = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
                    var messageHandler = new OpenCheckMessageHandler(Request.GetRequestMemoryStream(), postModel, appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig, 10);
                    await messageHandler.ExecuteAsync(new System.Threading.CancellationToken());
                    return Content(messageHandler.TextResponseMessage);
                }
                else
                {
                    return Content("success");
                    //messageHandler = new EtmsCustomMessageHandler(Request.GetRequestMemoryStream(), postModel, 10);
                }
            }
            catch (Exception ex)
            {
                Log.Error(postModel, ex, this.GetType());
                return Content("error：" + ex.Message);
            }
        }

        /// <summary>
        /// GET请求用于处理微信小程序后台的URL验证
        /// </summary>
        /// <param name="postModel"></param>
        /// <param name="echostr"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ActionName("MiniProgramUrlCheck")]
        public ActionResult MiniProgramUrlCheck(PostModel postModel, string echostr)
        {
            var appSettings = this._appConfigurtaionServices.AppSettings;
            var miniProgramConfig = appSettings.SenparcConfig.SenparcWeixinSetting.MiniProgramConfig;
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, miniProgramConfig.WxOpenToken))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, miniProgramConfig.WxOpenToken) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信小程序后台的Url，请注意保持Token一致。");
            }
        }
    }
}
