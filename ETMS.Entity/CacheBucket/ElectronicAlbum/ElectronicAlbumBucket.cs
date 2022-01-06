using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Entity.CacheBucket.ElectronicAlbum
{
    public class ElectronicAlbumBucket : ICacheDataContract
    {
        public EtElectronicAlbum ElectronicAlbum { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"ElectronicAlbumBucket_{parms[0]}_{parms[1]}";
        }
    }
}
