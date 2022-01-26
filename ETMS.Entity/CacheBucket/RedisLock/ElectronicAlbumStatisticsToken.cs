using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class ElectronicAlbumStatisticsToken : IRedisToken
    {
        public ElectronicAlbumStatisticsToken(int tenantId, long id)
        {
            this.TenantId = tenantId;
            this.ElectronicAlbumDetailId = id;
        }

        public int TenantId { get; set; }

        public long ElectronicAlbumDetailId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(30);

        public string GetKey()
        {
            return $"ElectronicAlbumStatisticsToken_{TenantId}_{ElectronicAlbumDetailId}";
        }
    }
}
