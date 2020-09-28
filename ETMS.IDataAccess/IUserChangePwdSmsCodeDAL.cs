using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IDataAccess
{
    public interface IUserChangePwdSmsCodeDAL
    {
        UserChangePwdSmsCodeBucket GetUserChangePwdSmsCode(int tenantId, long userId);

        UserChangePwdSmsCodeBucket AddUserChangePwdSmsCode(int tenantId, long userId, string smsCode);

        void RemoveUserChangePwdSmsCode(int tenantId, long userId);
    }
}
