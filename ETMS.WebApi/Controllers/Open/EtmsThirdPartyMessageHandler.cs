using ETMS.IBusiness.Wechart;
using ETMS.IOC;
using ETMS.LOG;
using Newtonsoft.Json;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.Weixin.Open.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers.Open
{
    public class EtmsThirdPartyMessageHandler : ThirdPartyMessageHandler
    {
        public EtmsThirdPartyMessageHandler(Stream inputStream, PostModel encryptPostModel)
            : base(inputStream, encryptPostModel)
        { }

        /// <summary>
        ///  推送component_verify_ticket协议
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override string OnComponentVerifyTicketRequest(RequestMessageComponentVerifyTicket requestMessage)
        {
            Log.Debug($"[OnComponentVerifyTicketRequest]requestMessage:{JsonConvert.SerializeObject(requestMessage)}", this.GetType());
            try
            {
                var componentAccessBLL = CustomServiceLocator.GetInstance<IComponentAccessBLL>();
                componentAccessBLL.SaveSysWechartVerifyTicket(requestMessage.AppId, requestMessage.ComponentVerifyTicket).Wait();
            }
            catch (Exception ex)
            {
                Log.Error(requestMessage, ex, this.GetType());
            }
            return base.OnComponentVerifyTicketRequest(requestMessage);
        }

        /// <summary>
        /// 推送取消授权通知
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override string OnUnauthorizedRequest(RequestMessageUnauthorized requestMessage)
        {
            Log.Debug($"[OnUnauthorizedRequest]requestMessage:{JsonConvert.SerializeObject(requestMessage)}", this.GetType());
            try
            {
                var componentAccessBLL = CustomServiceLocator.GetInstance<IComponentAccessBLL>();
                componentAccessBLL.OnUnauthorizeTenantWechart(requestMessage.AuthorizerAppid).Wait();
            }
            catch (Exception ex)
            {
                Log.Error(requestMessage, ex, this.GetType());
            }
            return base.OnUnauthorizedRequest(requestMessage);
        }
    }
}
