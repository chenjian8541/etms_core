using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class AIFaceBiduAccountAddRequest : AgentRequestBase
    {
        public string Appid { get; set; }

        public string ApiKey { get; set; }

        public string SecretKey { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Appid))
            {
                return "请求参数不完整";
            }
            if (string.IsNullOrEmpty(ApiKey))
            {
                return "请求参数不完整";
            }
            if (string.IsNullOrEmpty(SecretKey))
            {
                return "请求参数不完整";
            }
            return base.Validate();
        }
    }
}
