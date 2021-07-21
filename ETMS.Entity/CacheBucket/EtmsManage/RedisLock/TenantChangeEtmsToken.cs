using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.EtmsManage.RedisLock
{
    public class TenantChangeEtmsToken : IRedisToken
    {
        public TenantChangeEtmsToken(int agentId, int tenantId)
        {
            this.AgentId = agentId;
            this.TenantId = tenantId;
        }

        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"SYS_TenantChangeEtmsToken_{AgentId}_{TenantId}";
        }
    }
}
