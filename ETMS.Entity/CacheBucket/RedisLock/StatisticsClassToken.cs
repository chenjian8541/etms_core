﻿using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StatisticsClassToken : IRedisToken
    {
        public StatisticsClassToken(int tenantId, DateTime statisticsDate)
        {
            this.TenantId = tenantId;
            this.StatisticsDate = statisticsDate;
        }

        public int TenantId { get; set; }

        public DateTime StatisticsDate { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"StatisticsClassToken_{TenantId}_{StatisticsDate.ToString("yyyyMMdd")}";
        }
    }
}
