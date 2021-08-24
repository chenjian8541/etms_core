using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StatisticsTeacherSalaryMonthToken : IRedisToken
    {
        public StatisticsTeacherSalaryMonthToken(int tenantId)
        {
            this.TenantId = tenantId;
        }

        public int TenantId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"StatisticsTeacherSalaryMonthToken_{TenantId}";
        }
    }
}
