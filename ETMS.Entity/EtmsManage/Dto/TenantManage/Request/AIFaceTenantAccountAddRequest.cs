using ETMS.Entity.EtmsManage.Common;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class AIFaceTenantAccountAddRequest : AgentRequestBase
    {
        public string SecretId { get; set; }

        public string SecretKey { get; set; }

        public string Endpoint { get; set; }

        public string Region { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SecretId))
            {
                return "请求参数不完整";
            }
            if (string.IsNullOrEmpty(SecretKey))
            {
                return "请求参数不完整";
            }
            if (string.IsNullOrEmpty(Endpoint))
            {
                return "请求参数不完整";
            }
            if (string.IsNullOrEmpty(Region))
            {
                return "请求参数不完整";
            }
            return base.Validate();
        }
    }
}
