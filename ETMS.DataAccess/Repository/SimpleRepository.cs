using ETMS.DataAccess.Core;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess.Repository
{
    /// <summary>
    /// 以_tenantId为缓存参数，并只提供增、删、查功能
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TBucket"></typeparam>
    public abstract class SimpleRepository<TEntity, TBucket> : DataAccessBase<TBucket>
        where TEntity : Entity<long>
        where TBucket : class, ICacheDataContract, ICacheSimpleDataContract<TEntity>, new()
    {
        public SimpleRepository(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TBucket> GetDb(params object[] keys)
        {
            var myData = await _dbWrapper.FindList<TEntity>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            return new TBucket()
            {
                Entitys = myData
            };
        }

        public virtual async Task<bool> AddEntity(TEntity entity)
        {
            return await _dbWrapper.Insert(entity, async () => { await UpdateCache(_tenantId); });
        }

        public virtual async Task DelEntity(long id)
        {
            var buckey = await GetCache(_tenantId);
            var entity = buckey.Entitys.FirstOrDefault(p => p.Id == id);
            if (entity == null)
            {
                return;
            }
            entity.IsDeleted = EmIsDeleted.Deleted;
            await _dbWrapper.Update(entity, () => { RemoveCache(_tenantId); });
        }

        public virtual async Task<List<TEntity>> GetAll()
        {
            var bucket = await GetCache(_tenantId);
            return bucket.Entitys;
        }
    }
}
