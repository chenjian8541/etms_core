using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class SysUpgradeMsgPagingOutput
    {
        public int CId { get; set; }
        public int AgentId { get; set; }

        public string VersionNo { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string UpContent { get; set; }

        public byte Status { get; set; }

        public string StatusDesc { get; set; }
    }
}
