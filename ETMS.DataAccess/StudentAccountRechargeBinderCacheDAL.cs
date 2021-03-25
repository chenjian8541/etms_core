using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentAccountRechargeBinderCacheDAL : DataAccessBase<StudentAccountRechargeBinderBucket>, IStudentAccountRechargeBinderCacheDAL
    {
        public StudentAccountRechargeBinderCacheDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StudentAccountRechargeBinderBucket> GetDb(params object[] keys)
        {
            var studentId = keys[1].ToInt();
            var log = await this._dbWrapper.Find<EtStudentAccountRechargeBinder>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.StudentId == studentId);
            return new StudentAccountRechargeBinderBucket()
            {
                StudentAccountRechargeBinder = log
            };
        }

        public async Task UpdateStudentAccountRechargeBinder(long studentId)
        {
            await UpdateCache(_tenantId, studentId);
        }

        public async Task<EtStudentAccountRechargeBinder> GetStudentAccountRechargeBinder(long studentId)
        {
            var bucket = await GetCache(_tenantId, studentId);
            return bucket?.StudentAccountRechargeBinder;
        }

        public void RemoveStudentAccountRechargeBinder(long studentId)
        {
            RemoveCache(_tenantId, studentId);
        }
    }
}
