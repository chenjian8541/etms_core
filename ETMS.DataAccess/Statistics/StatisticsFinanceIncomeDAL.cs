using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Temp.View;
using ETMS.Entity.Common;

namespace ETMS.DataAccess
{
    public class StatisticsFinanceIncomeDAL : DataAccessBase, IStatisticsFinanceIncomeDAL
    {
        public StatisticsFinanceIncomeDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task UpdateStatisticsFinanceIncome(DateTime date)
        {
            var otDesc = date.EtmsToDateString();
            var statisticsSql = $"SELECT [Type],[ProjectType],[PayType],SUM([Sum]) AS TotalSum FROM EtIncomeLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmIncomeLogStatus.Normal} AND  Ot = '{otDesc}' GROUP BY [Type],[ProjectType],[PayType]";
            var hisData = await _dbWrapper.ExecuteObject<StatisticsFinanceIncomeView>(statisticsSql);
            await _dbWrapper.ExecuteScalar($"DELETE EtStatisticsFinanceIncome WHERE TenantId = {_tenantId} AND Ot = '{otDesc}'");
            var newEtStatisticsFinanceIncome = new List<EtStatisticsFinanceIncome>();
            foreach (var p in hisData)
            {
                newEtStatisticsFinanceIncome.Add(new EtStatisticsFinanceIncome()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = date,
                    PayType = p.PayType,
                    ProjectType = p.ProjectType,
                    TenantId = _tenantId,
                    TotalSum = p.TotalSum,
                    Type = p.Type
                });
            }
            if (newEtStatisticsFinanceIncome.Any())
            {
                await _dbWrapper.InsertRangeAsync(newEtStatisticsFinanceIncome);
            }
        }

        public async Task UpdateStatisticsFinanceIncomeMonth(DateTime time)
        {
            var firstDate = new DateTime(time.Year, time.Month, 1);
            var startTimeDesc = firstDate.EtmsToDateString();
            var endTimeDesc = firstDate.AddMonths(1).EtmsToDateString();
            var statisticsSql = $"SELECT [Type],[ProjectType],SUM([Sum]) AS TotalSum,COUNT(Id) AS TotalCount FROM EtIncomeLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmIncomeLogStatus.Normal} AND Ot >= '{startTimeDesc}' AND Ot < '{endTimeDesc}'  GROUP BY [Type],[ProjectType]";
            var myStatistics = await _dbWrapper.ExecuteObject<StatisticsFinanceIncomeMonthView>(statisticsSql);
            await _dbWrapper.Execute($"DELETE EtStatisticsFinanceIncomeMonth WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot = '{startTimeDesc}'");
            if (myStatistics.Any())
            {
                var statisticsFinanceIncomeMonths = new List<EtStatisticsFinanceIncomeMonth>();
                foreach (var p in myStatistics)
                {
                    statisticsFinanceIncomeMonths.Add(new EtStatisticsFinanceIncomeMonth()
                    {
                        IsDeleted = EmIsDeleted.Normal,
                        Month = firstDate.Month,
                        Ot = firstDate,
                        Year = firstDate.Year,
                        TenantId = _tenantId,
                        Type = p.Type,
                        ProjectType = p.ProjectType,
                        TotalCount = p.TotalCount,
                        TotalSum = p.TotalSum
                    });
                }
                _dbWrapper.InsertRange(statisticsFinanceIncomeMonths);
            }
        }

        public async Task<List<EtStatisticsFinanceIncome>> GetStatisticsFinanceIncome(DateTime startTime, DateTime endTime, byte type)
        {
            return await _dbWrapper.FindList<EtStatisticsFinanceIncome>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime && p.Type == type);
        }

        public async Task<List<EtStatisticsFinanceIncomeMonth>> GetStatisticsFinanceIncomeMonth(DateTime startTime, DateTime endTime, byte type)
        {
            return await _dbWrapper.FindList<EtStatisticsFinanceIncomeMonth>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.Ot >= startTime && p.Ot <= endTime && p.Type == type);
        }

        public async Task<Tuple<IEnumerable<EtStatisticsFinanceIncomeMonth>, int>> GetStatisticsFinanceIncomeMonthPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsFinanceIncomeMonth>("EtStatisticsFinanceIncomeMonth", "*", request.PageSize, request.PageCurrent, "[Ot] DESC", request.ToString());
        }
    }
}
