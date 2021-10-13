using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Utility;

namespace ETMS.Business
{
    public class PaymentStatisticsBLL : IPaymentStatisticsBLL
    {
        private readonly IStatisticsLcsPayDAL _statisticsLcsPayDAL;

        public PaymentStatisticsBLL(IStatisticsLcsPayDAL statisticsLcsPayDAL)
        {
            this._statisticsLcsPayDAL = statisticsLcsPayDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsLcsPayDAL);
        }

        public async Task<ResponseBase> StatisticsLcsPayDayGetLine(StatisticsLcsPayDayGetLineRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsData = await _statisticsLcsPayDAL.GetStatisticsLcsPayDay(currentDate, endDate);
            var echartsBar = new EchartsBar<decimal>();
            while (currentDate <= endDate)
            {
                var myTotalSum = statisticsData.Where(p => p.Ot == currentDate).Sum(p => p.TotalMoneyValue);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myTotalSum);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> StatisticsLcsPayDayGetPaging(StatisticsLcsPayDayGetPagingRequest request)
        {
            var pagingData = await _statisticsLcsPayDAL.GetStatisticsLcsPayDayPaging(request);
            var output = new List<StatisticsLcsPayDayGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new StatisticsLcsPayDayGetPagingOutput()
                    {
                        OtDesc = p.Ot.EtmsToDateString(),
                        TotalMoney = p.TotalMoney,
                        TotalMoneyRefund = p.TotalMoneyRefund,
                        TotalMoneyValue = p.TotalMoneyValue
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StatisticsLcsPayDayGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StatisticsLcsPayMonthGetLine(StatisticsLcsPayMonthGetLineRequest request)
        {
            DateTime startTime;
            DateTime endTime;
            if (request.Year != null && request.Year > 0)
            {
                startTime = new DateTime(request.Year.Value, 1, 1);
                endTime = startTime.AddYears(1).AddDays(-1);
            }
            else
            {
                startTime = new DateTime(DateTime.Now.Year, 1, 1);
                endTime = startTime.AddYears(1).AddDays(-1);
            }
            var statisticsMonth = await _statisticsLcsPayDAL.GetStatisticsLcsPayMonth(startTime, endTime);
            var echartsBar = new EchartsBar<decimal>();
            var index = 1;
            while (index <= 12)
            {
                var myStatisticsMonth = statisticsMonth.Where(p => p.Month == index).Sum(p => p.TotalMoneyValue);
                echartsBar.XData.Add($"{index}月");
                echartsBar.MyData.Add(myStatisticsMonth);
                index++;
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> StatisticsLcsPayMonthGetPaging(StatisticsLcsPayMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsLcsPayDAL.GetStatisticsLcsPayMonthPaging(request);
            var output = new List<StatisticsLcsPayMonthGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new StatisticsLcsPayMonthGetPagingOutput()
                    {
                        TotalMoney = p.TotalMoney,
                        TotalMoneyRefund = p.TotalMoneyRefund,
                        TotalMoneyValue = p.TotalMoneyValue,
                        Month = p.Month,
                        Year = p.Year,
                        Ot = p.Ot
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StatisticsLcsPayMonthGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StatisticsLcsPayYear(StatisticsLcsPayYearRequest request)
        {
            int year;
            if (request.Year != null)
            {
                year = request.Year.Value;
            }
            else
            {
                year = DateTime.Now.Year;
            }

            var bucket = await _statisticsLcsPayDAL.GetStatisticsLcsPayYear(year);
            if (bucket != null)
            {
                return ResponseBase.Success(new StatisticsLcsPayYearOutput()
                {
                    TotalMoney = bucket.TotalMoney,
                    TotalMoneyRefund = bucket.TotalMoneyRefund,
                    TotalMoneyValue = bucket.TotalMoneyValue
                });
            }
            return ResponseBase.Success(new StatisticsLcsPayYearOutput());
        }
    }
}
