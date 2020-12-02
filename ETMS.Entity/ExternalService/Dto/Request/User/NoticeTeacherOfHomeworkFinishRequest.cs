using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request.User
{
    public class NoticeTeacherOfHomeworkFinishRequest : NoticeRequestBase
    {
        public NoticeTeacherOfHomeworkFinishRequest(NoticeRequestBase requestBase)
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

        public string HomeworkTitle { get; set; }

        public string FinishTime { get; set; }

        public List<NoticeTeacherOfHomeworkFinishItem> Users { get; set; }
    }

    public class NoticeTeacherOfHomeworkFinishItem
    {
        public long UserId { get; set; }

        public string OpendId { get; set; }

        public string Phone { get; set; }

        public string UserName { get; set; }
    }
}
