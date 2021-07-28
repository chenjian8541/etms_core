using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket
{
    public class ParentMenusConfigBucket : ICacheDataContract
    {
        public List<ParentMenuConfigOutput> ParentMenus { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"ParentMenusConfigBucket_{parms[0]}";
        }
    }
}

