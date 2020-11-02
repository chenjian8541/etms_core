using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Business.WxCore
{
    public class WeChatLimit
    {
        public static bool IsSendTemplateMessage(int tenantId, string serviceTypeInfo, string verifyTypeInfo)
        {
            if (serviceTypeInfo != EmWechartAuthServiceTypeInfo.ServiceType2
                  && verifyTypeInfo == EmWechartAuthVerifyTypeInfo.VerifyType1Less)
            {
                LOG.Log.Warn($"[IsSendTemplateMessage]tenantId:{tenantId},无法发送模板消息", typeof(WeChatLimit));
                return false;
            }
            return true;
        }
    }
}
