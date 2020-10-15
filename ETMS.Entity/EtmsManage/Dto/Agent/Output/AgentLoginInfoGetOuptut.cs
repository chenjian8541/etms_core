using ETMS.Entity.Config.Router;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class AgentLoginInfoGetOuptut
    {
        public int AgentId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public List<RouteConfig> RouteConfigs { get; set; }
    }
}
