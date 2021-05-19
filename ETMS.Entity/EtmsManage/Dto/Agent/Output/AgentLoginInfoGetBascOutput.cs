using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class AgentLoginInfoGetBascOutput
    {
        public int AgentId { get; set; }

        public string TagKey { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string IdCard { get; set; }

        public string Address { get; set; }

        public int EtmsSmsCount { get; set; }

        public string Code { get; set; }

        public List<AgentEtmsAccountOutput> AgentEtmsAccounts { get; set; }
    }
    public class AgentEtmsAccountOutput
    {
        public int VersionId { get; set; }

        public string VersionName { get; set; }

        public int EtmsCount { get; set; }
    }
}
