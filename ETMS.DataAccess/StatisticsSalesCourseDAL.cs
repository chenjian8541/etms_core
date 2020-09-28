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
    public class StatisticsSalesCourseDAL : DataAccessBase, IStatisticsSalesCourseDAL
    {
        public StatisticsSalesCourseDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task UpdateStatisticsSalesCourse(DateTime time)
        {
            var otDesc = time.EtmsToDateString();
            var dayData = await this._dbWrapper.ExecuteObject<StatisticsSalesCourseView>($"SELECT ProductId AS CourseId,BugUnit,SUM(BuyQuantity) AS TotalCount,SUM(ItemAptSum) AS TotalAmount  FROM EtOrderDetail WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] <> {EmOrderStatus.Repeal} AND ProductType = {EmOrderProductType.Course} AND Ot = '{otDesc}' GROUP BY ProductId,BugUnit");
            await _dbWrapper.ExecuteScalar($"DELETE EtStatisticsSalesCourse WHERE TenantId = {_tenantId} AND Ot = '{otDesc}'");
            var statisticsSalesCourseData = new List<EtStatisticsSalesCourse>();
            foreach (var p in dayData)
            {
                statisticsSalesCourseData.Add(new EtStatisticsSalesCourse()
                {
                    Amount = p.TotalAmount,
                    Count = p.TotalCount,
                    CourseId = p.CourseId,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = time,
                    TenantId = _tenantId,
                    BugUnit = p.BugUnit
                });
            }
            if (statisticsSalesCourseData.Any())
            {
                _dbWrapper.InsertRange(statisticsSalesCourseData);
            }
        }

        public async Task<IEnumerable<GetStatisticsSalesCourseByAmountView>> GetStatisticsSalesCourseForAmount(DateTime startTime, DateTime endTime, int topLimit)
        {
            var sql = $"SELECT TOP {topLimit} CourseId,SUM(Amount) AS TotalAmount FROM EtStatisticsSalesCourse WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}' GROUP BY CourseId  ORDER BY TotalAmount DESC";
            return await _dbWrapper.ExecuteObject<GetStatisticsSalesCourseByAmountView>(sql.ToString());
        }

        public async Task<IEnumerable<GetStatisticsSalesCourseForCountView>> GetStatisticsSalesCourseForCount(DateTime startTime, DateTime endTime, int topLimit, byte bugUnit)
        {
            var sql = $"SELECT TOP {topLimit} CourseId,SUM([Count]) AS TotalCount FROM EtStatisticsSalesCourse WHERE TenantId = {_tenantId} AND BugUnit = {bugUnit} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}' GROUP BY CourseId  ORDER BY TotalCount DESC";
            return await _dbWrapper.ExecuteObject<GetStatisticsSalesCourseForCountView>(sql);
        }
    }
}
