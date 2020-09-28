using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class RecurringJobToken : IRedisToken
    {
        public RecurringJobToken(string jobName)
        {
            this.JobName = jobName;
        }

        public string JobName { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromMinutes(20);

        public int TenantId { get; set; } = 0;

        public string GetKey()
        {
            return $"RecurringJobToken_{JobName}";
        }
    }
}
