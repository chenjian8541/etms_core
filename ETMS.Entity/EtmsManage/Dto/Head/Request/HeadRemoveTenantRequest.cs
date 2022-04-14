using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.EtmsManage.Common;

namespace ETMS.Entity.EtmsManage.Dto.Head.Request
{
    public class HeadRemoveTenantRequest : AgentRequestBase
    {
        public int CId { get; set; }

        public string HeadName { get; set; }

        public string TenantName { get; set; }
        public override string Validate()
        {
            if (CId <= 0)
            {
                return "数据校验不合法";
            }
            return base.Validate();
        }
    }
}
