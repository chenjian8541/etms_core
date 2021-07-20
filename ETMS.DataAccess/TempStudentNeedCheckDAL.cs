using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp.View;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess
{
    public class TempStudentNeedCheckDAL : DataAccessBase<TempStudentNeedCheckCountBucket>, ITempStudentNeedCheckDAL
    {
        public TempStudentNeedCheckDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TempStudentNeedCheckCountBucket> GetDb(params object[] keys)
        {
            var ot = Convert.ToDateTime(keys[1]);
            var otDesc = ot.EtmsToDateString();
            var needCheckStatistics = await _dbWrapper.ExecuteObject<StatisticsGroupCount>($"SELECT IsCheckIn as SType,COUNT(IsCheckIn) as SCount FROM EtTempStudentNeedCheck WHERE TenantId = {_tenantId} AND Ot = '{otDesc}' AND IsDeleted = {EmIsDeleted.Normal} GROUP BY IsCheckIn");
            var needCheckInCount = await _dbWrapper.ExecuteScalar($"SELECT COUNT(0) FROM EtTempStudentNeedCheck WHERE TenantId = {_tenantId} AND Ot = '{otDesc}' AND IsCheckIn = {EmBool.False} AND IsDeleted = {EmIsDeleted.Normal}");
            var needCheckOutCount = await _dbWrapper.ExecuteScalar($"SELECT COUNT(0) FROM EtTempStudentNeedCheck WHERE TenantId = {_tenantId} AND Ot = '{otDesc}' AND IsCheckIn = {EmBool.True} AND IsCheckOut = {EmBool.False} AND IsDeleted = {EmIsDeleted.Normal}");
            var needAttendClassCount = await _dbWrapper.ExecuteScalar($"SELECT COUNT(0) FROM EtTempStudentNeedCheckClass WHERE TenantId = {_tenantId} AND Ot = '{otDesc}' AND [Status] = {EmTempStudentNeedCheckClassStatus.NotAttendClass} AND IsDeleted = {EmIsDeleted.Normal}");
            var bucket = new TempStudentNeedCheckCountBucket()
            {
                NeedAttendClassCount = needAttendClassCount.ToInt(),
                NeedCheckInCount = needCheckInCount.ToInt(),
                NeedCheckOutCount = needCheckOutCount.ToInt(),
                FinishCheckInCount = 0,
                NeedCheckCount = 0
            };
            if (needCheckStatistics.Any())
            {
                bucket.NeedCheckCount = needCheckStatistics.Sum(p => p.SCount);
                var isFinishCheckIn = needCheckStatistics.FirstOrDefault(p => p.SType == EmBool.True);
                if (isFinishCheckIn != null)
                {
                    bucket.FinishCheckInCount = isFinishCheckIn.SCount;
                }
            }
            return bucket;
        }

        public async Task<TempStudentNeedCheckCountBucket> GetTempStudentNeedCheckCount(DateTime ot)
        {
            return await GetCache(_tenantId, ot);
        }

        public async Task<EtTempStudentNeedCheck> TempStudentNeedCheckGet(long id)
        {
            return await _dbWrapper.Find<EtTempStudentNeedCheck>(p => p.Id == id && p.TenantId == _tenantId);
        }
        public async Task<bool> TempStudentNeedCheckAdd(List<EtTempStudentNeedCheck> entitys)
        {
            await TempStudentNeedCheckDelOldHis();
            if (entitys != null && entitys.Count > 0)
            {
                _dbWrapper.InsertRange(entitys);
            }
            await UpdateCache(_tenantId, DateTime.Now);
            return true;
        }

        private async Task<bool> TempStudentNeedCheckDelOldHis()
        {
            try
            {
                var hisOt = DateTime.Now.AddDays(-5); //删除5天前的数据
                await _dbWrapper.Execute($"DELETE EtTempStudentNeedCheck WHERE TenantId = {_tenantId} AND Ot <= '{hisOt.EtmsToDateString()}' ");
                return true;
            }
            catch (Exception ex)
            {
                LOG.Log.Error("TempStudentNeedCheckDelOldHis", ex, this.GetType());
                return true;
            }
        }

        public async Task<bool> TempStudentNeedCheckSetIsCheckIn(long studentId, DateTime ot)
        {
            var log = await _dbWrapper.Find<EtTempStudentNeedCheck>(p => p.IsCheckIn == EmBool.True && p.TenantId == _tenantId && p.StudentId == studentId && p.Ot == ot.Date);
            if (log != null)
            {
                return true;
            }
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheck SET IsCheckIn = {EmBool.True},CheckInOt = '{ot.EtmsToString()}' WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND Ot = '{ot.EtmsToDateString()}'");
            await UpdateCache(_tenantId, ot);
            return true;
        }

        public async Task<bool> TempStudentNeedCheckSetIsCheckOut(long studentId, DateTime ot)
        {
            var log = await _dbWrapper.Find<EtTempStudentNeedCheck>(p => p.IsCheckOut == EmBool.True && p.TenantId == _tenantId && p.StudentId == studentId && p.Ot == ot.Date);
            if (log != null)
            {
                return true;
            }
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheck SET IsCheckOut = {EmBool.True},CheckOutOt = '{ot.EtmsToString()}' WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND Ot = '{ot.EtmsToDateString()}'");
            await UpdateCache(_tenantId, ot);
            return true;
        }

        public async Task<bool> TempStudentNeedCheckSetIsCheckInById(long id, DateTime ot)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheck SET IsCheckIn = {EmBool.True},CheckInOt = '{ot.EtmsToString()}' WHERE TenantId = {_tenantId} AND Id = {id} ");
            await UpdateCache(_tenantId, DateTime.Now);
            return true;
        }

        public async Task<bool> TempStudentNeedCheckSetIsCheckOutById(long id, DateTime ot)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheck SET IsCheckOut = {EmBool.True},CheckOutOt = '{ot.EtmsToString()}' WHERE TenantId = {_tenantId} AND Id = {id} ");
            await UpdateCache(_tenantId, DateTime.Now);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtTempStudentNeedCheck>, int>> TempStudentNeedCheckGetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtTempStudentNeedCheck>("EtTempStudentNeedCheck", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> TempStudentNeedCheckClassAdd(List<EtTempStudentNeedCheckClass> entitys)
        {
            await TempStudentNeedCheckClassDelOldHis();
            if (entitys != null && entitys.Count > 0)
            {
                _dbWrapper.InsertRange(entitys);
            }
            await UpdateCache(_tenantId, DateTime.Now);
            return true;
        }

        private async Task<bool> TempStudentNeedCheckClassDelOldHis()
        {
            try
            {
                var hisOt = DateTime.Now.AddDays(-5); //删除5天前的数据
                await _dbWrapper.Execute($"DELETE EtTempStudentNeedCheckClass WHERE TenantId = {_tenantId} AND Ot <= '{hisOt.EtmsToDateString()}' ");
                return true;
            }
            catch (Exception ex)
            {
                LOG.Log.Error("TempStudentNeedCheckClassDelOldHis", ex, this.GetType());
                return true;
            }
        }

        public async Task<bool> TempStudentNeedCheckClassSetIsAttendClass(long classTimesId)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheckClass SET [Status] = {EmTempStudentNeedCheckClassStatus.IsAttendClass} WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimesId} ");
            await UpdateCache(_tenantId, DateTime.Now);
            return true;
        }

        public async Task<bool> TempStudentNeedCheckClassSetIsAttendClassByStudentId(long studentId, DateTime date)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheckClass SET [Status] = {EmTempStudentNeedCheckClassStatus.IsAttendClass} WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND ClassOt = '{date.EtmsToDateString()}'");
            await UpdateCache(_tenantId, DateTime.Now);
            return true;
        }

        public async Task<bool> TempStudentNeedCheckClassSetIsAttendClass(long classTimesId, long studentId)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheckClass SET [Status] = {EmTempStudentNeedCheckClassStatus.IsAttendClass} WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimesId} AND StudentId = {studentId} ");
            await UpdateCache(_tenantId, DateTime.Now);
            return true;
        }

        public async Task<bool> TempStudentNeedCheckClassSetIsAttendClassById(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheckClass SET [Status] = {EmTempStudentNeedCheckClassStatus.IsAttendClass} WHERE TenantId = {_tenantId} AND Id = {id} ");
            await UpdateCache(_tenantId, DateTime.Now);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtTempStudentNeedCheckClass>, int>> TempStudentNeedCheckClassGetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtTempStudentNeedCheckClass>("EtTempStudentNeedCheckClass", "*", request.PageSize, request.PageCurrent, "StartTime ASC", request.ToString());
        }

        public async Task<EtTempStudentNeedCheckClass> TempStudentNeedCheckClassGet(long id)
        {
            return await _dbWrapper.Find<EtTempStudentNeedCheckClass>(p => p.Id == id && p.TenantId == _tenantId);
        }
    }
}
