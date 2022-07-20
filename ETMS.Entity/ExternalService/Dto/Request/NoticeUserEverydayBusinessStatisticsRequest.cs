using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeUserEverydayBusinessStatisticsRequest : NoticeRequestBase
    {
        public NoticeUserEverydayBusinessStatisticsRequest(NoticeRequestBase requestBase)
            : base(requestBase.LoginTenantId,
                  requestBase.WechartAuthorizerId,
                  requestBase.TenantName,
                  requestBase.TenantSmsSignature,
                  requestBase.WechartTemplateMessageLimit,
                  requestBase.AccessToken,
                  requestBase.AuthorizerAppid)
        {
        }

        public string Title { get; set; }

        public string OtDesc { get; set; }

        public string Content { get; set; }

        public List<NoticeUserEverydayBusinessStatisticsStudent> Users { get; set; }
    }

    public class NoticeUserEverydayBusinessStatisticsStudent {
        public long UserId { get; set; }

        public string OpendId { get; set; }

        public string UserName { get; set; }
    }
}
