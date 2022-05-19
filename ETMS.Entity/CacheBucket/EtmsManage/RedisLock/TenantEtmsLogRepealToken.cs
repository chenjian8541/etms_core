using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.EtmsManage.RedisLock
{
    public class TenantEtmsLogRepealToken : IRedisToken
    {
        public TenantEtmsLogRepealToken(int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }

        public int TenantId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(30);

        public string GetKey()
        {
            return $"TenantEtmsLogRepealToken_{Id}";
        }
    }
}
