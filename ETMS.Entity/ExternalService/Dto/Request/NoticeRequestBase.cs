using System;
using System.Collections.Generic;
using System.Text;
using WxApi.SendEntity;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeRequestBase : SmsBase, IWxNotice
    {
        public NoticeRequestBase(int tenantId,
            int wechartAuthorizerId,
            string tenantName, string tenantSmsSignature,
            bool wxTemplateLimit, string accessToken, string authorizerAppid) : base(tenantId)
        {
            this.WechartAuthorizerId = wechartAuthorizerId;
            this.TenantName = tenantName;
            this.TenantSmsSignature = tenantSmsSignature;
            this.WechartTemplateMessageLimit = wxTemplateLimit;
            this.AccessToken = accessToken;
            this.AuthorizerAppid = authorizerAppid;
        }

        public string TemplateIdShort { get; set; }

        public string TemplateId { get; set; }

        public string Topcolor { get; set; } = "#FF0000";
        public string AccessToken { get; set; }
        public string Url { get; set; }

        public string Remark { get; set; }

        public int WechartAuthorizerId { get; set; }

        public string AuthorizerAppid { get; set; }

        public string TenantName { get; set; }

        public string TenantSmsSignature { get; set; }

        public bool WechartTemplateMessageLimit { get; set; }

        public string SmsTemplate { get; set; }
    }
}
