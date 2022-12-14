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
    public class ClassCategoryDAL : SimpleRepository<EtClassCategory, ClassCategoryBucket<EtClassCategory>>, IClassCategoryDAL
    {
        public ClassCategoryDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }
        public async Task<bool> AddClassCategory(EtClassCategory entity)
        {
            return await base.AddEntity(entity);
        }

        public async Task DelClassCategory(long id)
        {
            await base.DelEntity(id);
        }

        public async Task<List<EtClassCategory>> GetAllClassCategory()
        {
            return await base.GetAll();
        }
    }
}
