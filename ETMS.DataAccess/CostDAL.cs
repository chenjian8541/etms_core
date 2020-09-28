using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class CostDAL : DataAccessBase<CostBucket>, ICostDAL
    {
        public CostDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<CostBucket> GetDb(params object[] keys)
        {
            var cost = await _dbWrapper.Find<EtCost>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (cost == null)
            {
                return null;
            }
            return new CostBucket()
            {
                Cost = cost
            };
        }

        public async Task<bool> ExistCost(string name, long id = 0)
        {
            var cost = await _dbWrapper.Find<EtCost>(p => p.TenantId == _tenantId && p.Name == name && p.Id != id && p.IsDeleted == EmIsDeleted.Normal);
            return cost != null;
        }

        public async Task<bool> AddCost(EtCost cost)
        {
            return await _dbWrapper.Insert(cost, async () => { await UpdateCache(_tenantId, cost.Id); });
        }

        public async Task<bool> EditCost(EtCost cost)
        {
            return await _dbWrapper.Update(cost, async () => { await UpdateCache(_tenantId, cost.Id); });
        }

        public async Task<EtCost> GetCost(long id)
        {
            var costBucket = await this.GetCache(_tenantId, id);
            return costBucket?.Cost;
        }

        public async Task<bool> DelCost(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtCost SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id}");
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtCost>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtCost>("EtCost", "*", request.PageSize, request.PageCurrent, "[Status] ASC,Id DESC", request.ToString());
        }

        public async Task<bool> AddSaleQuantity(long id, int count)
        {
            await _dbWrapper.Execute($"UPDATE EtCost SET SaleQuantity = SaleQuantity + {count} WHERE id = {id}");
            await UpdateCache(_tenantId, id);
            return true;
        }

        public async Task<bool> DeductioneSaleQuantity(long id, int count)
        {
            await _dbWrapper.Execute($"UPDATE EtCost SET SaleQuantity = SaleQuantity - {count} WHERE id = {id}");
            await UpdateCache(_tenantId, id);
            return true;
        }
    }
}
