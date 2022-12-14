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
using ETMS.Entity.CacheBucket;
using ETMS.ICache;
using ETMS.Entity.View;

namespace ETMS.DataAccess
{
    public class StatisticsFinanceIncomeDAL : DataAccessBase<StatisticsFinanceIncomeYearBucket>, IStatisticsFinanceIncomeDAL
    {
        public StatisticsFinanceIncomeDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StatisticsFinanceIncomeYearBucket> GetDb(params object[] keys)
        {
            var year = keys[1].ToInt();
            var sql = $"SELECT [Type],SUM(TotalSum) AS MyTotalSum FROM EtStatisticsFinanceIncomeMonth WHERE TenantId = {_tenantId} AND [Year] = {year} AND IsDeleted = {EmIsDeleted.Normal} GROUP BY [Type]";
            var data = await _dbWrapper.ExecuteObject<StatisticsFinanceIncomeYearView>(sql);
            var inLog = data.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountIn);
            var outLog = data.FirstOrDefault(p => p.Type == EmIncomeLogType.AccountOut);
            var output = new StatisticsFinanceIncomeYearBucket();
            if (inLog != null)
            {
                output.TotalSumIn = inLog.MyTotalSum;
            }
            if (outLog != null)
            {
                output.TotalSumOut = outLog.MyTotalSum;
            }
            return output;
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
            RemoveCache(_tenantId, date.Year);
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
            RemoveCache(_tenantId, time.Year);
        }

        public async Task<List<EtStatisticsFinanceIncome>> GetStatisticsFinanceIncome(DateTime startTime, DateTime endTime, byte type)
        {
            return await _dbWrapper.FindList<EtStatisticsFinanceIncome>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime && p.Type == type);
        }

        public async Task<List<EtStatisticsFinanceIncome>> GetStatisticsFinanceIncome(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsFinanceIncome>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<List<EtStatisticsFinanceIncomeMonth>> GetStatisticsFinanceIncomeMonth(DateTime startTime, DateTime endTime, byte type)
        {
            return await _dbWrapper.FindList<EtStatisticsFinanceIncomeMonth>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.Ot >= startTime && p.Ot <= endTime && p.Type == type);
        }

        public async Task<Tuple<IEnumerable<EtStatisticsFinanceIncomeMonth>, int>> GetStatisticsFinanceIncomeMonthPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsFinanceIncomeMonth>("EtStatisticsFinanceIncomeMonth", "*", request.PageSize, request.PageCurrent, "[Ot] DESC", request.ToString());
        }

        public async Task<StatisticsFinanceIncomeYearBucket> GetStatisticsFinanceIncomeYear(int year)
        {
            return await GetCache(_tenantId, year);
        }
    }
}
