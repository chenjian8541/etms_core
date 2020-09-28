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
    public class GiftCategoryDAL : SimpleRepository<EtGiftCategory, GiftCategoryBucket<EtGiftCategory>>, IGiftCategoryDAL
    {
        public GiftCategoryDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddGiftCategory(EtGiftCategory entity)
        {
            return await this.AddEntity(entity);
        }

        public async Task DelGiftCategory(long id)
        {
            await this.DelEntity(id);
        }

        public async Task<List<EtGiftCategory>> GetAllGiftCategory()
        {
            return await this.GetAll();
        }
    }
}
