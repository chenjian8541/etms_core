using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class GrowthRecordAddRequest : NoticeRequestBase
    {
        public GrowthRecordAddRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public List<GrowthRecordAddStudent> Students { get; set; }
    }

    public class GrowthRecordAddStudent
    {

        public string ClassName { get; set; }

        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Url { get; set; }
    }
}
