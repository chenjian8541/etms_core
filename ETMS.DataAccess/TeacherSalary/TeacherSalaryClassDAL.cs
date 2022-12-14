using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.IDataAccess.TeacherSalary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Utility;
using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Entity.View;

namespace ETMS.DataAccess.TeacherSalary
{
    public class TeacherSalaryClassDAL : DataAccessBase, ITeacherSalaryClassDAL
    {
        public TeacherSalaryClassDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task<bool> DelTeacherSalaryClassTimes(long classRecordId)
        {
            await _dbWrapper.Execute($"DELETE EtTeacherSalaryClassTimes WHERE TenantId = {_tenantId} AND ClassRecordId = {classRecordId} ");
            return true;
        }

        public async Task<bool> SaveTeacherSalaryClassTimes(long classRecordId, List<EtTeacherSalaryClassTimes> entitys)
        {
            await DelTeacherSalaryClassTimes(classRecordId);
            if (entitys.Count == 1)
            {
                await _dbWrapper.Insert(entitys.First());
            }
            else
            {
                _dbWrapper.InsertRange(entitys);
            }
            return true;
        }

        public async Task<bool> DelTeacherSalaryClassDay(DateTime ot)
        {
            await _dbWrapper.Execute($"DELETE EtTeacherSalaryClassDay WHERE TenantId = {_tenantId} AND Ot = '{ot.EtmsToDateString()}' ");
            return true;
        }

        public async Task<bool> SaveTeacherSalaryClassDay(DateTime ot, List<EtTeacherSalaryClassDay> entitys)
        {
            await DelTeacherSalaryClassDay(ot);
            if (entitys.Count == 1)
            {
                await _dbWrapper.Insert(entitys.First());
            }
            else
            {
                _dbWrapper.InsertRange(entitys);
            }
            return true;
        }

        public async Task<IEnumerable<EtTeacherSalaryClassTimes>> GetTeacherSalaryClassTimes(List<long> teacherIds, DateTime startOt, DateTime endOt)
        {
            if (teacherIds == null || teacherIds.Count == 0)
            {
                return null;
            }
            var str = new StringBuilder($"SELECT * FROM EtTeacherSalaryClassTimes WHERE TenantId = {_tenantId} AND  IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startOt.EtmsToDateString()}' AND Ot <= '{endOt.EtmsToDateString()}'");
            if (teacherIds.Count == 1)
            {
                str.Append($" AND TeacherId = {teacherIds[0]}");
            }
            else
            {
                str.Append($" AND TeacherId IN ({string.Join(',', teacherIds)})");
            }
            return await _dbWrapper.ExecuteObject<EtTeacherSalaryClassTimes>(str.ToString());
        }

        public async Task<Tuple<IEnumerable<TeacherSalaryClassDayView>, int>> GetTeacherSalaryClassDayPaging(IPagingRequest request)
        {
            var table = $"(SELECT TeacherId,ClassId,SUM(ArrivedAndBeLateCount) AS TotalArrivedAndBeLateCount,SUM(ArrivedCount) AS TotalArrivedCount,SUM(BeLateCount) AS TotalBeLateCount,SUM(DeSum) AS TotalDeSum,SUM(LeaveCount) AS TotalLeaveCount,SUM(MakeUpStudentCount) AS TotalMakeUpStudentCount,SUM(NotArrivedCount) AS TotalNotArrivedCount,SUM(StudentClassTimes) AS TotalStudentClassTimes,SUM(TeacherClassTimes) AS TotalTeacherClassTimes,SUM(TryCalssStudentCount) AS TotalTryCalssStudentCount FROM EtTeacherSalaryClassDay WHERE {request} group by TeacherId,ClassId) TB";
            return await _dbWrapper.ExecutePage<TeacherSalaryClassDayView>(table, "*", request.PageSize, request.PageCurrent, "TeacherId", string.Empty);
        }

        public async Task<bool> DelTeacherSalaryClassTimes2(long classRecordId)
        {
            await _dbWrapper.Execute($"DELETE EtTeacherSalaryClassTimes2 WHERE TenantId = {_tenantId} AND ClassRecordId = {classRecordId} ");
            return true;
        }

        public async Task<bool> SaveTeacherSalaryClassTimes2(long classRecordId, List<EtTeacherSalaryClassTimes2> entitys)
        {
            await DelTeacherSalaryClassTimes2(classRecordId);
            if (entitys.Count == 1)
            {
                await _dbWrapper.Insert(entitys.First());
            }
            else
            {
                _dbWrapper.InsertRange(entitys);
            }
            return true;
        }

        public async Task<IEnumerable<EtTeacherSalaryClassTimes2>> GetTeacherSalaryClassTimes2(List<long> teacherIds, DateTime startOt, DateTime endOt)
        {
            if (teacherIds == null || teacherIds.Count == 0)
            {
                return null;
            }
            var str = new StringBuilder($"SELECT * FROM EtTeacherSalaryClassTimes2 WHERE TenantId = {_tenantId} AND  IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startOt.EtmsToDateString()}' AND Ot <= '{endOt.EtmsToDateString()}'");
            if (teacherIds.Count == 1)
            {
                str.Append($" AND TeacherId = {teacherIds[0]}");
            }
            else
            {
                str.Append($" AND TeacherId IN ({string.Join(',', teacherIds)})");
            }
            return await _dbWrapper.ExecuteObject<EtTeacherSalaryClassTimes2>(str.ToString());
        }
    }
}
