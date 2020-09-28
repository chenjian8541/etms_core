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

namespace ETMS.DataAccess
{
    public class StudentCourseStopLogDAL : DataAccessBase<StudentCourseStopLogBucket>, IStudentCourseStopLogDAL
    {
        public StudentCourseStopLogDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected async override Task<StudentCourseStopLogBucket> GetDb(params object[] keys)
        {
            var logs = await _dbWrapper.FindList<EtStudentCourseStopLog>(p => p.TenantId == _tenantId && p.StudentId == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (logs.Any())
            {
                logs = logs.OrderByDescending(p => p.Id).ToList();
            }
            return new StudentCourseStopLogBucket()
            {
                StudentCourseStopLogs = logs
            };
        }

        public async Task<bool> AddStudentCourseStopLog(EtStudentCourseStopLog log)
        {
            await this._dbWrapper.Insert(log);
            await base.UpdateCache(_tenantId, log.StudentId);
            return true;
        }

        public async Task<List<EtStudentCourseStopLog>> GetStudentCourseStopLog(long studentId)
        {
            var bucket = await GetCache(_tenantId, studentId);
            return bucket?.StudentCourseStopLogs;
        }

        public async Task<bool> StudentCourseRestore(long studentId, long courseId, DateTime restoreTime)
        {
            var strrestoreTime = restoreTime.EtmsToDateString();
            var sql = $"UPDATE EtStudentCourseStopLog SET RestoreTime = '{strrestoreTime}',StopDay = DATEDIFF(dd,StopTime,'{strrestoreTime}')  WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId} AND IsDeleted = {EmIsDeleted.Normal} AND RestoreTime IS NULL";
            await _dbWrapper.Execute(sql);
            await base.UpdateCache(_tenantId, studentId);
            return true;
        }
    }
}
