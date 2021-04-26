using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class SuitDAL : DataAccessBase<SuitBucket>, ISuitDAL
    {
        public SuitDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<SuitBucket> GetDb(params object[] keys)
        {
            var suit = await _dbWrapper.Find<EtSuit>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (suit == null)
            {
                return null;
            }
            var suitDetail = await _dbWrapper.FindList<EtSuitDetail>(p => p.TenantId == _tenantId && p.SuitId == suit.Id && p.IsDeleted == EmIsDeleted.Normal);
            return new SuitBucket()
            {
                Suit = suit,
                SuitDetails = suitDetail
            };
        }

        public async Task<bool> ExistSuit(string name, long id = 0)
        {
            var suit = await _dbWrapper.Find<EtSuit>(p => p.TenantId == _tenantId && p.Name == name && p.Id != id && p.IsDeleted == EmIsDeleted.Normal);
            return suit != null;
        }

        public async Task<bool> AddSuit(EtSuit suit, List<EtSuitDetail> suitDetails)
        {
            await _dbWrapper.Insert(suit);
            if (suitDetails != null && suitDetails.Any())
            {
                foreach (var s in suitDetails)
                {
                    s.SuitId = suit.Id;
                }
                _dbWrapper.InsertRange(suitDetails);
            }
            await base.UpdateCache(_tenantId, suit.Id);
            return true;
        }

        public async Task<bool> EditSuit(EtSuit suit, List<EtSuitDetail> suitDetails)
        {
            await _dbWrapper.Update(suit);
            await _dbWrapper.Execute($"DELETE EtSuitDetail WHERE SuitId = {suit.Id}");
            if (suitDetails != null && suitDetails.Any())
            {
                _dbWrapper.InsertRange(suitDetails);
            }
            await base.UpdateCache(_tenantId, suit.Id);
            return true;
        }

        public async Task<bool> EditSuit(EtSuit suit)
        {
            await _dbWrapper.Update(suit);
            await base.UpdateCache(_tenantId, suit.Id);
            return true;
        }

        public async Task<Tuple<EtSuit, List<EtSuitDetail>>> GetSuit(long id)
        {
            var suitBucket = await base.GetCache(_tenantId, id);
            if (suitBucket == null)
            {
                return null;
            }
            return Tuple.Create(suitBucket.Suit, suitBucket.SuitDetails);
        }

        public async Task<bool> DelSuit(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtSuit SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id} ; DELETE EtSuitDetail WHERE SuitId = {id}");
            base.RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtSuit>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtSuit>("EtSuit", "*", request.PageSize, request.PageCurrent, "[Status] ASC,Id DESC", request.ToString());
        }

        public async Task<IEnumerable<SuitExistProduct>> GetProductSuitUsed(byte productType, long productId)
        {
            var sql = $"SELECT TOP 5 Id,Name FROM EtSuitDetail WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ProductType = {productType} AND ProductId = {productId}";
            return await _dbWrapper.ExecuteObject<SuitExistProduct>(sql);
        }

        public async Task<List<long>> GetCoursePriceRuleUsed(long courseId)
        {
            var sql = $"SELECT CoursePriceRuleId AS Id FROM EtSuitDetail WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ProductId = {courseId} AND ProductType = {EmProductType.Course} GROUP BY CoursePriceRuleId";
            var result = await _dbWrapper.ExecuteObject<OnlyId>(sql);
            return result.Select(p => p.Id).ToList();
        }
    }
}
