using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.Head.Request
{
    public class HeadGetRequest : AgentRequestBase
    {
        public int CId { get; set; }

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

