using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentReservationRequest : NoticeRequestBase
    {
        public NoticeStudentReservationRequest(NoticeRequestBase requestBase)
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

        public string CourseDesc { get; set; }

        public string ClassOtDesc { get; set; }

        public string StudentCount { get; set; }

        public List<NoticeStudentReservationStudent> Students { get; set; }
    }

    public class NoticeStudentReservationStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
    }
}
