using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.MallGoods
{
    public class MallCartDAL : DataAccessBase<MallCartBucket>, IMallCartDAL
    {
        public MallCartDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<MallCartBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var mallCart = await _dbWrapper.Find<EtMallCart>(p => p.Id == id && p.TenantId == _tenantId
            && p.IsDeleted == EmIsDeleted.Normal);
            if (mallCart == null)
            {
                return null;
            }
            var view = JsonConvert.DeserializeObject<MallCartView>(mallCart.CartContent);
            return new MallCartBucket()
            {
                MallCartView = view
            };
        }

        public async Task<long> AddMallCart(EtMallCart entity)
        {
            await _dbWrapper.Insert(entity);
            return entity.Id;
        }

        public async Task<MallCartView> GetMallCart(long id)
        {
            var bucket = await GetCache(_tenantId, id);
            return bucket?.MallCartView;
        }
    }
}
