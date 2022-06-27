using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class ActivityGoToken : IRedisToken
    {
        public ActivityGoToken(long miniPgmUserId)
        {
            this.MiniPgmUserId = miniPgmUserId;
        }

        public int TenantId { get; set; }

        public long MiniPgmUserId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public string GetKey()
        {
            return $"ActivityGoToken_{MiniPgmUserId}";
        }
    }
}
