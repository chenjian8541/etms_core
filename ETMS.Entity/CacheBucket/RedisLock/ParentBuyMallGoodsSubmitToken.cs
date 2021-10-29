using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class ParentBuyMallGoodsSubmitToken : IRedisToken
    {
        public ParentBuyMallGoodsSubmitToken(int tenantId, long studentId)
        {
            this.TenantId = tenantId;
            this.StudentId = studentId;
        }

        public int TenantId { get; set; }

        public long StudentId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(60);

        public string GetKey()
        {
            return $"ParentBuyMallGoodsSubmitToken_{TenantId}_{StudentId}";
        }
    }
}
