using System;
using System.Collections.Generic;
using System.Text;
using WxApi.SendEntity;

namespace ETMS.Entity.ExternalService.Dto.Request
{
   public  interface IWxNotice
    {
         string TemplateIdShort { get; set; }

         string Topcolor { get; set; }

        string AccessToken { get; set; }

        string Url { get; set; }

        string Remark { get; set; }

        bool WechartTemplateMessageLimit { get; set; }

        int WechartAuthorizerId { get; set; }

        string AuthorizerAppid { get; set; }

        string TenantName { get; set; }

        string TenantSmsSignature { get; set; }
    }
}
