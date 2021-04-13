using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class AIFaceBiduAccountEditRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public string Appid { get; set; }

        public string ApiKey { get; set; }

        public string SecretKey { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "请求数据不合法";
            }
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
