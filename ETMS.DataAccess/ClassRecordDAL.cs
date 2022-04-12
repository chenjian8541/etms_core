using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp.View;
using ETMS.Entity.View;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Utility;
using ETMS.Entity.View.OnlyOneFiled;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Event.DataContract;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.DataAccess
{
    public class ClassRecordDAL : DataAccessBase, IClassRecordDAL
    {
        private readonly ISysTenantMqScheduleDAL _sysTenantMqScheduleDAL;

        public ClassRecordDAL(IDbWrapper dbWrapper, ISysTenantMqScheduleDAL sysTenantMqScheduleDAL) : base(dbWrapper)
        {
            this._sysTenantMqScheduleDAL = sysTenantMqScheduleDAL;
        }

        public async Task<long> AddEtClassRecord(EtClassRecord etClassRecord, List<EtClassRecordStudent> classRecordStudents,
            List<EtClassRecordEvaluateStudent> evaluateStudents = null)
        {
            await this._dbWrapper.Insert(etClassRecord);
            foreach (var s in classRecordStudents)
            {
                s.ClassRecordId = etClassRecord.Id;
            }
            this._dbWrapper.InsertRange(classRecordStudents);

            if (evaluateStudents != null && evaluateStudents.Count > 0) //课后点评
            {
                foreach (var item in evaluateStudents)
                {
                    item.ClassRecordId = etClassRecord.Id;
                    var myClassRecordStudent = classRecordStudents.FirstOrDefault(p => p.StudentId == item.StudentId);
                    if (myClassRecordStudent != null)
                    {
                        item.ClassRecordStudentId = myClassRecordStudent.Id;
                    }
                }
                this._dbWrapper.InsertRange(evaluateStudents);
            }

            await _sysTenantMqScheduleDAL.AddSysTenantMqSchedule(new SyncStudentLogOfSurplusCourseEvent(_tenantId)
            {
                Type = SyncStudentLogOfSurplusCourseEventType.ClassRecordStudent,
                Logs = classRecordStudents.Select(j => new SyncStudentLogOfSurplusCourseView()
                {
                    Id = j.Id,
                    CourseId = j.CourseId,
                    StudentId = j.StudentId
                })
            }, _tenantId, EmSysTenantMqScheduleType.SyncStudentLogOfSurplusCourse, TimeSpan.FromMinutes(1));
            return etClassRecord.Id;
        }

        public async Task<EtClassRecordAbsenceLog> GetRelatedAbsenceLog(long studentId, long courseId)
        {
            return await _dbWrapper.Find<EtClassRecordAbsenceLog>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.StudentId == studentId && p.CourseId == courseId && p.Status == EmClassRecordStatus.Normal
            && p.HandleStatus != EmClassRecordAbsenceHandleStatus.MarkFinish);
        }

        public void AddClassRecordAbsenceLog(List<EtClassRecordAbsenceLog> classRecordAbsenceLogs)
        {
            this._dbWrapper.InsertRange(classRecordAbsenceLogs);
        }

        public void AddClassRecordPointsApplyLog(List<EtClassRecordPointsApplyLog> classRecordPointsApplyLog)
        {
            _dbWrapper.InsertRange(classRecordPointsApplyLog);
        }

        public async Task<Tuple<IEnumerable<EtClassRecord>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtClassRecord>("EtClassRecord", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtClassRecord> GetClassRecord(long classRecordId)
        {
            return await _dbWrapper.Find<EtClassRecord>(classRecordId);
        }

        public async Task<List<EtClassRecordStudent>> GetClassRecordStudents(long classRecordId)
        {
            return await _dbWrapper.FindList<EtClassRecordStudent>(p => p.ClassRecordId == classRecordId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<Tuple<IEnumerable<ClassRecordAbsenceLogView>, int>> GetClassRecordAbsenceLogPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<ClassRecordAbsenceLogView>("ClassRecordAbsenceLogView", "*", request.PageSize, request.PageCurrent, "[HandleStatus] ASC,Id DESC", request.ToString());
        }

        public async Task<EtClassRecordAbsenceLog> GetClassRecordAbsenceLog(long id)
        {
            return await _dbWrapper.Find<EtClassRecordAbsenceLog>(id);
        }

        public async Task<List<EtClassRecordAbsenceLog>> GetClassRecordAbsenceLogByClassRecordId(long classRecordId)
        {
            return await _dbWrapper.FindList<EtClassRecordAbsenceLog>(p => p.TenantId == _tenantId && p.ClassRecordId == classRecordId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> UpdateClassRecordAbsenceLog(EtClassRecordAbsenceLog log)
        {
            return await _dbWrapper.Update(log);
        }

        public async Task<EtClassRecordPointsApplyLog> GetClassRecordPointsApplyLog(long id)
        {
            return await _dbWrapper.Find<EtClassRecordPointsApplyLog>(id);
        }

        public async Task<bool> EditClassRecordPointsApplyLog(EtClassRecordPointsApplyLog log)
        {
            return await _dbWrapper.Update(log);
        }

        public async Task<Tuple<IEnumerable<EtClassRecordStudent>, int>> GetClassRecordStudentPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtClassRecordStudent>("EtClassRecordStudent", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtClassRecordStudent> GetEtClassRecordStudentById(long id)
        {
            return await _dbWrapper.Find<EtClassRecordStudent>(id);
        }

        public async Task<bool> EditClassRecord(EtClassRecord classRecord)
        {
            await _dbWrapper.Update(classRecord);
            return true;
        }

        public async Task<bool> EditClassRecordStudent(EtClassRecordStudent etClassRecordStudent, bool isChangeDeClassTime = false)
        {
            if (isChangeDeClassTime)
            {
                if (etClassRecordStudent.CheckOt.Date != DateTime.Now.Date)
                {
                    etClassRecordStudent.SurplusCourseDesc = string.Empty;
                }
                else
                {
                    await _sysTenantMqScheduleDAL.AddSysTenantMqSchedule(new SyncStudentLogOfSurplusCourseEvent(_tenantId)
                    {
                        Type = SyncStudentLogOfSurplusCourseEventType.ClassRecordStudent,
                        Logs = new List<SyncStudentLogOfSurplusCourseView>() { new SyncStudentLogOfSurplusCourseView() {
                        Id = etClassRecordStudent.Id,
                        CourseId = etClassRecordStudent.CourseId,
                        StudentId = etClassRecordStudent.StudentId
                    } }
                    }, _tenantId, EmSysTenantMqScheduleType.SyncStudentLogOfSurplusCourse, TimeSpan.FromMinutes(1));
                }
            }
            await _dbWrapper.Update(etClassRecordStudent);
            return true;
        }

        public async Task<Tuple<IEnumerable<ClassRecordPointsApplyLogView>, int>> GetClassRecordPointsApplyLog(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<ClassRecordPointsApplyLogView>("ClassRecordPointsApplyLogView", "*", request.PageSize, request.PageCurrent, "[HandleStatus] ASC,Id DESC", request.ToString());
        }

        public async Task<List<EtClassRecordPointsApplyLog>> GetClassRecordPointsApplyLogByClassRecordId(long classRecordId)
        {
            return await _dbWrapper.FindList<EtClassRecordPointsApplyLog>(p => p.TenantId == _tenantId && p.ClassRecordId == classRecordId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> SetClassRecordRevoke(long classRecordId)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtClassRecord SET [Status] = {EmClassRecordStatus.Revoked} WHERE TenantId = {_tenantId} AND id = {classRecordId};");
            sql.Append($"UPDATE EtClassRecordStudent SET [Status] = {EmClassRecordStatus.Revoked} WHERE TenantId = {_tenantId} AND ClassRecordId = {classRecordId};");
            sql.Append($"UPDATE EtClassRecordEvaluateTeacher SET [Status] = {EmClassRecordStatus.Revoked} WHERE TenantId = {_tenantId} AND ClassRecordId = {classRecordId};");
            sql.Append($"UPDATE EtClassRecordEvaluateStudent SET [Status] = {EmClassRecordStatus.Revoked} WHERE TenantId = {_tenantId} AND ClassRecordId = {classRecordId};");
            sql.Append($"UPDATE EtClassRecordPointsApplyLog SET [Status] = {EmClassRecordStatus.Revoked} WHERE TenantId = {_tenantId} AND ClassRecordId = {classRecordId};");
            sql.Append($"UPDATE EtClassRecordAbsenceLog SET [Status] = {EmClassRecordStatus.Revoked} WHERE TenantId = {_tenantId} AND ClassRecordId = {classRecordId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClassRecordAddEvaluateStudentCount(long classRecordId, int addCount)
        {
            await this._dbWrapper.Execute($"UPDATE EtClassRecord SET EvaluateStudentCount = EvaluateStudentCount + {addCount} WHERE id = {classRecordId} ");
            return true;
        }

        public async Task<bool> AddClassRecordOperationLog(EtClassRecordOperationLog log)
        {
            await _dbWrapper.Insert(log);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtClassRecordOperationLog>, int>> GetClassRecordOperationLogPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtClassRecordOperationLog>("EtClassRecordOperationLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtClassRecordPointsApplyLog> GetClassRecordPointsApplyLogByClassRecordId(long classRecordId, long studentId)
        {
            return await _dbWrapper.Find<EtClassRecordPointsApplyLog>(p => p.TenantId == _tenantId && p.ClassRecordId == classRecordId && p.StudentId == studentId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<ClassRecordStatistics> GetClassRecordStatistics(long classId)
        {
            var log = await this._dbWrapper.ExecuteObject<ClassRecordStatistics>($"SELECT ISNULL(SUM(ClassTimes),0) AS TotalFinishClassTimes,COUNT(Id) AS TotalFinishCount FROM EtClassRecord WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassRecordStatus.Normal} AND ClassId = {classId} ");
            return log.FirstOrDefault();
        }

        public async Task<bool> ClassRecordStudentDeEvaluateCount(long classRecordStudentId, int deCount)
        {
            await this._dbWrapper.Execute($"UPDATE EtClassRecordStudent SET EvaluateCount = EvaluateCount - {deCount} WHERE Id = {classRecordStudentId}");
            return true;
        }

        public async Task<List<EtClassRecord>> GetClassRecord(DateTime classOt)
        {
            return await this._dbWrapper.FindList<EtClassRecord>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.ClassOt == classOt && p.Status == EmClassRecordStatus.Normal);
        }

        public async Task<bool> ExistClassRecord(long classId, DateTime classOt, int startTime, int endTime)
        {
            var sql = $"SELECT TOP 1 0 FROM EtClassRecord WHERE TenantId = {_tenantId} AND ClassId = {classId} AND [Status] = {EmClassRecordStatus.Normal} AND ClassOt = '{classOt.EtmsToDateString()}' AND ((StartTime >= '{startTime}' AND StartTime < '{endTime}') OR (EndTime > '{startTime}' AND EndTime <= '{endTime}') OR (StartTime < '{startTime}' AND EndTime > '{endTime}'))";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj != null;
        }

        public async Task<ClassRecordTeacherStatistics> GetClassRecordTeacherStatistics(long teacherId, DateTime startDate, DateTime endDate)
        {
            var sql = $"SELECT SUM(ClassTimes) AS TotalClassTimes,COUNT(0) AS TotalCount FROM EtClassRecord WHERE TenantId = {_tenantId} AND IsDeleted = {EmClassRecordStatus.Normal} AND [Status] = {EmClassRecordStatus.Normal} AND Teachers LIKE '%{teacherId}%' AND ClassOt >= '{startDate.EtmsToDateString()}' AND ClassOt < '{endDate.EtmsToDateString()}'";
            var data = await _dbWrapper.ExecuteObject<ClassRecordTeacherStatistics>(sql);
            return data.FirstOrDefault();
        }

        /// <summary>
        /// 学员期间各课程的请假次数
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="courseIds"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StudentCourseIsLeaveCountView>> GetClassRecordStudentCourseIsLeaveCount(long studentId, DateTime startDate, DateTime endDate)
        {
            var strSql = new StringBuilder();
            strSql.Append($"SELECT CourseId,COUNT(CourseId) AS TotalCount FROM EtClassRecordStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassRecordStatus.Normal} AND StudentId = {studentId} AND StudentCheckStatus = {EmClassStudentCheckStatus.Leave} AND ClassOt >= '{startDate.EtmsToDateString()}' AND ClassOt <= '{endDate.EtmsToDateString()}' GROUP BY CourseId");
            return await _dbWrapper.ExecuteObject<StudentCourseIsLeaveCountView>(strSql.ToString());
        }

        /// <summary>
        /// 获取超上课时未处理的记录
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public async Task<List<EtClassRecordStudent>> ClassRecordStudentHasUntreatedExceed(long studentId, long courseId)
        {
            return await _dbWrapper.FindList<EtClassRecordStudent>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.StudentId == studentId && p.CourseId == courseId && p.ExceedClassTimes > 0 && p.IsExceedProcessed == EmBool.False
            && p.Status == EmClassRecordStatus.Normal);
        }

        public async Task UpdateClassRecordStudentIsExceedProcessed(long studentId, long courseId)
        {
            await _dbWrapper.Execute($"UPDATE EtClassRecordStudent SET IsExceedProcessed = {EmBool.True} WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND StudentId = {studentId} AND CourseId = {courseId} AND IsExceedProcessed = {EmBool.False}");
        }

        public async Task ClassRecordAddDeSum(long id, decimal addDeSum)
        {
            await _dbWrapper.Execute($"UPDATE EtClassRecord SET DeSum = DeSum + {addDeSum} WHERE Id  = {id} AND TenantId = {_tenantId}");
        }

        public async Task SyncClassCategoryId(long classId, long? classCategoryId)
        {
            if (classCategoryId == null)
            {
                await _dbWrapper.Execute($"UPDATE EtClassRecord SET ClassCategoryId = NULL WHERE TenantId = {_tenantId} AND ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal}");
            }
            else
            {
                await _dbWrapper.Execute($"UPDATE EtClassRecord SET ClassCategoryId = {classCategoryId.Value} WHERE TenantId = {_tenantId} AND ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal}");
            }
        }

        public async Task UpdateClassRecordStudentSurplusCourseDesc(List<UpdateStudentLogOfSurplusCourseView> upLogs)
        {
            if (upLogs.Count == 1)
            {
                var myLog = upLogs.First();
                await _dbWrapper.Execute($"UPDATE EtClassRecordStudent SET SurplusCourseDesc = '{myLog.SurplusCourseDesc}' WHERE Id = {myLog.Id}");
                return;
            }
            if (upLogs.Count <= 50)
            {
                var sql = new StringBuilder();
                foreach (var p in upLogs)
                {
                    sql.Append($"UPDATE EtClassRecordStudent SET SurplusCourseDesc = '{p.SurplusCourseDesc}' WHERE Id = {p.Id} ;");
                }
                await _dbWrapper.Execute(sql.ToString());
            }
            else
            {
                foreach (var p in upLogs)
                {
                    await _dbWrapper.Execute($"UPDATE EtClassRecordStudent SET SurplusCourseDesc = '{p.SurplusCourseDesc}' WHERE Id = {p.Id}");
                }
            }
        }
    }
}
