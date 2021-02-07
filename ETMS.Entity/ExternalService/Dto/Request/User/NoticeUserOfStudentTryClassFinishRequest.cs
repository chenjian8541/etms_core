using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request.User
{
    public class NoticeUserOfStudentTryClassFinishRequest : NoticeRequestBase
    {
        public NoticeUserOfStudentTryClassFinishRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string CourseName { get; set; }

        public string ClassOtDesc { get; set; }

        public List<NoticeUserOfStudentTryClassFinishUser> Users { get; set; }
    }

    public class NoticeUserOfStudentTryClassFinishUser
    {
        public long UserId { get; set; }

        public string OpendId { get; set; }

        public string Phone { get; set; }

        public string UserName { get; set; }
    }
}
