using ETMS.Entity.CacheBucket.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Alien
{
    public interface IMgTempDataCacheDAL
    {
        MgUserLoginOnlineBucket GetUserLoginOnlineBucket(int headId, long userId, int loginClientType);

        void SetUserLoginOnlineBucket(int headId, long userId, string loginTime, int loginClientType);
    }
}
