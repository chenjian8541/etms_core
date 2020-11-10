using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using System.Threading.Tasks;
using Senparc.NeuChar.Entities.Request;
using Senparc.NeuChar.Helpers;
using ETMS.LOG;

namespace ETMS.WebApi.Controllers.Open
{
    public partial class EtmsCustomMessageHandler : MessageHandler<EtmsCustomMessageContext>
    {
        /// <summary>
        /// 模板消息集合（Key：checkCode，Value：OpenId）
        /// 注意：这里只做测试，只适用于单服务器
        /// </summary>
        public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();

        /// <summary>
        /// 为中间件提供生成当前类的委托
        /// </summary>
        public static Func<Stream, PostModel, int, EtmsCustomMessageHandler> GenerateMessageHandler = (stream, postModel, maxRecordCount)
            => new EtmsCustomMessageHandler(stream, postModel, maxRecordCount, false /* 是否只允许处理加密消息，以提高安全性 */);

        public EtmsCustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, bool onlyAllowEcryptMessage = false)
            : base(inputStream, postModel, maxRecordCount, onlyAllowEcryptMessage)
        {
            GlobalMessageContext.ExpireMinutes = 3;

            OnlyAllowEncryptMessage = true;//是否只允许接收加密消息，默认为 false

            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                // appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            }

            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseText = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseText.Content = "祝您学习进步，生活愉快≧◠◡◠≦";
            return responseText;
        }

        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnLocationRequestAsync(RequestMessageLocation requestMessage)
        {
            var responseText = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseText.Content = "愿您以梦为马,不负韶华";
            return responseText;
        }


        public override async Task<IResponseMessageBase> OnShortVideoRequestAsync(RequestMessageShortVideo requestMessage)
        {
            var responseText = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseText.Content = "愿您以梦为马,不负韶华";
            return responseText;
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnImageRequestAsync(RequestMessageImage requestMessage)
        {
            if (base.GlobalMessageContext.GetMessageContext(requestMessage).RequestMessages.Count() % 2 == 0)
            {
                var responseMessage = CreateResponseMessage<ResponseMessageNews>();

                responseMessage.Articles.Add(new Article()
                {
                    Title = "您刚才发送了图片信息",
                    Description = "您发送的图片将会显示在边上",
                    PicUrl = requestMessage.PicUrl,
                    Url = ""
                });
                responseMessage.Articles.Add(new Article()
                {
                    Title = "第二条",
                    Description = "第二条带连接的内容",
                    PicUrl = requestMessage.PicUrl,
                    Url = ""
                });

                return responseMessage;
            }
            else
            {
                var responseMessage = CreateResponseMessage<ResponseMessageImage>();
                responseMessage.Image.MediaId = requestMessage.MediaId;
                return responseMessage;
            }
        }

        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnLinkRequestAsync(RequestMessageLink requestMessage)
        {
            var responseText = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseText.Content = "愿您以梦为马,不负韶华";
            return responseText;
        }

        public override async Task<IResponseMessageBase> OnFileRequestAsync(RequestMessageFile requestMessage)
        {
            var responseText = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseText.Content = "愿您以梦为马,不负韶华";
            return responseText;
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEventRequestAsync(IRequestMessageEventBase requestMessage)
        {
            var responseText = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseText.Content = "愿您以梦为马,不负韶华";
            return responseText;
        }


        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "愿您以梦为马,不负韶华";// "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }

        public override async Task<IResponseMessageBase> OnUnknownTypeRequestAsync(RequestMessageUnknownType requestMessage)
        {
            /*
             * 此方法用于应急处理SDK没有提供的消息类型，
             * 原始XML可以通过requestMessage.RequestDocument（或this.RequestDocument）获取到。
             * 如果不重写此方法，遇到未知的请求类型将会抛出异常（v14.8.3 之前的版本就是这么做的）
             */
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "愿您以梦为马,不负韶华";

            Log.Error("[OnUnknownTypeRequestAsync]微信消息：未知请求消息类型", this.GetType());

            return responseMessage;
        }

    }
}
