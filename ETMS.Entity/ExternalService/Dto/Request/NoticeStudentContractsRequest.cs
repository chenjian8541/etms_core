using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentContractsRequest : NoticeRequestBase
    {
        public NoticeStudentContractsRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken)
        {
        }

        public string TimeDedc { get; set; }

        public string BuyDesc { get; set; }

        public string AptSumDesc { get; set; }

        public string PaySumDesc { get; set; }

        public string OrderNo { get; set; }

        public List<NoticeStudentContractsStudent> Students { get; set; }
    }

    public class NoticeStudentContractsStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
    }
}
