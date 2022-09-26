using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Common;
using ETMS.Entity.Temp;
using ETMS.Entity.View;

namespace ETMS.DataAccess
{
    public class StudentCheckOnLogDAL : DataAccessBase<StudentCheckOnLastTimeBucket>, IStudentCheckOnLogDAL
    {
        public StudentCheckOnLogDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StudentCheckOnLastTimeBucket> GetDb(params object[] keys)
        {
            var studentId = keys[1].ToLong();
            var logs = await _dbWrapper.ExecuteObject<EtStudentCheckOnLog>(
                $"SELECT TOP 1 * FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal} ORDER BY CheckOt DESC");
            var myLog = logs.FirstOrDefault();
            if (myLog == null)
            {
                return null;
            }
            return new StudentCheckOnLastTimeBucket()
            {
                StudentCheckOnLog = myLog
            };
        }

        public async Task<EtStudentCheckOnLog> GetStudentCheckOnLog(long id)
        {
            return await _dbWrapper.Find<EtStudentCheckOnLog>(p => p.Id == id);
        }

        public async Task<long> AddStudentCheckOnLog(EtStudentCheckOnLog entity)
        {
            entity.CheckOtDate = entity.CheckOt.Date;
            await _dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.StudentId);
            return entity.Id;
        }

        public async Task<bool> EditStudentCheckOnLog(EtStudentCheckOnLog entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.StudentId);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtStudentCheckOnLog>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtStudentCheckOnLog>("EtStudentCheckOnLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<StudentCheckOnLogView>, int>> GetViewPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<StudentCheckOnLogView>("StudentCheckOnLogView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtStudentCheckOnLog> GetStudentCheckOnLastTime(long studentId)
        {
            var bucket = await GetCache(_tenantId, studentId);
            return bucket?.StudentCheckOnLog;
        }

        public async Task<List<EtStudentCheckOnLog>> GetStudentCheckOnLogByClassTimesId(long classTimesId)
        {
            return await this._dbWrapper.FindList<EtStudentCheckOnLog>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.ClassTimesId == classTimesId);
        }

