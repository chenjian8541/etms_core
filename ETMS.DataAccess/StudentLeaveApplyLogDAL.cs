using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.ICache;
using ETMS.Entity.Common;
using ETMS.Entity.View;

namespace ETMS.DataAccess
{
    public class StudentLeaveApplyLogDAL : DataAccessBase<StudentLeaveBucket>, IStudentLeaveApplyLogDAL
    {
        public StudentLeaveApplyLogDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StudentLeaveBucket> GetDb(params object[] keys)
        {
            var time = Convert.ToDateTime(keys[1]).Date;
            var logs = await _dbWrapper.FindList<EtStudentLeaveApplyLog>(p => p.TenantId == _tenantId
            && p.EndDate >= time && p.StartDate <= time && p.HandleStatus == EmStudentLeaveApplyHandleStatus.Pass && p.IsDeleted == EmIsDeleted.Normal);
            if (logs == null || !logs.Any())
            {
                return null;
            }
            return new StudentLeaveBucket()
            {
                logs = logs
            };
        }

        public async Task<Tuple<IEnumerable<StudentLeaveApplyLogView>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<StudentLeaveApplyLogView>("StudentLeaveApplyLogView", "*", request.PageSize, request.PageCurrent, $"case when HandleStatus = {EmStudentLeaveApplyHandleStatus.Unreviewed} then 1 else 2 end,id desc", request.ToString());
        }

        public async Task<bool> AddStudentLeaveApplyLog(EtStudentLeaveApplyLog log)
        {
            await _dbWrapper.Insert(log);
            await UpdateCache(log);
            return true;
        }

        public async Task<bool> EditStudentLeaveApplyLog(EtStudentLeaveApplyLog log)
        {
            await _dbWrapper.Update(log);
            await UpdateCache(log);
            return true;
        }

        private async Task UpdateCache(EtStudentLeaveApplyLog log)
        {
            var allDate = EtmsHelper2.GetStartStepToAnd(log.StartDate, log.EndDate);
            if (allDate != null && allDate.Count > 0)
            {
                foreach (var p in allDate)
                {
                    await UpdateCache(_tenantId, p);
                }
            }
        }

        public async Task<EtStudentLeaveApplyLog> GetStudentLeaveApplyLog(long id)
        {
            return await _dbWrapper.Find<EtStudentLeaveApplyLog>(id);
        }

        public async Task<List<EtStudentLeaveApplyLog>> GetStudentLeaveApplyPassLog(DateTime time)
        {
            var bucket = await GetCache(_tenantId, time);
            return bucket?.logs;
        }

        public async Task<int> GetStudentLeaveApplyCount(long studentId, DateTime startTime, DateTime endTime)
        {
            var strStart = startTime.EtmsToDateString();
            var strEnd = endTime.EtmsToDateString();
            var sql = $"SELECT COUNT(0) FROM EtStudentLeaveApplyLog WHERE TenantId = {_tenantId} AND StudentId = {studentId}  AND  IsDeleted = {EmIsDeleted.Normal} AND HandleStatus IN ({EmStudentLeaveApplyHandleStatus.Unreviewed},{EmStudentLeaveApplyHandleStatus.Pass}) AND (StartDate BETWEEN '{strStart}' AND '{strEnd}' OR EndDate  BETWEEN '{strStart}' AND '{strEnd}') ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }
    }
}
