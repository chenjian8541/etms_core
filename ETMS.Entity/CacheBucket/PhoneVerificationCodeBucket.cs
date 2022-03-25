using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.User.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public class PhoneVerificationCodeBucket : ICacheDataContract
    {
        public string Phone { get; set; }

        public string VerificationCode { get; set; }

        public TimeSpan TimeOut { get; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 手机号码
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public string GetKeyFormat(params object[] parms)
        {
            return $"PhoneVerificationCodeBucket_{parms[0]}";
        }
    }
}
