using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class StudentCourseSurplusRequest : NoticeRequestBase
    {
        public StudentCourseSurplusRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public List<StudentCourseSurplusItem> Students { get; set; }
    }

    public class StudentCourseSurplusItem
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string CourseName { get; set; }

        public string Url { get; set; }

        public string SurplusQuantityDesc { get; set; }
    }
}
