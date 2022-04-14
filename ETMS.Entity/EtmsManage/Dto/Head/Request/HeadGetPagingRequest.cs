using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.Head.Request
{
    public class HeadGetPagingRequest : AgentPagingBase
    {
        public string Name { get; set; }

        public override bool IsNeedLimitUserData()
        {
            return true;
        }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet());
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND [Name] LIKE '%{Name}%'");
            }
            return condition.ToString();
        }
    }
}
