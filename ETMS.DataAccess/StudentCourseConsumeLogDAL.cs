﻿using ETMS.DataAccess.Core;
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
            }, _tenantId, EmSysTenantMqScheduleType.SyncStudentLogOfSurplusCourse, TimeSpan.FromMinutes(2)).Wait();
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
            }, _tenantId, EmSysTenantMqScheduleType.SyncStudentLogOfSurplusCourse, TimeSpan.FromMinutes(2)).Wait();
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
    }
}
