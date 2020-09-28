using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess
{
    public class ParentStudentDAL : DataAccessBase<ParentStudentBucket>, IParentStudentDAL
    {
        public ParentStudentDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }
        protected override async Task<ParentStudentBucket> GetDb(params object[] keys)
        {
            var phone = keys[1].ToString();
            var students = await _dbWrapper.ExecuteObject<ParentStudentInfo>(
                $"SELECT Id,Name,Gender,Avatar from EtStudent WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {keys[0]} AND (Phone = '{phone}' or PhoneBak = '{phone}')");
            if (students == null || !students.Any())
            {
                return null;
            }
            return new ParentStudentBucket()
            {
                Phone = phone,
                ParentStudents = students
            };
        }

        public async Task<IEnumerable<ParentStudentInfo>> GetParentStudents(int tenantId, string phone)
        {
            var parentStudentBucket = await base.GetCache(tenantId, phone);
            return parentStudentBucket?.ParentStudents;
        }

        public async Task<IEnumerable<ParentStudentInfo>> UpdateCacheAndGetParentStudents(int tenantId, string phone)
        {
            var parentStudentBucket = await base.UpdateCache(tenantId, phone);
            return parentStudentBucket?.ParentStudents;
        }
    }
}
