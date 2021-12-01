using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantChangeMaxUserCountRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public int NewMaxUserCount { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "机构Id不能为空";
            }
            if (NewMaxUserCount < 0)
            {
                return "数量必须大于0";
            }
            return base.Validate();
        }
    }
}
