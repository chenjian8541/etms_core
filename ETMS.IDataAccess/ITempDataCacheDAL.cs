using ETMS.Entity.CacheBucket.TempShortTime;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITempDataCacheDAL
    {
        NoticeStudentsOfClassDayBucket GetNoticeStudentsOfClassDayBucket(int tenantId, DateTime classOt);

        void SetNoticeStudentsOfClassDayBucket(int tenantId, DateTime classOt);
    }
}
