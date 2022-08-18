using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
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
    public class ReservationCourseSetDAL : DataAccessBase<ReservationCourseSetBucket>, IReservationCourseSetDAL
    {
        public ReservationCourseSetDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ReservationCourseSetBucket> GetDb(params object[] keys)
        {
            var logs = await _dbWrapper.FindList<EtReservationCourseSet>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            return new ReservationCourseSetBucket()
            {
                ReservationCourseSets = logs
            };
        }

        public async Task<bool> ExistReservationCourse(long courseId)
        {
            var log = await _dbWrapper.Find<EtReservationCourseSet>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal &&
            p.CourseId == courseId);
            return log != null;
        }

        public async Task<List<EtReservationCourseSet>> GetReservationCourseSet()
        {
            var bucket = await GetCache(_tenantId);
            return bucket?.ReservationCourseSets;
        }

        public async Task AddReservationCourseSet(EtReservationCourseSet entity)
        {
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_tenantId);
        }

        public async Task UpdateReservationCourseSet(long id, int limitCount)
        {
            await _dbWrapper.Execute($"UPDATE EtReservationCourseSet SET LimitCount = {limitCount} WHERE Id = {id}");
            await UpdateCache(_tenantId);
        }

        public async Task DelReservationCourseSet(long id)
        {
            await _dbWrapper.Execute($"DELETE EtReservationCourseSet WHERE Id = {id} AND TenantId = {_tenantId}");
            await UpdateCache(_tenantId);
        }
    }
}
