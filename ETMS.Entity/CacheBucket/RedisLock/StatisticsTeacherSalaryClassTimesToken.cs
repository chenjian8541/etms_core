using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StatisticsTeacherSalaryClassTimesToken : IRedisToken
    {
        public StatisticsTeacherSalaryClassTimesToken(int tenantId, long classRecordId)
        {
            this.TenantId = tenantId;
            this.ClassRecordId = classRecordId;
        }

        public int TenantId { get; set; }

        public long ClassRecordId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public string GetKey()
        {
            return $"StatisticsTeacherSalaryClassTimesToken_{TenantId}_{ClassRecordId}";
        }
    }
}
