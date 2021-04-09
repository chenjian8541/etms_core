using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentCustomizeMsgRequest : NoticeRequestBase
    {
        public NoticeStudentCustomizeMsgRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string OtTime { get; set; }

        public List<NoticeStudentCustomizeMsgStudent> Students { get; set; }
    }

    public class NoticeStudentCustomizeMsgStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Title { get; set; }

        public string Msg { get; set; } = "";
    }
}
