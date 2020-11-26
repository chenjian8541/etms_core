using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IDataAccess
{
    public interface ISysDataClearSmsCodeDAL
    {
        SysDataClearSmsCodeBucket GetSysDataClearSmsCode(int tenantId);

        SysDataClearSmsCodeBucket AddSysDataClearSmsCode(int tenantId, string smsCode);

        void RemoveSysDataClearSmsCode(int tenantId);
    }
}
