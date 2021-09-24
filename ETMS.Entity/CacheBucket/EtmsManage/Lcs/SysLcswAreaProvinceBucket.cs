using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage.Lcs
{
    public class SysLcswAreaProvinceBucket : ICacheDataContract
    {
        public IEnumerable<LcsRegionsView> Datas { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return "SysLcswAreaProvinceBucket";
        }
    }
}
