using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;


namespace ETMS.Entity.CacheBucket.EtmsManage.RedisLock
{
    public class TenantAddToken : IRedisToken
    {
        public TenantAddToken(int agentId)
        {
            this.AgentId = agentId;
        }

        public int TenantId { get; set; }

        public int AgentId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"SYS_TenantAddToken_{AgentId}";
        }
    }
}
