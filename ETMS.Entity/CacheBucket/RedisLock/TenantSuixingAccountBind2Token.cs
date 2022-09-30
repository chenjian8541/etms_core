using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class TenantSuixingAccountBind2Token : IRedisToken
    {
        public TenantSuixingAccountBind2Token(int tenantId)
        {
            this.TenantId = tenantId;
        }

        public int TenantId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(30);

        public string GetKey()
        {
            return $"TenantSuixingAccountBind2Token_{TenantId}";
        }
    }
}
