using ETMS.Entity.Config;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View.Alien;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.Alien
{
    public class MgOrganizationBucket : ICacheDataContract
    {
        public List<MgOrganizationView> MgOrganizationView { get; set; }

        public List<MgOrganization> AllOrganization { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromDays(BucketTimeOutConfig.ComTimeOutDay);

        /// <summary>
        /// HeadId
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"MgOrganizationBucket_{parms[0]}";
        }
    }
}
