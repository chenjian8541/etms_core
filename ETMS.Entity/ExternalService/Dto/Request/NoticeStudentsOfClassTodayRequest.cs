using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentsOfClassTodayRequest : NoticeRequestBase
    {
        public NoticeStudentsOfClassTodayRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string ClassRoom { get; set; }

        public string StartTimeDesc { get; set; }

        public string ClassTimeDesc { get; set; }

        public string ClassTimeFullDesc { get; set; }

        public List<NoticeStudentsOfClassTodayStudent> Students { get; set; }
    }

    public class NoticeStudentsOfClassTodayStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Phone { get; set; }

        public string StudentName { get; set; }

        public string CourseName { get; set; }
    }
}
