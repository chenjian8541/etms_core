using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class HomeworkEditRequest : NoticeRequestBase
    {
        public HomeworkEditRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string ClassName { get; set; }
        public string HomeworkTitle { get; set; }

        public string ExDateDesc { get; set; }

        public List<HomeworkAddEdit> Students { get; set; }
    }

    public class HomeworkAddEdit
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Url { get; set; }
    }
}
