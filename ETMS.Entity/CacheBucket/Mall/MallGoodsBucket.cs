using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.Mall
{
    public class MallGoodsBucket : ICacheDataContract
    {
        public EtMallGoods MallGoods { get; set; }

        public List<EtMallCoursePriceRule> MallCoursePriceRules { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        public string GetKeyFormat(params object[] parms)
        {
            return $"MallGoodsBucket_{parms[0]}_{parms[1]}";
        }
    }
}
