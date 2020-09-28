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
using System.Linq;

namespace ETMS.DataAccess
{
    public class ClassSetDAL : SimpleRepository<EtClassSet, ClassSetBucket<EtClassSet>>, IClassSetDAL
    {
        public ClassSetDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ClassSetBucket<EtClassSet>> GetDb(params object[] keys)
        {
            var bucket = await base.GetDb();
            if (bucket != null && bucket.Entitys != null && bucket.Entitys.Any())
            {
                bucket.Entitys = bucket.Entitys.OrderBy(p => p.StartTime).ToList();
            }
            return bucket;
        }

        public async Task<bool> AddClassSet(EtClassSet entity)
        {
            return await base.AddEntity(entity);
        }

        public async Task DelClassSet(long id)
        {
            await base.DelEntity(id);
        }

        public async Task<List<EtClassSet>> GetAllClassSet()
        {
            return await base.GetAll();
        }
    }
}
