using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentCheckOutRequest : NoticeRequestBase
    {
        public NoticeStudentCheckOutRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string CheckOtDesc { get; set; }

        public List<NoticeStudentCheckOutStudent> Students { get; set; }
    }

    public class NoticeStudentCheckOutStudent
    {
        public string Url { get; set; }

        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
    }
}
