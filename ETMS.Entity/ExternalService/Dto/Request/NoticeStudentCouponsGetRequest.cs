using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentCouponsGetRequest : NoticeRequestBase
    {
        public NoticeStudentCouponsGetRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string OtDesc { get; set; }

        public string Title { get; set; }

        public string CouponsConent { get; set; }

        public List<NoticeStudentCouponsGetStudent> Students { get; set; }
    }

    public class NoticeStudentCouponsGetStudent
    {
        public string Url { get; set; }

        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
    }
}
