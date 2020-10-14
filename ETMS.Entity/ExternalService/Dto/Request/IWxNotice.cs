using System;
using System.Collections.Generic;
using System.Text;
using WxApi.SendEntity;

namespace ETMS.Entity.ExternalService.Dto.Request
{
   public  interface IWxNotice
    {
         string TemplateId { get; set; }

         string Topcolor { get; set; }

        string AccessToken { get; set; }

        string Url { get; set; }

        string Remark { get; set; }
    }
}
