using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
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
    public class TenantToDoThingDAL : DataAccessBase<TenantToDoThingBucket>, ITenantToDoThingDAL
    {
        private readonly ITenantConfigDAL _tenantConfigDAL;

        public TenantToDoThingDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider, ITenantConfigDAL tenantConfigDAL) : base(dbWrapper, cacheProvider)
        {
            this._tenantConfigDAL = tenantConfigDAL;
        }

        protected override async Task<TenantToDoThingBucket> GetDb(params object[] keys)
        {
            this._tenantConfigDAL.InitTenantId(_tenantId);
            var config = await _tenantConfigDAL.GetTenantConfig();
            var bucket = new TenantToDoThingBucket()
            {
                StudentCourseNotEnough = await GetStudentCourseNotEnough(config.StudentCourseRenewalConfig.LimitClassTimes, config.StudentCourseRenewalConfig.LimitDay),
                StudentOrderArrears = await GetStudentOrderArrears(),
                ClassNotScheduled = await GetClassNotScheduled(),
                ClassTimesTimeOutNotCheckSign = await GetClassTimesTimeOutNotCheckSign(),
                GoodsInventoryNotEnough = await GetGoodsInventoryNotEnough(),
                ClassRecordAbsent = await GetClassRecordAbsent(),
                StudentLeaveApplyLogCount = await GetStudentLeaveApplyLogCount(),
                TryCalssApplyLogCount = await GetTryCalssApplyLogCount()
            };
            return bucket;
        }

        private async Task<int> GetStudentCourseNotEnough(int limitClassTimes, int limitDay)
        {
            var sql = $"SELECT COUNT(0) FROM StudentCourseView WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND BuyQuantity > 0 AND StudentType = {EmStudentType.ReadingStudent} AND ((DeType={EmDeClassTimesType.ClassTimes} AND SurplusQuantity <= {limitClassTimes}) OR (DeType<>{EmDeClassTimesType.ClassTimes} AND SurplusQuantity=0 AND SurplusSmallQuantity <={limitDay}))";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        private async Task<int> GetStudentOrderArrears()
        {
            var sql = $"SELECT COUNT(0) FROM EtOrder WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} and ArrearsSum > 0 AND [Status] <> {EmOrderStatus.Repeal} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        private async Task<int> GetClassNotScheduled()
        {
            var sql = $"SELECT COUNT(0) FROM EtClass WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ScheduleStatus = {EmClassScheduleStatus.Unscheduled} AND CompleteStatus = {EmClassCompleteStatus.UnComplete} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        private async Task<int> GetClassTimesTimeOutNotCheckSign()
        {
            var thisWeekDate = EtmsHelper.GetWeekStartEndDate(DateTime.Now);
            var sql = $"SELECT COUNT(0) FROM EtClassTimes WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassTimesStatus.UnRollcall} AND ClassOt >= '{thisWeekDate.Item1.EtmsToDateString()}' AND ClassOt <= '{thisWeekDate.Item2.EtmsToDateString()}'";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        private async Task<int> GetGoodsInventoryNotEnough()
        {
            var sql = $"SELECT COUNT(0) FROM  EtGoods WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmGoodsStatus.Enabled} AND LimitQuantity IS NOT NULL AND InventoryQuantity <= LimitQuantity ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        private async Task<int> GetClassRecordAbsent()
        {
            var sql = $"SELECT COUNT(0) FROM EtClassRecordAbsenceLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassRecordStatus.Normal} AND [HandleStatus] = {EmClassRecordAbsenceHandleStatus.Unprocessed} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<int> GetStudentLeaveApplyLogCount()
        {
            var sql = $"SELECT COUNT(0) AS TotalCount  FROM EtStudentLeaveApplyLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [HandleStatus] = {EmStudentLeaveApplyHandleStatus.Unreviewed} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<int> GetTryCalssApplyLogCount()
        {
            var sql = $"SELECT COUNT(0) AS TotalCount  FROM EtTryCalssApplyLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [HandleStatus] = {EmTryCalssApplyHandleStatus.Unreviewed} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<bool> ResetTenantToDoThing()
        {
            await UpdateCache(_tenantId);
            return true;
        }

        public async Task<TenantToDoThingBucket> GetTenantToDoThing()
        {
            return await GetCache(_tenantId);
        }
    }
}
