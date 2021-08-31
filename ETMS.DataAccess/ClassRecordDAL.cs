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

namespace ETMS.DataAccess
{
    public class ClassRecordDAL : DataAccessBase, IClassRecordDAL
    {
        public ClassRecordDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<long> AddEtClassRecord(EtClassRecord etClassRecord, List<EtClassRecordStudent> classRecordStudents)
        {
            await this._dbWrapper.Insert(etClassRecord);
            foreach (var s in classRecordStudents)
            {
                s.ClassRecordId = etClassRecord.Id;
            }
            this._dbWrapper.InsertRange(classRecordStudents);
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

        public async Task<Tuple<IEnumerable<EtClassRecord>, int>> GetPaging(RequestPagingBase request)
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

        public async Task<bool> EditClassRecordStudent(EtClassRecordStudent etClassRecordStudent)
        {
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
            var sql = $"SELECT TOP 1 0 FROM EtClassRecord WHERE TenantId = {_tenantId} AND ClassId = {classId} AND [Status] = {EmClassRecordStatus.Normal} AND ClassOt = '{classOt.EtmsToDateString()}' AND (StartTime BETWEEN '{startTime}' AND '{endTime}' OR EndTime BETWEEN '{startTime}' AND '{endTime}')";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj != null;
        }
    }
}
