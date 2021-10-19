using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class GoodsDAL : DataAccessBase<GoodsBucket>, IGoodsDAL
    {
        public GoodsDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<GoodsBucket> GetDb(params object[] keys)
        {
            var goods = await _dbWrapper.Find<EtGoods>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (goods == null)
            {
                return null;
            }
            return new GoodsBucket()
            {
                Goods = goods
            };
        }

        public async Task<bool> ExistGoods(string name, long id = 0)
        {
            var goods = await _dbWrapper.Find<EtGoods>(p => p.TenantId == _tenantId && p.Name == name && p.Id != id && p.IsDeleted == EmIsDeleted.Normal);
            return goods != null;
        }

        public async Task<bool> AddGoods(EtGoods goods)
        {
            return await _dbWrapper.Insert(goods, async () => { await UpdateCache(_tenantId, goods.Id); });
        }

        public async Task<bool> EditGoods(EtGoods goods)
        {
            return await _dbWrapper.Update(goods, async () => { await UpdateCache(_tenantId, goods.Id); });
        }

        public async Task<EtGoods> GetGoods(long id)
        {
            var goodsBucket = await this.GetCache(_tenantId, id);
            return goodsBucket?.Goods;
        }

        public async Task<bool> DelGoods(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtGoods SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id}");
            await _dbWrapper.Execute($"UPDATE EtMallGoods SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND ProductType = {EmProductType.Goods} AND RelatedId = {id}");
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtGoods>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtGoods>("EtGoods", "*", request.PageSize, request.PageCurrent, "[Status] ASC,Id DESC", request.ToString());
        }

        public async Task<bool> AddGoodsInventoryLog(EtGoodsInventoryLog log)
        {
            return await _dbWrapper.Insert(log);
        }

        public async Task<bool> AddInventoryQuantity(long goodId, int addCount)
        {
            await _dbWrapper.Execute($"UPDATE EtGoods SET InventoryQuantity = InventoryQuantity + {addCount} WHERE id = {goodId}");
            await UpdateCache(_tenantId, goodId);
            return true;
        }

        public async Task<bool> SubtractInventoryAndAddSaleQuantity(long goodId, int count)
        {
            await _dbWrapper.Execute($"UPDATE EtGoods SET InventoryQuantity = InventoryQuantity - {count},SaleQuantity = SaleQuantity + {count} WHERE id = {goodId}");
            await UpdateCache(_tenantId, goodId);
            return true;
        }

        public async Task<bool> AddInventoryAndDeductionSaleQuantity(long goodId, int count)
        {
            await _dbWrapper.Execute($"UPDATE EtGoods SET InventoryQuantity = InventoryQuantity + {count},SaleQuantity = SaleQuantity - {count} WHERE id = {goodId}");
            await UpdateCache(_tenantId, goodId);
            return true;
        }

        public async Task<Tuple<IEnumerable<GoodsInventoryLogView>, int>> GetGoodsInventoryLogPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<GoodsInventoryLogView>("GoodsInventoryLogView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
