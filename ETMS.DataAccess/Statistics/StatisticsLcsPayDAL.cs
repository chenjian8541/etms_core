using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess.Statistics;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Statistics
{
    public class StatisticsLcsPayDAL : DataAccessBase<StatisticsLcsPayYearBucket>, IStatisticsLcsPayDAL
    {
        public StatisticsLcsPayDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StatisticsLcsPayYearBucket> GetDb(params object[] keys)
        {
            var year = keys[1].ToInt();
            var sql = $"SELECT ISNULL(SUM(TotalMoney),0) AS MyTotalMoney,ISNULL(SUM(TotalMoneyRefund),0) AS MyTotalMoneyRefund, ISNULL(SUM(TotalMoneyValue),0) AS MyTotalMoneyValue FROM EtStatisticsLcsPayMonth WHERE TenantId = {_tenantId} AND [Year] = {year} AND IsDeleted = {EmIsDeleted.Normal}";
            var data = await _dbWrapper.ExecuteObject<StatisticsLcsPayYearView>(sql);
            var bucket = new StatisticsLcsPayYearBucket()
            {
                TotalMoney = 0,
                TotalMoneyRefund = 0,
                TotalMoneyValue = 0
            };
            if (data.Any())
            {
                var myData = data.First();
                bucket.TotalMoney = myData.MyTotalMoney;
                bucket.TotalMoneyRefund = myData.MyTotalMoneyRefund;
                bucket.TotalMoneyValue = myData.MyTotalMoneyValue;
            }
            return bucket;
        }

        public async Task UpdateStatisticsLcsPayDay(DateTime date)
        {
            var otDesc = date.EtmsToDateString();
            var sql = $"SELECT [Status],SUM(TotalFeeValue) TotalValue FROM EtTenantLcsPayLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND PayFinishDate = '{otDesc}' AND ([Status] = {EmLcsPayLogStatus.PaySuccess} OR [Status] = {EmLcsPayLogStatus.Refunded}) GROUP BY [Status]";
            var statisticsData = await _dbWrapper.ExecuteObject<TenantLcsPaySumValue>(sql);
            var totalMoneyValue = 0M;
            var totalMoney = 0M;
            if (statisticsData.Any())
            {
                var paySuccessData = statisticsData.Where(p => p.Status == EmLcsPayLogStatus.PaySuccess).FirstOrDefault();
                if (paySuccessData != null)
                {
                    totalMoneyValue = paySuccessData.TotalValue;
                    totalMoney += paySuccessData.TotalValue;
                }
                var refundedData = statisticsData.Where(p => p.Status == EmLcsPayLogStatus.Refunded).FirstOrDefault();
                if (refundedData != null)
                {
                    totalMoney += refundedData.TotalValue;
                }
            }
            var totalMoneyRefund = 0M;
            sql = $"SELECT ISNULL(SUM(TotalFeeValue),0) TotalMoney FROM EtTenantLcsPayLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND RefundDate = '{otDesc}' AND [Status] = {EmLcsPayLogStatus.Refunded} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            if (obj != null)
            {
                totalMoneyRefund = Convert.ToDecimal(obj);
            }

            var hisData = await _dbWrapper.Find<EtStatisticsLcsPayDay>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == date);
            if (hisData != null)
            {
                hisData.TotalMoney = totalMoney;
                hisData.TotalMoneyRefund = totalMoneyRefund;
                hisData.TotalMoneyValue = totalMoneyValue;
                await _dbWrapper.Update(hisData);
            }
            else
            {
                await _dbWrapper.Insert(new EtStatisticsLcsPayDay()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    OrderType = -1,
                    Ot = date,
                    TenantId = _tenantId,
                    TotalMoney = totalMoney,
                    TotalMoneyRefund = totalMoneyRefund,
                    TotalMoneyValue = totalMoneyValue
                });
            }
        }

        public async Task UpdateStatisticsLcsPayMonth(DateTime myDate)
        {
            var firstDate = new DateTime(myDate.Year, myDate.Month, 1);
            var startTimeDesc = firstDate.EtmsToDateString();
            var endTimeDesc = firstDate.AddMonths(1).EtmsToDateString();
            var sql = $"SELECT [Status],SUM(TotalFeeValue) TotalValue FROM EtTenantLcsPayLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND PayFinishDate >= '{startTimeDesc}' AND PayFinishDate < '{endTimeDesc}' AND ([Status] = {EmLcsPayLogStatus.PaySuccess} OR [Status] = {EmLcsPayLogStatus.Refunded}) GROUP BY [Status]";
            var statisticsData = await _dbWrapper.ExecuteObject<TenantLcsPaySumValue>(sql);
            var totalMoneyValue = 0M;
            var totalMoney = 0M;
            if (statisticsData.Any())
            {
                var paySuccessData = statisticsData.Where(p => p.Status == EmLcsPayLogStatus.PaySuccess).FirstOrDefault();
                if (paySuccessData != null)
                {
                    totalMoneyValue = paySuccessData.TotalValue;
                    totalMoney += paySuccessData.TotalValue;
                }
                var refundedData = statisticsData.Where(p => p.Status == EmLcsPayLogStatus.Refunded).FirstOrDefault();
                if (refundedData != null)
                {
                    totalMoney += refundedData.TotalValue;
                }
            }
            var totalMoneyRefund = 0M;
            sql = $"SELECT ISNULL(SUM(TotalFeeValue),0) TotalMoney FROM EtTenantLcsPayLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND RefundDate >= '{startTimeDesc}' AND RefundDate< '{endTimeDesc}' AND [Status] = {EmLcsPayLogStatus.Refunded} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            if (obj != null)
            {
                totalMoneyRefund = Convert.ToDecimal(obj);
            }

            var hisData = await _dbWrapper.Find<EtStatisticsLcsPayMonth>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == firstDate);
            if (hisData != null)
            {
                hisData.TotalMoney = totalMoney;
                hisData.TotalMoneyRefund = totalMoneyRefund;
                hisData.TotalMoneyValue = totalMoneyValue;
                await _dbWrapper.Update(hisData);
            }
            else
            {
                await _dbWrapper.Insert(new EtStatisticsLcsPayMonth()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    OrderType = -1,
                    Ot = firstDate,
                    TenantId = _tenantId,
                    TotalMoney = totalMoney,
                    TotalMoneyRefund = totalMoneyRefund,
                    TotalMoneyValue = totalMoneyValue,
                    Year = firstDate.Year,
                    Month = firstDate.Month
                });
            }
            RemoveCache(_tenantId, firstDate.Year);
        }

        public async Task<List<EtStatisticsLcsPayDay>> GetStatisticsLcsPayDay(DateTime startTime, DateTime endTime)
        {
            return await this._dbWrapper.FindList<EtStatisticsLcsPayDay>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<List<EtStatisticsLcsPayMonth>> GetStatisticsLcsPayMonth(DateTime startTime, DateTime endTime)
        {
            return await this._dbWrapper.FindList<EtStatisticsLcsPayMonth>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<Tuple<IEnumerable<EtStatisticsLcsPayDay>, int>> GetStatisticsLcsPayDayPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsLcsPayDay>("EtStatisticsLcsPayDay", "*", request.PageSize, request.PageCurrent, "[Ot] DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtStatisticsLcsPayMonth>, int>> GetStatisticsLcsPayMonthPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsLcsPayMonth>("EtStatisticsLcsPayMonth", "*", request.PageSize, request.PageCurrent, "[Ot] DESC", request.ToString());
        }

        public async Task<StatisticsLcsPayYearBucket> GetStatisticsLcsPayYear(int year)
        {
            return await GetCache(_tenantId, year);
        }
    }
}
