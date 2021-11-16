using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class HandleParentBuyMallGoodsSubmitToken : IRedisToken
    {
        public HandleParentBuyMallGoodsSubmitToken(int tenantId, long lcsPayLogId)
        {
            this.TenantId = tenantId;
            this.LcsPayLogId = lcsPayLogId;
        }

        public int TenantId { get; set; }

        public long LcsPayLogId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromMinutes(10);

        public string GetKey()
        {
            return $"HandleParentBuyMallGoodsSubmitToken_{TenantId}_{LcsPayLogId}";
        }
    }
}
