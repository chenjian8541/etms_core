using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentCourseNotEnoughRequest : NoticeRequestBase
    {
        public NoticeStudentCourseNotEnoughRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string CourseName { get; set; }

        public string NotEnoughDesc { get; set; }

        public List<NoticeStudentCourseNotEnoughStudent> Students { get; set; }
    }

    public class NoticeStudentCourseNotEnoughStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Phone { get; set; }

        public string StudentName { get; set; }
    }
}
