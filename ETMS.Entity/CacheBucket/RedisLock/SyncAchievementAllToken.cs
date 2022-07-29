using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class SyncAchievementAllToken : IRedisToken
    {
        public SyncAchievementAllToken(int tenantId, long achievementId)
        {
            this.TenantId = tenantId;
            this.AchievementId = achievementId;
        }

        public int TenantId { get; set; }

        public long AchievementId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public string GetKey()
        {
            return $"SyncAchievementAllToken_{TenantId}_{AchievementId}";
        }

    }
}
