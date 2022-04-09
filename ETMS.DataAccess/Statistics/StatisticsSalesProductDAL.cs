using ETMS.DataAccess.Core;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Common;

namespace ETMS.DataAccess
{
    public class StatisticsSalesProductDAL : DataAccessBase, IStatisticsSalesProductDAL
    {
        public StatisticsSalesProductDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task UpdateStatisticsSales(DateTime date)
        {
            var statisticsSql = $"SELECT ProductType,SUM(ItemAptSum) AS DaySum FROM EtOrderDetail WHERE Ot = '{date.EtmsToDateString()}' AND TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] <> {EmOrderStatus.Repeal} GROUP BY ProductType";
            var myStatisticsSales = await _dbWrapper.ExecuteObject<StatisticsSalesProduct>(statisticsSql);
            var courseSumLog = myStatisticsSales.FirstOrDefault(p => p.ProductType == EmProductType.Course);
            var goodsSumLog = myStatisticsSales.FirstOrDefault(p => p.ProductType == EmProductType.Goods);
            var costSumLog = myStatisticsSales.FirstOrDefault(p => p.ProductType == EmProductType.Cost);
            var myCourseSum = courseSumLog == null ? 0 : courseSumLog.DaySum;
            var myGoodsSum = goodsSumLog == null ? 0 : goodsSumLog.DaySum;
            var myCostSum = costSumLog == null ? 0 : costSumLog.DaySum;
            var hisData = await _dbWrapper.Find<EtStatisticsSalesProduct>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == date);
            if (hisData != null)
            {
                hisData.CourseSum = myCourseSum;
                hisData.GoodsSum = myGoodsSum;
                hisData.CostSum = myCostSum;
                hisData.SalesTotalSum = myCourseSum + myGoodsSum + myCostSum;
                await _dbWrapper.Update(hisData);
            }
            else
            {
                await _dbWrapper.Insert(new EtStatisticsSalesProduct()
                {
                    CourseSum = myCourseSum,
                    GoodsSum = myGoodsSum,
                    CostSum = myCostSum,
                    SalesTotalSum = myCourseSum + myGoodsSum + myCostSum,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = date,
                    TenantId = _tenantId
                });
            }
        }

        public async Task UpdateStatisticsSalesProductMonth(DateTime time)
        {
            var firstDate = new DateTime(time.Year, time.Month, 1);
            var startTimeDesc = firstDate.EtmsToDateString();
            var endTimeDesc = firstDate.AddMonths(1).EtmsToDateString();
            var statisticsSql = $"SELECT ProductType,SUM(ItemAptSum) AS DaySum FROM EtOrderDetail WHERE Ot >= '{startTimeDesc}' AND Ot < '{endTimeDesc}' AND TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] <> {EmOrderStatus.Repeal} GROUP BY ProductType";
            var myStatisticsSales = await _dbWrapper.ExecuteObject<StatisticsSalesProduct>(statisticsSql);
            var courseSumLog = myStatisticsSales.FirstOrDefault(p => p.ProductType == EmProductType.Course);
            var goodsSumLog = myStatisticsSales.FirstOrDefault(p => p.ProductType == EmProductType.Goods);
            var costSumLog = myStatisticsSales.FirstOrDefault(p => p.ProductType == EmProductType.Cost);
            var myCourseSum = courseSumLog == null ? 0 : courseSumLog.DaySum;
            var myGoodsSum = goodsSumLog == null ? 0 : goodsSumLog.DaySum;
            var myCostSum = costSumLog == null ? 0 : costSumLog.DaySum;
            var hisData = await _dbWrapper.Find<EtStatisticsSalesProductMonth>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == firstDate);
            if (hisData != null)
            {
                hisData.CourseSum = myCourseSum;
                hisData.GoodsSum = myGoodsSum;
                hisData.CostSum = myCostSum;
                hisData.SalesTotalSum = myCourseSum + myGoodsSum + myCostSum;
                await _dbWrapper.Update(hisData);
            }
            else
            {
                await _dbWrapper.Insert(new EtStatisticsSalesProductMonth()
                {
                    CourseSum = myCourseSum,
                    GoodsSum = myGoodsSum,
                    CostSum = myCostSum,
                    SalesTotalSum = myCourseSum + myGoodsSum + myCostSum,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = firstDate,
                    TenantId = _tenantId,
                    Month = firstDate.Month,
                    Year = firstDate.Year
                });
            }
        }

        public async Task<List<EtStatisticsSalesProduct>> GetStatisticsSalesProduct(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsSalesProduct>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<List<EtStatisticsSalesProductMonth>> GetEtStatisticsSalesProductMonth(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsSalesProductMonth>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<Tuple<IEnumerable<EtStatisticsSalesProductMonth>, int>> GetEtStatisticsSalesProductMonthPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsSalesProductMonth>("EtStatisticsSalesProductMonth", "*", request.PageSize, request.PageCurrent, "[Ot] DESC", request.ToString());
        }
    }
}
