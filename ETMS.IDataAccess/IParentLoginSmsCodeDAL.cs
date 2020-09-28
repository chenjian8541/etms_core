using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IDataAccess
{
    public interface IParentLoginSmsCodeDAL
    {
        ParentLoginSmsCodeBucket GetParentLoginSmsCode(string code, string phone);

        ParentLoginSmsCodeBucket AddParentLoginSmsCode(string code, string phone, string smsCode);

        void RemoveParentLoginSmsCode(string code, string phone);
    }
}
