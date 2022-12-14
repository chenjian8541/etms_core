using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeClassCheckSignRequest : NoticeRequestBase
    {
        public NoticeClassCheckSignRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string ClassTimeDesc { get; set; }

        public string ClassName { get; set; }

        public string TeacherDesc { get; set; }

        public List<NoticeClassCheckSignStudent> Students { get; set; }
    }

    public class NoticeClassCheckSignStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string CourseName { get; set; }

        public string StudentCheckStatusDesc { get; set; }

        public byte StudentCheckStatus { get; set; }

        public string DeClassTimesDesc { get; set; }

        public string SurplusClassTimesDesc { get; set; }

        public string LinkUrl { get; set; }

        public int RewardPoints { get; set; }

        public int Points { get; set; }

        public string ExTimeDesc { get; set; }
    }
}
