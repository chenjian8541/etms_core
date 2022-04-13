using ETMS.Entity.Config;
using ETMS.Entity.Dto.User.Output;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Entity.CacheBucket.TempShortTime
{
    public class UserTenantEntrancePCBucket : ICacheDataContract
    {
        public TimeSpan TimeOut { get; } = TimeSpan.FromMinutes(5);

        public long LoginTimestamp { get; set; }

        public UserLoginOutput MyUserLoginOutput { get; set; }

        /// <summary>
        /// 机构Id+用户Id
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"UserTenantEntrancePCBucket_{parms[0]}_{parms[1]}";
        }
    }
}
