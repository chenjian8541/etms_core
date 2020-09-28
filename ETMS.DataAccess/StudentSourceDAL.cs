using ETMS.DataAccess.Core;
using ETMS.DataAccess.Repository;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentSourceDAL : SimpleRepository<EtStudentSource, StudentSourceBucket<EtStudentSource>>, IStudentSourceDAL
    {
        public StudentSourceDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddStudentSource(EtStudentSource entity)
        {
            return await base.AddEntity(entity);
        }

        public async Task DelStudentSource(long id)
        {
            await base.DelEntity(id);
        }

        public async Task<List<EtStudentSource>> GetAllStudentSource()
        {
            return await base.GetAll();
        }
    }
}
