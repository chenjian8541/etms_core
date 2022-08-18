using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View.OnlyOneFiled;
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
    public class ClassTimesRuleStudentDAL : DataAccessBase<ClassTimesRuleStudentBucket>, IClassTimesRuleStudentDAL
    {
        public ClassTimesRuleStudentDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ClassTimesRuleStudentBucket> GetDb(params object[] keys)
        {
            var classId = keys[1].ToLong();
            var ruleId = keys[2].ToLong();
            var logs = await _dbWrapper.FindList<EtClassTimesRuleStudent>(
                p => p.TenantId == _tenantId && p.ClassId == classId && p.RuleId == ruleId && p.IsDeleted == EmIsDeleted.Normal);
            return new ClassTimesRuleStudentBucket()
            {
                ClassTimesRuleStudents = logs
            };
        }

        public async Task<bool> ExistStudent(long studentId, long classId, long ruleId)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtClassTimesRuleStudent WHERE TenantId = {_tenantId} AND ClassId = {classId} AND RuleId = {ruleId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal}");
            return obj != null;
        }

        public async Task<List<EtClassTimesRuleStudent>> GetClassTimesRuleStudent(long classId, long ruleId)
        {
            var bucket = await GetCache(_tenantId, classId, ruleId);
            return bucket?.ClassTimesRuleStudents;
        }

        public async Task AddClassTimesRuleStudent(List<EtClassTimesRuleStudent> entitys)
        {
            if (entitys == null || entitys.Count == 0)
            {
                return;
            }
            var myentity = entitys.First();
            if (entitys.Count == 1)
            {
                await _dbWrapper.Insert(myentity);
            }
            else
            {
                await _dbWrapper.InsertRangeAsync(entitys);
            }
            RemoveCache(_tenantId, myentity.ClassId, myentity.RuleId);
        }

        public async Task DelClassTimesRuleStudent(long id, long classId, long ruleId)
        {
            await _dbWrapper.Execute($"UPDATE EtClassTimesRuleStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id}");
            await UpdateCache(_tenantId, classId, ruleId);
        }

        public async Task DelClassTimesRuleStudentByStudentId(long studentId, long classId)
        {
            var sql = $"SELECT RuleId FROM EtClassTimesRuleStudent WHERE TenantId = {_tenantId} AND ClassId = {classId} AND StudentId = {studentId}";
            var ruleIds = await _dbWrapper.ExecuteObject<OnlyOneFiledRuleId>(sql);
            if (ruleIds.Any())
            {
                await _dbWrapper.Execute(
                    $"UPDATE EtClassTimesRuleStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND ClassId = {classId} AND StudentId = {studentId}");
                foreach (var myRule in ruleIds)
                {
                    RemoveCache(_tenantId, classId, myRule.RuleId);
                }
            }
        }

        public async Task ClearClassTimesRuleStudent(long classId, List<long> ruleIds)
        {
            if (ruleIds == null || ruleIds.Count == 0)
            {
                return;
            }
            var sql = string.Empty;
            if (ruleIds.Count == 1)
            {
                sql = $"UPDATE EtClassTimesRuleStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND ClassId = {classId} AND RuleId = {ruleIds[0]}";
            }
            else
            {
                sql = $"UPDATE EtClassTimesRuleStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND ClassId = {classId} AND RuleId IN ({string.Join(',', ruleIds)})";
            }
            await _dbWrapper.Execute(sql);
            foreach (var p in ruleIds)
            {
                RemoveCache(_tenantId, classId, p);
            }
        }

        public async Task<IEnumerable<OnlyOneFiledRuleId>> GetIsSetRuleStudent(long classId)
        {
            var sql = $"SELECT RuleId FROM EtClassTimesRuleStudent WHERE TenantId = {_tenantId} AND ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal}  GROUP BY RuleId";
            return await _dbWrapper.ExecuteObject<OnlyOneFiledRuleId>(sql);
        }
    }
}
