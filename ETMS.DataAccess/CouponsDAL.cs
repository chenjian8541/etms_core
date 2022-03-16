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
    public class CouponsDAL : DataAccessBase<CouponsBucket>, ICouponsDAL
    {
        public CouponsDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<CouponsBucket> GetDb(params object[] keys)
        {
            var coupons = await _dbWrapper.Find<EtCoupons>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (coupons == null)
            {
                return null;
            }
            return new CouponsBucket()
            {
                Coupons = coupons
            };
        }

        public async Task<bool> AddCoupons(EtCoupons coupons)
        {
            return await _dbWrapper.Insert(coupons, async () => { await UpdateCache(_tenantId, coupons.Id); });
        }

        public async Task<bool> EditCoupons(EtCoupons coupons)
        {
            return await _dbWrapper.Update(coupons, async () => { await UpdateCache(_tenantId, coupons.Id); });
        }

        public async Task<EtCoupons> GetCoupons(long id)
        {
            var couponsBucket = await this.GetCache(_tenantId, id);
            return couponsBucket?.Coupons;
        }

        public async Task<bool> DelCoupons(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtCoupons SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id}");
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtCoupons>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtCoupons>("EtCoupons", "*", request.PageSize, request.PageCurrent, "[Status] ASC,Id DESC", request.ToString());
        }

        public async Task<int> StudentTodayGetCount(long studentId, long couponsId)
        {
            var todayStart = DateTime.Now.Date.EtmsToString();
            var todayEnd = DateTime.Now.AddDays(1).Date.EtmsToString();
            var result = await _dbWrapper.ExecuteScalar(
                $"SELECT COUNT(0) FROM EtCouponsStudentGet WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CouponsId = {couponsId} AND IsDeleted = {EmIsDeleted.Normal} AND GetTime > '{todayStart}' AND GetTime < '{todayEnd}'");
            return result.ToInt();
        }

        public async Task<int> StudentGetCount(long studentId, long couponsId)
        {
            var result = await _dbWrapper.ExecuteScalar(
                $"SELECT COUNT(0) FROM EtCouponsStudentGet WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CouponsId = {couponsId} AND IsDeleted = {EmIsDeleted.Normal} ");
            return result.ToInt();
        }

        public async Task<bool> AddCouponsStudentGet(EtCouponsStudentGet etCouponsStudentGet)
        {
            return await _dbWrapper.Insert(etCouponsStudentGet);
        }

        public void AddCouponsStudentGet(List<EtCouponsStudentGet> etCouponsStudentGets)
        {
            _dbWrapper.InsertRange(etCouponsStudentGets);
        }

        public async Task<bool> AddCouponsGetCount(long couponsId, int addCount)
        {
            await _dbWrapper.Execute($"UPDATE EtCoupons SET GetCount = GetCount + {addCount} WHERE id = {couponsId}");
            await UpdateCache(_tenantId, couponsId);
            return true;
        }

        public async Task<bool> AddCouponsUseCount(long couponsId, int addCount)
        {
            await _dbWrapper.Execute($"UPDATE EtCoupons SET UsedCount = UsedCount + {addCount} WHERE id = {couponsId}");
            await UpdateCache(_tenantId, couponsId);
            return true;
        }

        public async Task<Tuple<IEnumerable<CouponsStudentGetView>, int>> CouponsStudentGetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<CouponsStudentGetView>("CouponsStudentGetView", "*", request.PageSize, request.PageCurrent, "[Status] ASC,Id DESC", request.ToString());
        }

        public async Task<EtCouponsStudentGet> CouponsStudentGet(long id)
        {
            return await _dbWrapper.Find<EtCouponsStudentGet>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> ChangeCouponsStudentGetStatus(long id, byte newStatus)
        {
            var count = await _dbWrapper.Execute($"UPDATE EtCouponsStudentGet SET [Status] = {newStatus} WHERE id = {id}");
            return count > 0;
        }

        public async Task DelCouponsStudentGet(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtCouponsStudentGet SET [IsDeleted] = {EmIsDeleted.Deleted} WHERE id = {id}");
        }

        public async Task<bool> AddCouponsStudentUse(EtCouponsStudentUse etCouponsStudentUse)
        {
            return await _dbWrapper.Insert(etCouponsStudentUse);
        }

        public async Task<Tuple<IEnumerable<CouponsStudentUseView>, int>> CouponsStudentUsePaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<CouponsStudentUseView>("CouponsStudentUseView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<IEnumerable<CouponsStudentGetView>> GetCouponsCanUse(long studentId)
        {
            var now = DateTime.Now.EtmsToDateString();
            var sql = $"select top 50 * from CouponsStudentGetView where TenantId = {_tenantId} AND [Status] = {EmCouponsStudentStatus.Unused} AND studentId = {studentId} and (LimitUseTime <= '{now}' or LimitUseTime is null) and (ExpiredTime > '{now}' or ExpiredTime is null) and IsDeleted = {EmIsDeleted.Normal}";
            return await _dbWrapper.ExecuteObject<CouponsStudentGetView>(sql);
        }

        public async Task<List<EtCouponsStudentGet>> GetCouponsStudentGet(string generateNo)
        {
            return await _dbWrapper.FindList<EtCouponsStudentGet>(p => p.TenantId == _tenantId && p.GenerateNo == generateNo && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<IEnumerable<EtCouponsStudentGet>> GetCouponsStudentGetToExpire(DateTime minTime, DateTime maxTime)
        {
            return await _dbWrapper.ExecuteObject<EtCouponsStudentGet>($"select top 1000 * from EtCouponsStudentGet where TenantId = {_tenantId} and IsDeleted = {EmIsDeleted.Normal} and [Status] = {EmCouponsStudentStatus.Unused} and IsRemindExpired = {EmBool.False} and ExpiredTime >= '{minTime.EtmsToDateString()}' and ExpiredTime <= '{maxTime.EtmsToDateString()}'");
        }
    }
}
