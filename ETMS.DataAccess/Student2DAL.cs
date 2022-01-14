using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class Student2DAL : DataAccessBase<StudentByCardNoBucket>, IStudent2DAL
    {
        public Student2DAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StudentByCardNoBucket> GetDb(params object[] keys)
        {
            var cardNo = keys[1].ToString();
            if (string.IsNullOrEmpty(cardNo))
            {
                return null;
            }
            var student = await _dbWrapper.Find<EtStudent>(p => p.TenantId == _tenantId && p.CardNo == cardNo && p.IsDeleted == EmIsDeleted.Normal);
            if (student == null)
            {
                return null;
            }
            return new StudentByCardNoBucket()
            {
                Student = student
            };
        }

        public async Task<bool> UpdateCache(string cardNo)
        {
            await UpdateCache(_tenantId, cardNo);
            return true;
        }

        public bool RemoveCache(string cardNo)
        {
            RemoveCache(_tenantId, cardNo);
            return true;
        }

        public async Task<EtStudent> GetStudent(string cardNo)
        {
            var bucket = await GetCache(_tenantId, cardNo);
            return bucket?.Student;
        }

        public async Task<EtStudent> GetStudentByDb(string cardNo)
        {
            return await _dbWrapper.Find<EtStudent>(p => p.TenantId == _tenantId && p.CardNo == cardNo && p.IsDeleted == EmIsDeleted.Normal);
        }
    }
}
