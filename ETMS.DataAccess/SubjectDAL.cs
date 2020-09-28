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
    public class SubjectDAL : SimpleRepository<EtSubject, SubjectBucket<EtSubject>>, ISubjectDAL
    {
        public SubjectDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddSubject(EtSubject entity)
        {
            return await base.AddEntity(entity);
        }

        public async Task DelSubject(long id)
        {
            await base.DelEntity(id);
        }

        public async Task<List<EtSubject>> GetAllSubject()
        {
            return await base.GetAll();
        }
    }
}
