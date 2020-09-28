using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
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
    public class StudentTrackLogDAL : DataAccessBase<StudentTrackLogBucket>, IStudentTrackLogDAL
    {
        public StudentTrackLogDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StudentTrackLogBucket> GetDb(params object[] keys)
        {
            var studentTrackLogs = await _dbWrapper.FindList<EtStudentTrackLog>(p => p.TenantId == _tenantId && p.StudentId == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            return new StudentTrackLogBucket()
            {
                TrackLogs = studentTrackLogs.OrderByDescending(p => p.Id).ToList()
            };
        }

        public async Task<long> AddStudentTrackLog(EtStudentTrackLog etStudentTrackLog)
        {
             await _dbWrapper.Insert(etStudentTrackLog, async () => { await UpdateCache(_tenantId, etStudentTrackLog.StudentId); });
            return etStudentTrackLog.Id;
        }

        public async Task<bool> AddStudentTrackLog(List<EtStudentTrackLog> etStudentTrackLogs)
        {
            _dbWrapper.InsertRange(etStudentTrackLogs);
            await UpdateCache(_tenantId, etStudentTrackLogs.First().StudentId);
            return true;
        }
        public async Task<List<EtStudentTrackLog>> GetStudentTrackLog(long studentId)
        {
            var studentTrackLogBucket = await this.GetCache(_tenantId, studentId);
            return studentTrackLogBucket?.TrackLogs;
        }

        public async Task<bool> EditStudentTrackLog(EtStudentTrackLog etStudentTrackLog)
        {
            return await _dbWrapper.Update(etStudentTrackLog, async () => { await UpdateCache(_tenantId, etStudentTrackLog.StudentId); });
        }

        public async Task<EtStudentTrackLog> GetTrackLog(long id)
        {
            return await _dbWrapper.Find<EtStudentTrackLog>(id);
        }

        public async Task<bool> DelStudentTrackLog(long id, long studentId)
        {
            await _dbWrapper.Execute($"UPDATE EtStudentTrackLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id}");
            await this.UpdateCache(_tenantId, studentId);
            return true;
        }

        public async Task<Tuple<IEnumerable<StudentTrackLogView>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<StudentTrackLogView>("StudentTrackLogView", "*", request.PageSize, request.PageCurrent, "TrackTime DESC", request.ToString());
        }
    }
}
