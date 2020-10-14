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
    public class GiftDAL : DataAccessBase<GiftBucket>, IGiftDAL
    {
        public GiftDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<GiftBucket> GetDb(params object[] keys)
        {
            var gift = await _dbWrapper.Find<EtGift>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (gift == null)
            {
                return null;
            }
            return new GiftBucket()
            {
                Gift = gift
            };
        }

        public async Task<bool> ExistGift(string name, long id = 0)
        {
            var gift = await _dbWrapper.Find<EtGift>(p => p.TenantId == _tenantId && p.Name == name && p.Id != id && p.IsDeleted == EmIsDeleted.Normal);
            return gift != null;
        }
        public async Task<bool> AddGift(EtGift gift)
        {
            return await _dbWrapper.Insert(gift, async () => { await UpdateCache(_tenantId, gift.Id); });
        }

        public async Task<bool> EditGift(EtGift gift)
        {
            return await _dbWrapper.Update(gift, async () => { await UpdateCache(_tenantId, gift.Id); });
        }

        public async Task<EtGift> GetGift(long id)
        {
            var giftBucket = await this.GetCache(_tenantId, id);
            return giftBucket?.Gift;
        }

        public async Task<bool> DelGift(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtGift SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id}");
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtGift>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtGift>("EtGift", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> IsUserCanNotBeDelete(long id)
        {
            var log = await _dbWrapper.Find<EtGiftExchangeLogDetail>(p => p.GiftId == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            return log != null;
        }

        public async Task<bool> AddGiftExchange(EtGiftExchangeLog giftExchangeLog, List<EtGiftExchangeLogDetail> giftExchangeLogDetails)
        {
            LOG.Log.Info(Newtonsoft.Json.JsonConvert.SerializeObject(giftExchangeLog), this.GetType());
            await _dbWrapper.Insert(giftExchangeLog);
            foreach (var detail in giftExchangeLogDetails)
            {
                detail.GiftExchangeLogId = giftExchangeLog.Id;
            }
            _dbWrapper.InsertRange(giftExchangeLogDetails);
            return true;
        }

        public async Task<int> GetStudentExchangeNums(long studentId, long giftId)
        {
            var exCount = await _dbWrapper.ExecuteScalar($"SELECT ISNULL(SUM([Count]),0) FROM EtGiftExchangeLogDetail WHERE StudentId = {studentId} AND GiftId = {giftId} ");
            return exCount.ToInt();
        }

        public async Task<bool> DeductionNums(long giftId, int deNums)
        {
            await _dbWrapper.Execute($"UPDATE EtGift SET Nums = Nums - {deNums} WHERE id = {giftId}");
            await UpdateCache(_tenantId, giftId);
            return true;
        }

        public async Task<Tuple<IEnumerable<GiftExchangeLogView>, int>> GetExchangeLogPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<GiftExchangeLogView>("GiftExchangeLogView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<GiftExchangeLogDetailView>, int>> GetExchangeLogDetailPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<GiftExchangeLogDetailView>("GiftExchangeLogDetailView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<IEnumerable<GiftExchangeLogDetailView>> GetExchangeLogDetail(long giftExchangeLogId)
        {
            return await _dbWrapper.ExecuteObject<GiftExchangeLogDetailView>($"SELECT * FROM GiftExchangeLogDetailView WHERE GiftExchangeLogId = {giftExchangeLogId} AND IsDeleted = {EmIsDeleted.Normal}");
        }

        public async Task<bool> UpdateExchangeLogNewStatus(long giftExchangeLogId, byte newStatus)
        {
            var count = await _dbWrapper.Execute($"UPDATE EtGiftExchangeLog SET [Status] = {newStatus} WHERE Id = {giftExchangeLogId};UPDATE EtGiftExchangeLogDetail SET [Status] = {newStatus} WHERE GiftExchangeLogId = {giftExchangeLogId};");
            return count > 0;
        }
    }
}
