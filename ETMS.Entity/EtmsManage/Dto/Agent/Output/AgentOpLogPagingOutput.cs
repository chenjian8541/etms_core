using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class AgentOpLogPagingOutput
    {
        public int CId { get; set; }
        public int AgentId { get; set; }

        /// <summary>
        ///  操作类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmSysAgentOpLogType"/>
        /// </summary>
        public int Type { get; set; }

        public string IpAddress { get; set; }

        public string OpContent { get; set; }

        public DateTime Ot { get; set; }

        public string AgentName { get; set; }

        public string AgentPhone { get; set; }

        public string TypeDesc { get; set; }
    }
}
