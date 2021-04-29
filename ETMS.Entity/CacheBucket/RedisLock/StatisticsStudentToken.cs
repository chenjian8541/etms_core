using ETMS.Entity.Config;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;


namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StatisticsStudentToken : IRedisToken
    {
        public StatisticsStudentToken(int tenantId, DateTime statisticsDate, EmStatisticsStudentType type)
        {
            this.TenantId = tenantId;
            this.StatisticsDate = statisticsDate;
            this.OpType = type;
        }

        public int TenantId { get; set; }

        public DateTime StatisticsDate { get; set; }

        public EmStatisticsStudentType OpType { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"StatisticsStudentToken_{TenantId}_{(int)OpType}_{StatisticsDate.ToString("yyyyMMdd")}";
        }
    }
}