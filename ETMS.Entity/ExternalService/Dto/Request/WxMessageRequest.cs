using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class WxMessageRequest : NoticeRequestBase
    {
        public WxMessageRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken)
        {
        }

        public string OtDesc { get; set; }

        public List<WxMessageStudent> Students { get; set; }
    }

    public class WxMessageStudent {

        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Url { get; set; }
    }
}
