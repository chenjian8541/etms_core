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
        public ActionResult ComponentNotice([FromQuery]PostModel postModel)
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
        public async Task<ActionResult> ComponentCallback([FromQuery]Senparc.Weixin.MP.Entities.Request.PostModel postModel, string appId)
        {
            try
            {
                Log.Debug($"[ComponentCallback]postModel:{JsonConvert.SerializeObject(postModel)}", this.GetType());
                var appSettings = this._appConfigurtaionServices.AppSettings;
                postModel.Token = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentToken;
                postModel.EncodingAESKey = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentEncodingAESKey;
                postModel.AppId = appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;

                MessageHandler<EtmsCustomMessageContext> messageHandler = null;
                if (appSettings.SenparcConfig.CheckPublish)
                {
                    messageHandler = new OpenCheckMessageHandler(Request.GetRequestMemoryStream(), postModel, appSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig, 10);
                }
                else
                {
                    messageHandler = new EtmsCustomMessageHandler(Request.GetRequestMemoryStream(), postModel, 10);
                }
                await messageHandler.ExecuteAsync(new System.Threading.CancellationToken());
                return Content(messageHandler.TextResponseMessage);
            }
            catch (Exception ex)
            {
                Log.Error(postModel, ex, this.GetType());
                return Content("error：" + ex.Message);
            }
        }
    }
}
