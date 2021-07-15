using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class JobAnalyzeDAL : DataAccessBase, IJobAnalyzeDAL
    {
        public JobAnalyzeDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task UpdateClassTimesRuleLoopStatus()
        {
            await _dbWrapper.Execute($"UPDATE EtClassTimesRule SET IsNeedLoop = {EmBool.False} WHERE TenantId = {_tenantId} AND EndDate IS NOT NULL AND LastJobProcessTime >= EndDate AND IsDeleted = {EmIsDeleted.Normal}");
        }

        public async Task<Tuple<IEnumerable<LoopClassTimesRule>, int>> GetNeedLoopClassTimesRule(int pageSize, int pageCurrent)
        {
            return await _dbWrapper.ExecutePage<LoopClassTimesRule>("EtClassTimesRule", "Id,ClassId", pageSize, pageCurrent, "Id DESC",
                $" TenantId = {_tenantId} AND IsNeedLoop = {EmBool.True} AND IsDeleted = {EmIsDeleted.Normal}");
        }

        public async Task<EtClassTimesRule> GetClassTimesRule(long id)
        {
            return await _dbWrapper.Find<EtClassTimesRule>(id);
        }

        public async Task UpdateClassTimesRule(long id, DateTime lastJobProcessTime)
        {
            await _dbWrapper.Execute($"UPDATE EtClassTimesRule SET LastJobProcessTime = '{lastJobProcessTime.EtmsToDateString()}' WHERE id = {id}");
        }

        public async Task AddClassTimes(EtClassTimes etClassTimes)
        {
            await _dbWrapper.Insert(etClassTimes);
        }

        public async Task<Tuple<IEnumerable<StudentCourseConsume>, int>> GetNeedConsumeStudentCourse(int pageSize, int pageCurrent, DateTime time)
        {
            var timeDesc = time.EtmsToDateString();
            return await _dbWrapper.ExecutePage<StudentCourseConsume>("EtStudentCourseDetail", "Id", pageSize, pageCurrent, "Id DESC",
             $" TenantId = {_tenantId} AND [Status] = {EmStudentCourseStatus.Normal} AND DeType = {EmDeClassTimesType.Day}  AND StartTime <= '{timeDesc}' AND EndTime >= '{timeDesc}' AND IsDeleted = {EmIsDeleted.Normal} AND (LastJobProcessTime IS NULL OR LastJobProcessTime < '{timeDesc}') ");
        }

        public async Task<EtStudentCourseDetail> GetStudentCourseDetail(long id)
        {
            return await _dbWrapper.Find<EtStudentCourseDetail>(id);
        }

        public async Task<bool> EditStudentCourseDetail(EtStudentCourseDetail entity)
        {
            return await _dbWrapper.Update(entity);
        }

        public async Task<Tuple<IEnumerable<HasCourseStudent>, int>> GetHasCourseStudent(int pageSize, int pageCurrent)
        {
            return await _dbWrapper.ExecutePage<HasCourseStudent>("EtStudent", "Id", pageSize, pageCurrent, "Id DESC",
                $" TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND StudentType = {EmStudentType.ReadingStudent}");
        }

        public async Task<List<EtClassTimes>> GetClassTimesUnRollcall(DateTime classOt)
        {
            return await _dbWrapper.FindList<EtClassTimes>(p => p.TenantId == _tenantId && p.ClassOt == classOt && p.Status == EmClassTimesStatus.UnRollcall && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<List<EtClassTimesStudent>> GetClassTimesStudent(long classTimesId)
        {
            return await _dbWrapper.FindList<EtClassTimesStudent>(p => p.TenantId == _tenantId && p.ClassTimesId == classTimesId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<EtClassTimes> GetClassTimes(long classTimesId)
        {
            return await _dbWrapper.Find<EtClassTimes>(p => p.Id == classTimesId && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<EtClassRecord> GetClassRecord(long id)
        {
            return await _dbWrapper.Find<EtClassRecord>(p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<List<EtClassRecordStudent>> GetClassRecordStudent(long classRecordId)
        {
            return await _dbWrapper.FindList<EtClassRecordStudent>(p => p.ClassRecordId == classRecordId && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<Tuple<IEnumerable<OnlyId>, int>> GetStudent(int pageSize, int pageCurrent)
        {
            return await _dbWrapper.ExecutePage<OnlyId>("EtStudent", "Id", pageSize, pageCurrent, "Id DESC",
                $" TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} ");
        }
    }
}
