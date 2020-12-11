using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IDataAccess
{
    public interface ISysSafeSmsCodeDAL
    {
        SysSafeSmsCodeBucket GetSysSafeSmsCode(int tenantId);

        SysSafeSmsCodeBucket AddSysSafeSmsCode(int tenantId, string smsCode);

        void RemoveSysSafeSmsCode(int tenantId);
    }
}
