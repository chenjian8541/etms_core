using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request.User
{
    public class NoticeUserMessageRequest : NoticeRequestBase
    {
        public NoticeUserMessageRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string Title { get; set; }

        public string OtDesc { get; set; }

        public string Content { get; set; }

        public List<NoticeUserMessageUser> Users { get; set; }
    }

    public class NoticeUserMessageUser {
        public long UserId { get; set; }

        public string OpendId { get; set; }

        public string Phone { get; set; }

        public string UserName { get; set; }

        public string Url { get; set; }
    }
}
