using ETMS.Entity.Config;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.CacheBucket.Mall
{
    public class MallPrepayBucket : ICacheDataContract
    {
        public TimeSpan TimeOut { get; } = TimeSpan.FromMinutes(10);

        public MallPrepayView MallCartView { get; set; }

        /// <summary>
        /// TenantId+LcsPayLogId
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"MallPrepayBucket_{parms[0]}_{parms[1]}";
        }
    }
}
