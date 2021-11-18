using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View.Persistence;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess
{
    public class StatisticsStudentSourceDAL : DataAccessBase, IStatisticsStudentSourceDAL
    {
        public StatisticsStudentSourceDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task UpdateStatisticsStudentSource(DateTime time)
        {
            var otDesc = time.EtmsToDateString();
            var dayData = await this._dbWrapper.ExecuteObject<StatisticsStudentSourceView>($"SELECT SourceId,COUNT(0) AS TotalCount FROM EtStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{otDesc}' GROUP BY SourceId");
            await _dbWrapper.ExecuteScalar($"DELETE EtStatisticsStudentSource WHERE TenantId = {_tenantId} AND Ot = '{otDesc}'");
            var listData = new List<EtStatisticsStudentSource>();
            foreach (var p in dayData)
            {
                listData.Add(new EtStatisticsStudentSource()
                {
                    Count = p.TotalCount,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = time,
                    SourceId = p.SourceId,
                    TenantId = _tenantId
                });
            }
            if (listData.Any())
            {
               await _dbWrapper.InsertRangeAsync(listData);
            }
        }

        public async Task<IEnumerable<StatisticsStudentSourceView>> GetStatisticsStudentSource(DateTime startTime, DateTime endTime)
        {
            var sql = $"SELECT SourceId,SUM([Count]) AS TotalCount FROM EtStatisticsStudentSource WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}' GROUP BY SourceId";
            return await _dbWrapper.ExecuteObject<StatisticsStudentSourceView>(sql);
        }
    }
}
