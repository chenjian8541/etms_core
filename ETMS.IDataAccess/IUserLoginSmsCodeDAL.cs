using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IDataAccess
{
    public interface IUserLoginSmsCodeDAL
    {
        UserLoginSmsCodeBucket GetUserLoginSmsCode(string code, string phone);

        UserLoginSmsCodeBucket AddUserLoginSmsCode(string code, string phone,string smsCode);

        void RemoveUserLoginSmsCode(string code, string phone);
    }
}
