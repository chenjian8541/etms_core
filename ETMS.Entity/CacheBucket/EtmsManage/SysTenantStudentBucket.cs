using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket.EtmsManage
{
    public class SysTenantStudentBucket : ICacheDataContract
    {
        public List<SysTenantStudent> SysTenantStudents { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromHours(BucketTimeOutConfig.SysTenantPeopleOutHour);

        /// <summary>
        /// phone
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"SysTenantStudentBucket_{parms[0]}";
        }
    }
}