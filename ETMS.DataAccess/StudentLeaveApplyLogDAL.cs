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
            return await _dbWrapper.ExecutePage<StudentLeaveApplyLogView>("StudentLeaveApplyLogView", "*", request.PageSize, request.PageCurrent, "HandleStatus ASC,Id DESC", request.ToString());
        }

        public async Task<bool> AddStudentLeaveApplyLog(EtStudentLeaveApplyLog log)
        {
            return await _dbWrapper.Insert(log, async () => { await UpdateCache(_tenantId, log.StartDate); });
        }

        public async Task<bool> EditStudentLeaveApplyLog(EtStudentLeaveApplyLog log)
        {
            return await _dbWrapper.Update(log, async () => { await UpdateCache(_tenantId, log.StartDate); });
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
    }
}
