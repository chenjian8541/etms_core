using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Senparc.NeuChar;
using System.Xml.Linq;
using Senparc.Weixin.MP.MessageContexts;

namespace ETMS.WebApi.Controllers.Open
{
    public class EtmsCustomMessageContext : DefaultMpMessageContext
    {
        public EtmsCustomMessageContext()
        {
            base.MessageContextRemoved += CustomMessageContext_MessageContextRemoved;
        }

        void CustomMessageContext_MessageContextRemoved(object sender, WeixinContextRemovedEventArgs<IRequestMessageBase, IResponseMessageBase> e)
        {
            var messageContext = e.MessageContext as EtmsCustomMessageContext;
            if (messageContext == null)
            {
                return;
            }
        }
    }
}
