using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.Business
{
    public class StatisticsSalesBLL : IStatisticsSalesBLL
    {
        private readonly IStatisticsSalesProductDAL _statisticsSalesProductDAL;

        public StatisticsSalesBLL(IStatisticsSalesProductDAL statisticsSalesProductDAL)
        {
            this._statisticsSalesProductDAL = statisticsSalesProductDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsSalesProductDAL);
        }

        public async Task StatisticsSalesProductConsumeEvent(StatisticsSalesProductEvent request)
        {
            await _statisticsSalesProductDAL.UpdateStatisticsSales(request.StatisticsDate.Date);
        }

        public async Task<ResponseBase> GetStatisticsSalesProduct(GetStatisticsSalesProductRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsSalesProductData = await _statisticsSalesProductDAL.GetStatisticsSalesProduct(currentDate, endDate);
            var outPut = new EchartsBarMulti();
            outPut.SourceItems.Add(new List<string>() { "ot", "教程", "物品", "费用" });
            while (currentDate <= endDate)
            {
                var mySourceItem = new List<string>();
                mySourceItem.Add(currentDate.ToString("MM-dd"));
                var myCourseSum = statisticsSalesProductData.FirstOrDefault(p => p.Ot == currentDate);
                if (myCourseSum != null)
                {
                    mySourceItem.Add(myCourseSum.CourseSum.ToString());
                    mySourceItem.Add(myCourseSum.GoodsSum.ToString());
                    mySourceItem.Add(myCourseSum.CostSum.ToString());
                }
                else
                {
                    mySourceItem.Add("0");
                    mySourceItem.Add("0");
                    mySourceItem.Add("0");
                }
                outPut.SourceItems.Add(mySourceItem);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> GetStatisticsSalesProductProportion(GetStatisticsSalesProductProportionRequest request)
        {
            var statisticsSalesProductData = await _statisticsSalesProductDAL.GetStatisticsSalesProduct(request.StartOt.Value, request.EndOt.Value);
            var myTotalCourseSum = statisticsSalesProductData.Sum(p => p.CourseSum);
            var myTotalGoodsSum = statisticsSalesProductData.Sum(p => p.GoodsSum);
            var myCostSum = statisticsSalesProductData.Sum(p => p.CostSum);
            var echartsStatisticsSalesProduct = new EchartsPie<decimal>();
            echartsStatisticsSalesProduct.LegendData.Add("教程");
            echartsStatisticsSalesProduct.MyData.Add(new EchartsPieData<decimal>() { Name = "教程", Value = myTotalCourseSum });
            echartsStatisticsSalesProduct.LegendData.Add("物品");
            echartsStatisticsSalesProduct.MyData.Add(new EchartsPieData<decimal>() { Name = "物品", Value = myTotalGoodsSum });
            echartsStatisticsSalesProduct.LegendData.Add("费用");
            echartsStatisticsSalesProduct.MyData.Add(new EchartsPieData<decimal>() { Name = "费用", Value = myCostSum });
            return ResponseBase.Success(echartsStatisticsSalesProduct);
        }
    }
}
