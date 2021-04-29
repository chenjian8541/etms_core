using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentMessageRequest : NoticeRequestBase
    {
        public NoticeStudentMessageRequest(NoticeRequestBase requestBase)
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

        public List<NoticeStudentMessageStudent> Students { get; set; }
    }

    public class NoticeStudentMessageStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Url { get; set; }
    }
}
