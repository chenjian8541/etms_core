using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class AgentGetOutput
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string TagKey { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string IdCard { get; set; }

        public string Address { get; set; }

        public bool IsLock { get; set; }

        public string Remark { get; set; }

        public string KefuQQ { get; set; }

        public string KefuPhone { get; set; }
    }
}
