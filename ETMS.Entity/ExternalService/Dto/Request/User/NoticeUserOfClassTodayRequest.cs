using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request.User
{
    public class NoticeUserOfClassTodayRequest : NoticeRequestBase
    {
        public NoticeUserOfClassTodayRequest(NoticeRequestBase requestBase)
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

        public List<NoticeUserOfClassTodayTeacher> Users { get; set; }
    }

    public class NoticeUserOfClassTodayTeacher
    {

        public long UserId { get; set; }

        public string OpendId { get; set; }

        public string Phone { get; set; }

        public string UserName { get; set; }

        public string CourseName { get; set; }
    }
}
