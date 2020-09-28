using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View.Persistence;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StatisticsClassAttendanceTagDAL : DataAccessBase, IStatisticsClassAttendanceTagDAL
    {
        public StatisticsClassAttendanceTagDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task UpdateStatisticsClassAttendanceTag(DateTime time)
        {
            var otDesc = time.EtmsToDateString();
            var dayData = await _dbWrapper.ExecuteObject<StatisticsClassAttendanceTagView>($"SELECT StudentCheckStatus,COUNT(0) AS TotalCount FROM EtClassRecordStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassRecordStatus.Normal} AND ClassOt = '{otDesc}' GROUP BY StudentCheckStatus");
            await _dbWrapper.ExecuteScalar($"DELETE EtStatisticsClassAttendanceTag WHERE TenantId = {_tenantId} AND Ot = '{otDesc}'");
            var listData = new List<EtStatisticsClassAttendanceTag>();
            foreach (var p in dayData)
            {
                listData.Add(new EtStatisticsClassAttendanceTag()
                {
                    Count = p.TotalCount,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = time,
                    StudentCheckStatus = p.StudentCheckStatus,
                    TenantId = _tenantId
                });
            }
            if (listData.Any())
            {
                _dbWrapper.InsertRange(listData);
            }
        }

        public async Task<IEnumerable<StatisticsClassAttendanceTagView>> GetStatisticsClassAttendanceTag(DateTime startTime, DateTime endTime)
        {
            var sql = $"SELECT StudentCheckStatus,SUM([Count]) AS TotalCount FROM EtStatisticsClassAttendanceTag WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}' GROUP BY StudentCheckStatus";
            return await _dbWrapper.ExecuteObject<StatisticsClassAttendanceTagView>(sql);
        }
    }
}
