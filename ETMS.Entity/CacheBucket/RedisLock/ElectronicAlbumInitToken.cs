using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.RedisLock
{
    public class ElectronicAlbumInitToken : IRedisToken
    {
        public ElectronicAlbumInitToken(int tenantId, long electronicAlbumId)
        {
            this.TenantId = tenantId;
            this.ElectronicAlbumId = electronicAlbumId;
        }

        public int TenantId { get; set; }

        public long ElectronicAlbumId { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(30);

        public string GetKey()
        {
            return $"ElectronicAlbumInitToken_{TenantId}_{ElectronicAlbumId}";
        }
    }
}
