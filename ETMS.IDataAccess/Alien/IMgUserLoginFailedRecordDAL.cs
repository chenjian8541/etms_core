using ETMS.Entity.CacheBucket.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Alien
{
    public interface IMgUserLoginFailedRecordDAL
    {
        MgUserLoginFailedBucket GetUserLoginFailedRecord(string code, string phone);

        MgUserLoginFailedBucket AddUserLoginFailedRecord(string code, string phone);

        void RemoveUserLoginFailedRecord(string code, string phone);
    }
}
