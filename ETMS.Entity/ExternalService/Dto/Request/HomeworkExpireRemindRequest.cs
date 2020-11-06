using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class HomeworkExpireRemindRequest : NoticeRequestBase
    {
        public HomeworkExpireRemindRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken)
        {
        }
        public List<HomeworkExpireRemindStudent> Students { get; set; }
    }

    public class HomeworkExpireRemindStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Url { get; set; }

        public string ExDateDesc { get; set; }

        public string HomeworkTitle { get; set; }

        public string ClassName { get; set; }
    }
}
