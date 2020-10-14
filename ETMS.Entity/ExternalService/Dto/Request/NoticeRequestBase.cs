using System;
using System.Collections.Generic;
using System.Text;
using WxApi.SendEntity;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeRequestBase : SmsBase, IWxNotice
    {
        public NoticeRequestBase(int tenantId) : base(tenantId)
        {
        }

        public string TemplateId { get; set; }
        public string Topcolor { get; set; } = "#FF0000";
        public string AccessToken { get; set; }
        public string Url { get; set; }

        public string Remark { get; set; }
    }
}
