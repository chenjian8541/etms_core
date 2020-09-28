using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IUserLoginFailedRecordDAL
    {
        UserLoginFailedBucket GetUserLoginFailedRecord(string code, string phone);

        UserLoginFailedBucket AddUserLoginFailedRecord(string code, string phone);

        void RemoveUserLoginFailedRecord(string code, string phone);
    }
}