        public async Task<EtStudentCheckOnLog> GetStudentDeLog(long classTimesId, long studentId)
        {
            return await this._dbWrapper.Find<EtStudentCheckOnLog>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.ClassTimesId == classTimesId && p.StudentId == studentId && p.Status != EmStudentCheckOnLogStatus.Revoke);
        }

        public async Task<IEnumerable<EtStudentCheckOnLog>> GetStudentDeLog(List<long> classTimesIds, long studentId)
        {
            var sql = string.Empty;
            if (classTimesIds.Count == 1)
            {
                sql = $"SELECT * FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ClassTimesId = {classTimesIds[0]} AND StudentId = {studentId} AND [Status] != {EmStudentCheckOnLogStatus.Revoke} ";
            }
            else
            {
                sql = $"SELECT * FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ClassTimesId IN ({string.Join(',', classTimesIds)}) AND StudentId = {studentId} AND [Status] != {EmStudentCheckOnLogStatus.Revoke} ";
            }
            return await this._dbWrapper.ExecuteObject<EtStudentCheckOnLog>(sql);
        }

        public async Task<bool> UpdateStudentCheckOnIsBeRollcall(long classTimesId)
        {
            await _dbWrapper.Execute($"update [EtStudentCheckOnLog] set [Status] = {EmStudentCheckOnLogStatus.BeRollcall} where TenantId = {_tenantId} and ClassTimesId = {classTimesId} ");
            return true;
        }

        public async Task<bool> RevokeCheckSign(long classTimesId)
        {
            await _dbWrapper.Execute($"UPDATE [EtStudentCheckOnLog] SET [Status] = {EmStudentCheckOnLogStatus.Revoke},ClassTimesId = null,ClassId = null,CourseId =null,ClassOtDesc='',DeType = {EmDeClassTimesType.NotDe},DeClassTimes = 0,DeSum = 0,ExceedClassTimes =0,DeStudentCourseDetailId = null,Remark='撤销点名' WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimesId} ");
            return true;
        }

        public async Task<int> GetStudentOneDayAttendClassCount(long studentId, DateTime date)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT COUNT(0) FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND StudentId = {studentId} AND [Status] IN ({EmStudentCheckOnLogStatus.NormalAttendClass},{EmStudentCheckOnLogStatus.BeRollcall}) AND CheckOtDate = '{date.EtmsToDateString()}'");
            return obj.ToInt();
        }

        public async Task<IEnumerable<OnlyId>> GetOneDayStudentCheckInAllClassTimes(DateTime date)
        {
            var sql = $"SELECT ClassTimesId AS Id FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND [Status] = {EmStudentCheckOnLogStatus.NormalAttendClass} AND [IsDeleted] = {EmIsDeleted.Normal} AND ClassTimesId IS NOT NULL AND CheckOtDate = '{date.EtmsToDateString()}' GROUP BY ClassTimesId";
            return await _dbWrapper.ExecuteObject<OnlyId>(sql);
        }

        public async Task<DateTime?> GetStudentLastGoClassTime(long studentId)
        {
            var sql = $"SELECT TOP 1 CheckOt FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmStudentCheckOnLogStatus.NormalAttendClass} ORDER BY CheckOt DESC ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            if (obj == null)
            {
                return null;
            }
            return Convert.ToDateTime(obj);
        }

        public async Task<int> GetStatisticsCheckInCount(DateTime date)
        {
            var sql = $"SELECT COUNT(0) FROM (SELECT COUNT(StudentId) as MyStudentId FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND CheckOtDate = '{date.EtmsToDateString()}' AND [CheckType] = {EmStudentCheckOnLogCheckType.CheckIn} GROUP BY StudentId) tb ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            if (obj == null)
            {
                return 0;
            }
            return obj.ToInt();
        }

        public async Task<int> GetStatisticsCheckOutCount(DateTime date)
        {
            var sql = $"SELECT COUNT(0) FROM (SELECT COUNT(StudentId) as MyStudentId FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND CheckOtDate = '{date.EtmsToDateString()}' AND [CheckType] = {EmStudentCheckOnLogCheckType.CheckOut} GROUP BY StudentId) tb ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            if (obj == null)
            {
                return 0;
            }
            return obj.ToInt();
        }

        public async Task<int> GetStatisticsCheckAttendClassCount(DateTime date)
        {
            var sql = $"SELECT COUNT(0) FROM (SELECT COUNT(StudentId) as MyStudentId FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND CheckOtDate = '{date.EtmsToDateString()}' AND [CheckType] = {EmStudentCheckOnLogCheckType.CheckIn} AND [Status] IN (1,2) GROUP BY StudentId) tb ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            if (obj == null)
            {
                return 0;
            }
            return obj.ToInt();
        }

        public async Task SaveStudentCheckOnStatistics(EtStudentCheckOnStatistics entity)
        {
            var hisLog = await _dbWrapper.Find<EtStudentCheckOnStatistics>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal &&
            p.Ot == entity.Ot);
            if (hisLog == null)
            {
                await _dbWrapper.Insert(entity);
            }
            else
            {
                hisLog.CheckInCount = entity.CheckInCount;
                hisLog.CheckOutCount = entity.CheckOutCount;
                hisLog.CheckAttendClassCount = entity.CheckAttendClassCount;
                await _dbWrapper.Update(hisLog);
            }
        }

        public async Task<Tuple<IEnumerable<EtStudentCheckOnStatistics>, int>> GetPagingStatistics(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtStudentCheckOnStatistics>("EtStudentCheckOnStatistics", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<StudentCheckOnDeInfoView> GetStudentCheckOnDeInfo(DateTime ot)
        {
            var obj = await _dbWrapper.ExecuteObject<StudentCheckOnDeInfoView>(
                $"select ISNULL(SUM(DeSum),0) as TotalDeSum,ISNULL(SUM(DeClassTimes),0) as TotalDeClassTimes from EtStudentCheckOnLog where TenantId = {_tenantId} and IsDeleted = {EmIsDeleted.Normal} and CheckType = {EmStudentCheckOnLogCheckType.CheckIn} and CheckOtDate = '{ot.EtmsToDateString()}' and [Status] = {EmStudentCheckOnLogStatus.NormalAttendClass} AND DeClassTimes > 0");

            return obj.FirstOrDefault();
        }
    }
}
