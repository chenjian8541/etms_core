using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Event.DataContract;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.View;
using ETMS.Entity.Enum;
using ETMS.Utility;

namespace ETMS.DataAccess
{
    public class StudentCourseConsumeLogDAL : DataAccessBase, IStudentCourseConsumeLogDAL
    {
        private readonly ISysTenantMqScheduleDAL _sysTenantMqScheduleDAL;

        public StudentCourseConsumeLogDAL(IDbWrapper dbWrapper, ISysTenantMqScheduleDAL sysTenantMqScheduleDAL) : base(dbWrapper)
        {
            this._sysTenantMqScheduleDAL = sysTenantMqScheduleDAL;
        }

        public void AddStudentCourseConsumeLog(List<EtStudentCourseConsumeLog> studentCourseConsumeLogs)
        {
            _dbWrapper.InsertRange(studentCourseConsumeLogs);
            _sysTenantMqScheduleDAL.AddSysTenantMqSchedule(new SyncStudentLogOfSurplusCourseEvent(_tenantId)
            {
                Type = SyncStudentLogOfSurplusCourseEventType.StudentCourseConsumeLog,
                Logs = studentCourseConsumeLogs.Select(j => new SyncStudentLogOfSurplusCourseView()
                {
                    Id = j.Id,
                    CourseId = j.CourseId,
                    StudentId = j.StudentId
                })
            }, _tenantId, EmSysTenantMqScheduleType.SyncStudentLogOfSurplusCourse, TimeSpan.FromMinutes(1)).Wait();
        }

        public async Task AddStudentCourseConsumeLog(EtStudentCourseConsumeLog studentCourseConsumeLogs)
        {
            await _dbWrapper.Insert(studentCourseConsumeLogs);
            _sysTenantMqScheduleDAL.AddSysTenantMqSchedule(new SyncStudentLogOfSurplusCourseEvent(_tenantId)
            {
                Type = SyncStudentLogOfSurplusCourseEventType.StudentCourseConsumeLog,
                Logs = new List<SyncStudentLogOfSurplusCourseView>(){ new SyncStudentLogOfSurplusCourseView()
                {
                    Id = studentCourseConsumeLogs.Id,
                    CourseId = studentCourseConsumeLogs.CourseId,
                    StudentId = studentCourseConsumeLogs.StudentId
                } }
            }, _tenantId, EmSysTenantMqScheduleType.SyncStudentLogOfSurplusCourse, TimeSpan.FromMinutes(1)).Wait();
        }

        public async Task<Tuple<IEnumerable<EtStudentCourseConsumeLog>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStudentCourseConsumeLog>("EtStudentCourseConsumeLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task UpdateStudentCourseConsumeLogSurplusCourseDesc(List<UpdateStudentLogOfSurplusCourseView> upLogs)
        {
            if (upLogs.Count == 1)
            {
                var myLog = upLogs.First();
                await _dbWrapper.Execute($"UPDATE EtStudentCourseConsumeLog SET SurplusCourseDesc = '{myLog.SurplusCourseDesc}' WHERE Id = {myLog.Id}");
                return;
            }
            if (upLogs.Count <= 50)
            {
                var sql = new StringBuilder();
                foreach (var p in upLogs)
                {
                    sql.Append($"UPDATE EtStudentCourseConsumeLog SET SurplusCourseDesc = '{p.SurplusCourseDesc}' WHERE Id = {p.Id} ;");
                }
                await _dbWrapper.Execute(sql.ToString());
            }
            else
            {
                foreach (var p in upLogs)
                {
                    await _dbWrapper.Execute($"UPDATE EtStudentCourseConsumeLog SET SurplusCourseDesc = '{p.SurplusCourseDesc}' WHERE Id = {p.Id}");
                }
            }
        }

        public async Task<DateTime?> GetLastConsumeTime(long studentId, long coueseId)
        {
            var sql = $"SELECT TOP 1 Ot FROM EtStudentCourseConsumeLog WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {coueseId} AND IsDeleted = {EmIsDeleted.Normal} AND SourceType NOT IN (5,6,7,8,11) ORDER BY Ot DESC";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            if (obj == null)
            {
                return null;
            }
            return Convert.ToDateTime(obj);
        }

        public async Task<CourseConsumeLogDeInfoView> GetDirectDeClassTimesDeSumInfo(DateTime ot)
        {
            var sql = $"SELECT ISNULL(SUM(DeSum),0) as TotalDeSum,ISNULL(SUM(DeClassTimes),0) as TotalDeClassTimes FROM EtStudentCourseConsumeLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND SourceType IN (14,15) AND Ot = '{ot.EtmsToDateString()}'";
            var log = await _dbWrapper.ExecuteObject<CourseConsumeLogDeInfoView>(sql);
            return log.FirstOrDefault();
        }
    }
}
