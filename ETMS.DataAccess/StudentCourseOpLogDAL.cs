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
using System.Linq;

namespace ETMS.DataAccess
{
    public class StudentCourseOpLogDAL : DataAccessBase<StudentCourseOpLogBucket>, IStudentCourseOpLogDAL
    {
        public StudentCourseOpLogDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StudentCourseOpLogBucket> GetDb(params object[] keys)
        {
            var studentId = keys[1].ToLong();
            var log = await _dbWrapper.FindList<EtStudentCourseOpLog>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.StudentId == studentId);
            return new StudentCourseOpLogBucket()
            {
                StudentCourseOpLogs = log
            };
        }

        public async Task<bool> AddStudentCourseOpLog(EtStudentCourseOpLog entity)
        {
            await _dbWrapper.Insert(entity);
            RemoveCache(_tenantId, entity.StudentId);
            return true;
        }

        public void AddStudentCourseOpLog(List<EtStudentCourseOpLog> entitys)
        {
            _dbWrapper.InsertRange(entitys);
            var studentIds = entitys.Select(p => p.StudentId).Distinct();
            foreach (var myStudentId in studentIds)
            {
                RemoveCache(_tenantId, myStudentId);
            }
        }

        public async Task<List<EtStudentCourseOpLog>> GetStudentCourseOpLogs(long studentId)
        {
            var bucket = await GetCache(_tenantId, studentId);
            return bucket?.StudentCourseOpLogs;
        }
    }
}
