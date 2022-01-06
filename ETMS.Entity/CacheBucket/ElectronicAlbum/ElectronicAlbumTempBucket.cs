using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.ElectronicAlbum
{
    public class ElectronicAlbumTempBucket: ICacheDataContract
    {
        public EtElectronicAlbumTemp ElectronicAlbumTemp { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"ElectronicAlbumTempBucket_{parms[0]}_{parms[1]}";
        }
    }
}
