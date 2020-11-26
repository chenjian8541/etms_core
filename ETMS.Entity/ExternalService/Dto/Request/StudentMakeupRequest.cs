using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class StudentMakeupRequest : NoticeRequestBase
    {
        public StudentMakeupRequest(NoticeRequestBase requestBase)
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

        public string ClassOt { get; set; }

        public string ClassTime { get; set; }

        public string TeacherDesc { get; set; }

        public List<StudentMakeupItem> Students { get; set; }
    }

    public class StudentMakeupItem {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
    }
}
