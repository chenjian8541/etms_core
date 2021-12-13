using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ISysPhoneSmsCodeDAL
    {
        SysPhoneSmsCodeBucket GetSysPhoneSmsCode(string phone);

        SysPhoneSmsCodeBucket AddSysPhoneSmsCode(string phone, string smsCode);

        void RemoveSysPhoneSmsCode(string phone);
    }
}
