using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class HomeworkCommentRequest : NoticeRequestBase
    {
        public HomeworkCommentRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken)
        {
        }

        public string HomeworkTitle { get; set; }

        public string OtDesc { get; set; }

        public string UserName { get; set; }

        public List<HomeworkCommentStudent> Students { get; set; }

    }

    public class HomeworkCommentStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Url { get; set; }
    }
}
