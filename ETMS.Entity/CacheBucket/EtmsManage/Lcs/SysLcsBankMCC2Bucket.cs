using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage.Lcs
{
    public class SysLcsBankMCC2Bucket : ICacheDataContract
    {
        public IEnumerable<LcsRegionsView> Datas { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// uni1Id
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"SysLcsBankMCC2Bucket_{parms[0]}";
        }
    }
}