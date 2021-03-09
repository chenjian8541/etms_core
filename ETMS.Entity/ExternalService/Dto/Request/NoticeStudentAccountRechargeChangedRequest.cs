using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentAccountRechargeChangedRequest : NoticeRequestBase
    {
        public NoticeStudentAccountRechargeChangedRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string AccountRechargePhone { get; set; }

        public string OtDesc { get; set; }

        public string BalanceDesc { get; set; }

        public string BalanceRealDesc { get; set; }

        public string BalanceGiveDesc { get; set; }

        public string ChangeSumDesc { get; set; }

        public List<NoticeStudentAccountRechargeChangedStudent> Students { get; set; }
    }

    public class NoticeStudentAccountRechargeChangedStudent
    {
        public long StudentId { get; set; }

        public string OpendId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
    }
}
