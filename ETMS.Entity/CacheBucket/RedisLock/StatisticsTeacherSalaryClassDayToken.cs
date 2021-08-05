using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StatisticsTeacherSalaryClassDayToken : IRedisToken
    {
        public StatisticsTeacherSalaryClassDayToken(int tenantId, DateTime time)
        {
            this.TenantId = tenantId;
            this.Time = time;
        }

        public int TenantId { get; set; }

        public DateTime Time { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public string GetKey()
        {
            return $"StatisticsTeacherSalaryClassDayToken_{TenantId}_{Time.ToString("yyyyMMdd")}";
        }
    }
}
