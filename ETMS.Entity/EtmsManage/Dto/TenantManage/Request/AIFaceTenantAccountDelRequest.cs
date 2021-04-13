using ETMS.Entity.EtmsManage.Common;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class AIFaceTenantAccountDelRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "请求数据不合法";
            }
            return base.Validate();
        }
    }
}