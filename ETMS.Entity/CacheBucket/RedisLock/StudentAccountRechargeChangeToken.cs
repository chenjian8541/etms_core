using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class StudentAccountRechargeChangeToken : IRedisToken
    {
        public StudentAccountRechargeChangeToken(int tenantId, long studentAccountRechargeId)
        {
            this.TenantId = tenantId;
            this.StudentAccountRechargeId = studentAccountRechargeId;
        }

        public int TenantId { get; set; }

        public long StudentAccountRechargeId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

        public string GetKey()
        {
            return $"StudentAccountRechargeChangeToken_{TenantId}_{StudentAccountRechargeId}";
        }
    }
}

