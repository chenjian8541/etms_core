using ETMS.Entity.Config;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using System.IO;
using Senparc.NeuChar.Helpers;
using ETMS.IBusiness.Wechart;
using ETMS.IOC;
using System;
using ETMS.LOG;
using Newtonsoft.Json;

namespace ETMS.WebApi.Controllers.Open
{
    /// <summary>
    /// 开放平台全网发布之前需要做的验证
    /// </summary>
    public class OpenCheckMessageHandler : MessageHandler<EtmsCustomMessageContext>
    {
        private readonly ComponentConfig _componentConfig;

        public OpenCheckMessageHandler(Stream inputStream, PostModel postModel, ComponentConfig componentConfig, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            this._componentConfig = componentConfig;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {

            try
            {
                if (requestMessage.Content.StartsWith("TESTCOMPONENT_MSG_TYPE_TEXT"))
                {
                    var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                    responseMessage.Content = "TESTCOMPONENT_MSG_TYPE_TEXT_callback";//固定为TESTCOMPONENT_MSG_TYPE_TEXT_callback
                    return responseMessage;
                }

                if (requestMessage.Content.StartsWith("QUERY_AUTH_CODE:"))
                {
                    var componentAccessBLL = CustomServiceLocator.GetInstance<IComponentAccessBLL>();
                    var openTicket = componentAccessBLL.GetSysWechartVerifyTicket(_componentConfig.ComponentAppid).Result;
                    var query_auth_code = requestMessage.Content.Replace("QUERY_AUTH_CODE:", "");
                    var component_access_token = Senparc.Weixin.Open.ComponentAPIs.ComponentApi.GetComponentAccessToken(_componentConfig.ComponentAppid,
                        _componentConfig.ComponentSecret, openTicket).component_access_token;
                    var oauthResult = Senparc.Weixin.Open.ComponentAPIs.ComponentApi.QueryAuth(component_access_token, _componentConfig.ComponentAppid, query_auth_code);

                    //调用客服接口
                    var content = query_auth_code + "_from_api";
                    var sendResult = Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(oauthResult.authorization_info.authorizer_access_token,
                          requestMessage.FromUserName, content);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[OpenCheckMessageHandler]OnTextRequest:{JsonConvert.SerializeObject(requestMessage)}", ex, this.GetType());
            }
            return null;
        }

        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = requestMessage.Event + "from_callback";
            return responseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "默认消息";
            return responseMessage;
        }
    }
}
