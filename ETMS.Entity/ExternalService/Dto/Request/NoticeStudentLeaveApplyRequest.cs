using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentLeaveApplyRequest: NoticeRequestBase
    {
        public NoticeStudentLeaveApplyRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken)
        {
        }

        public string TimeDesc { get; set; }

        public string StartTimeDesc { get; set; }

        public string EndTimeDesc { get; set; }

        public List<NoticeStudentLeaveApplyStudent> Students { get; set; }
    }

    public class NoticeStudentLeaveApplyStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string HandleStatusDesc { get; set; }

        public byte HandleStatus { get; set; }

        public string HandleUser { get; set; }
    }
}
