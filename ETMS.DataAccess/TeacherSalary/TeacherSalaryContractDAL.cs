using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.TeacherSalary;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.TeacherSalary
{
    public class TeacherSalaryContractDAL : DataAccessBase<TeacherSalaryContractBucket>, ITeacherSalaryContractDAL
    {
        public TeacherSalaryContractDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TeacherSalaryContractBucket> GetDb(params object[] keys)
        {
            var teacherId = keys[1].ToLong();
            var teacherSalaryContractFixeds = await this._dbWrapper.FindList<EtTeacherSalaryContractFixed>(p => p.TenantId == _tenantId
            && p.TeacherId == teacherId && p.IsDeleted == EmIsDeleted.Normal);

            var teacherSalaryContractPerformanceSet = await this._dbWrapper.Find<EtTeacherSalaryContractPerformanceSet>(p => p.TenantId == _tenantId
            && p.TeacherId == teacherId && p.IsDeleted == EmIsDeleted.Normal);

            var teacherSalaryContractPerformanceSetDetail = await this._dbWrapper.FindList<EtTeacherSalaryContractPerformanceSetDetail>(p => p.TenantId == _tenantId
            && p.TeacherId == teacherId && p.IsDeleted == EmIsDeleted.Normal);

            var bascs = await this._dbWrapper.FindList<EtTeacherSalaryContractPerformanceLessonBasc>(p => p.TenantId == _tenantId
            && p.TeacherId == teacherId && p.IsDeleted == EmIsDeleted.Normal);
            return new TeacherSalaryContractBucket()
            {
                TeacherSalaryContractFixeds = teacherSalaryContractFixeds,
                TeacherSalaryContractPerformanceSet = teacherSalaryContractPerformanceSet,
                TeacherSalaryContractPerformanceSetDetails = teacherSalaryContractPerformanceSetDetail,
                EtTeacherSalaryContractPerformanceLessonBascs = bascs
            };
        }

        public async Task<TeacherSalaryContractBucket> GetTeacherSalaryContract(long teacherId)
        {
            return await GetCache(_tenantId, teacherId);
        }

        public async Task<bool> SaveTeacherSalaryContract(long teacherId, List<EtTeacherSalaryContractFixed> fixeds, EtTeacherSalaryContractPerformanceSet performanceSet,
            List<EtTeacherSalaryContractPerformanceSetDetail> performanceSetDetails, List<EtTeacherSalaryContractPerformanceLessonBasc> bascs)
        {
            var strSql = new StringBuilder();
            strSql.Append($"DELETE EtTeacherSalaryContractFixed WHERE TenantId = {_tenantId} AND TeacherId  = {teacherId} ;");
            strSql.Append($"DELETE EtTeacherSalaryContractPerformanceSet WHERE TenantId = {_tenantId} AND TeacherId  = {teacherId} ;");
            strSql.Append($"DELETE EtTeacherSalaryContractPerformanceSetDetail WHERE TenantId = {_tenantId} AND TeacherId  = {teacherId} ;");
            strSql.Append($"DELETE EtTeacherSalaryContractPerformanceLessonBasc WHERE TenantId = {_tenantId} AND TeacherId  = {teacherId} ;");
            await _dbWrapper.Execute(strSql.ToString());
            if (fixeds.Any())
            {
                _dbWrapper.InsertRange(fixeds);
            }
            if (performanceSet != null)
            {
                await _dbWrapper.Insert(performanceSet);
            }
            if (performanceSetDetails.Any())
            {
                _dbWrapper.InsertRange(performanceSetDetails);
            }
            if (bascs != null && bascs.Any())
            {
                _dbWrapper.InsertRange(bascs);
            }
            await UpdateCache(_tenantId, teacherId);
            return true;
        }

        public async Task ClearTeacherSalaryContractPerformance(long teacherId)
        {
            var strSql = new StringBuilder();
            strSql.Append($"DELETE EtTeacherSalaryContractPerformanceSet WHERE TenantId = {_tenantId} AND TeacherId  = {teacherId} ;");
            strSql.Append($"DELETE EtTeacherSalaryContractPerformanceSetDetail WHERE TenantId = {_tenantId} AND TeacherId  = {teacherId} ;");
            strSql.Append($"DELETE EtTeacherSalaryContractPerformanceLessonBasc WHERE TenantId = {_tenantId} AND TeacherId  = {teacherId} ;");
            await _dbWrapper.Execute(strSql.ToString());
            await UpdateCache(_tenantId, teacherId);
        }
    }
}
