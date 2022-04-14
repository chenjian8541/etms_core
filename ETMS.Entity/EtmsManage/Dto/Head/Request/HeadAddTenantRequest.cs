using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.EtmsManage.Common;

namespace ETMS.Entity.EtmsManage.Dto.Head.Request
{
    public class HeadAddTenantRequest : AgentRequestBase
    {
        public int HeadId { get; set; }

        public int TenantId { get; set; }

        public override string Validate()
        {
            if (HeadId <= 0 || TenantId <= 0)
            {
                return "数据校验不合法";
            }
            return base.Validate();
        }
    }
}
