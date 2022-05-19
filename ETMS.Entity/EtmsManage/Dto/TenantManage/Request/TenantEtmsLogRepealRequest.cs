using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantEtmsLogRepealRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public string Remark { get; set; }
        public override string Validate()
        {
            if (Id <= 0)
            {
                return "请选择授权记录";
            }
            return base.Validate();
        }
    }
}
