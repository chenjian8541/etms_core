using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Utility;
using ETMS.IEventProvider;
using ETMS.Event.DataContract.Statistics;

namespace ETMS.Business
{
    public class StatisticsFinanceBLL : IStatisticsFinanceBLL
    {
        private readonly IStatisticsFinanceIncomeDAL _statisticsFinanceIncomeDAL;

        private readonly IIncomeProjectTypeDAL _incomeProjectTypeDAL;

        private readonly IEventPublisher _eventPublisher;

        public StatisticsFinanceBLL(IStatisticsFinanceIncomeDAL statisticsFinanceIncomeDAL, IIncomeProjectTypeDAL incomeProjectTypeDAL,
            IEventPublisher eventPublisher)
        {
            this._statisticsFinanceIncomeDAL = statisticsFinanceIncomeDAL;
            this._incomeProjectTypeDAL = incomeProjectTypeDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsFinanceIncomeDAL, _incomeProjectTypeDAL);
        }

        public async Task StatisticsFinanceIncomeConsumeEvent(StatisticsFinanceIncomeEvent request)
        {
            await _statisticsFinanceIncomeDAL.UpdateStatisticsFinanceIncome(request.StatisticsDate.Date);
            if (!EtmsHelper2.IsThisMonth(request.StatisticsDate.Date))
            {
                _eventPublisher.Publish(new StatisticsFinanceIncomeMonthEvent(request.TenantId)
                {
                    Time = request.StatisticsDate.Date
                });
            }
            _eventPublisher.Publish(new SysTenantStatisticsWeekAndMonthEvent(request.TenantId, StatisticsWeekAndMonthType.Income));
        }

        public async Task<ResponseBase> GetStatisticsFinanceIn(GetStatisticsFinanceInRequest request)
        {
            return ResponseBase.Success(await GetStatisticsFinanceIncome(request.StartOt.Value, request.EndOt.Value, EmIncomeLogType.AccountIn));
        }

        public async Task<ResponseBase> GetStatisticsFinanceInProjectType(GetStatisticsFinanceInProjectTypeRequest request)
        {
            return ResponseBase.Success(await GetStatisticsFinanceIncomeProjectType(request.StartOt.Value, request.EndOt.Value, EmIncomeLogType.AccountIn));
        }

        public async Task<ResponseBase> GetStatisticsFinanceInPayType(GetStatisticsFinanceInPayTypeRequest request)
        {
            return ResponseBase.Success(await GetStatisticsFinanceIncomePayType(request.StartOt.Value, request.EndOt.Value, EmIncomeLogType.AccountIn, request.AgtPayType));
        }

        public async Task<ResponseBase> GetStatisticsFinanceOut(GetStatisticsFinanceOutRequest request)
        {
            return ResponseBase.Success(await GetStatisticsFinanceIncome(request.StartOt.Value, request.EndOt.Value, EmIncomeLogType.AccountOut));
        }

        public async Task<ResponseBase> GetStatisticsFinanceOutProjectType(GetStatisticsFinanceOutProjectTypeRequest request)
        {
            return ResponseBase.Success(await GetStatisticsFinanceIncomeProjectType(request.StartOt.Value, request.EndOt.Value, EmIncomeLogType.AccountOut));
        }

        public async Task<ResponseBase> GetStatisticsFinanceOutPayType(GetStatisticsFinanceOutPayTypeRequest request)
        {
            return ResponseBase.Success(await GetStatisticsFinanceIncomePayType(request.StartOt.Value, request.EndOt.Value, EmIncomeLogType.AccountOut, request.AgtPayType));
        }

        private async Task<EchartsBar<decimal>> GetStatisticsFinanceIncome(DateTime currentDate, DateTime endDate, byte type)
        {
            var statisticsData = await _statisticsFinanceIncomeDAL.GetStatisticsFinanceIncome(currentDate, endDate, type);
            var echartsBar = new EchartsBar<decimal>();
            while (currentDate <= endDate)
            {
                var myTotalSum = statisticsData.Where(p => p.Ot == currentDate).Sum(p => p.TotalSum);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myTotalSum);
                currentDate = currentDate.AddDays(1);
            }
            return echartsBar;
        }

        private async Task<EchartsPie<decimal>> GetStatisticsFinanceIncomeProjectType(DateTime currentDate, DateTime endDate, byte type)
        {
            var statisticsData = await _statisticsFinanceIncomeDAL.GetStatisticsFinanceIncome(currentDate, endDate, type);
            var echartsPieProjectType = new EchartsPie<decimal>();
            if (statisticsData != null && statisticsData.Any())
            {
                var newProjectTypeStatisticsData = statisticsData.GroupBy(p => p.ProjectType).Select(y => new { MyProjectType = y.Key, MyTotalSum = y.Sum(p => p.TotalSum) });
                var allProjectType = await _incomeProjectTypeDAL.GetAllIncomeProjectType();
                const string otherTagName = "其他";
                var echartsOther = new EchartsPieData<decimal>() { Name = otherTagName, Value = 0 };
                foreach (var item in newProjectTypeStatisticsData)
                {
                    var tempName = EmIncomeLogProjectType.GetIncomeLogProjectType(allProjectType, item.MyProjectType);
                    if (string.IsNullOrEmpty(tempName))
                    {
                        echartsOther.Value += item.MyTotalSum;
                        continue;
                    }
                    echartsPieProjectType.LegendData.Add(tempName);
                    echartsPieProjectType.MyData.Add(new EchartsPieData<decimal>()
                    {
                        Name = tempName,
                        Value = item.MyTotalSum
                    });
                }
                if (echartsOther.Value > 0)
                {
                    echartsPieProjectType.LegendData.Add(otherTagName);
                    echartsPieProjectType.MyData.Add(echartsOther);
                }
            }
            return echartsPieProjectType;
        }

        private async Task<EchartsPie<decimal>> GetStatisticsFinanceIncomePayType(DateTime currentDate, DateTime endDate,
            byte type, int agtPayType)
        {
            var statisticsData = await _statisticsFinanceIncomeDAL.GetStatisticsFinanceIncome(currentDate, endDate, type);
            var echartsPiePayType = new EchartsPie<decimal>();
            if (statisticsData != null && statisticsData.Any())
            {
                var newPayTypeStatisticsData = statisticsData.GroupBy(p => p.PayType).Select(y => new { MyPayType = y.Key, MyTotalSum = y.Sum(p => p.TotalSum) });
                var allPayType = await _incomeProjectTypeDAL.GetAllIncomeProjectType();
                const string otherTagName = "其他";
                var echartsOther = new EchartsPieData<decimal>() { Name = otherTagName, Value = 0 };
                foreach (var item in newPayTypeStatisticsData)
                {
                    var tempName = EmPayType.GetPayType(item.MyPayType, agtPayType);
                    if (string.IsNullOrEmpty(tempName))
                    {
                        echartsOther.Value += item.MyTotalSum;
                        continue;
                    }
                    echartsPiePayType.LegendData.Add(tempName);
                    echartsPiePayType.MyData.Add(new EchartsPieData<decimal>()
                    {
                        Name = tempName,
                        Value = item.MyTotalSum
                    });
                }
                if (echartsOther.Value > 0)
                {
                    echartsPiePayType.LegendData.Add(otherTagName);
                    echartsPiePayType.MyData.Add(echartsOther);
                }
            }
            return echartsPiePayType;
        }

        public async Task<ResponseBase> GetStatisticsFinanceIncomeMonth(GetStatisticsFinanceIncomeMonthRequest request)
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
            var statisticsMonth = await _statisticsFinanceIncomeDAL.GetStatisticsFinanceIncomeMonth(startTime, endTime, request.Type);
            var echartsBar = new EchartsBar<decimal>();
            var index = 1;
            while (index <= 12)
            {
                var myStatisticsMonth = statisticsMonth.Where(p => p.Month == index).Sum(p => p.TotalSum);
                echartsBar.XData.Add($"{index}月");
                echartsBar.MyData.Add(myStatisticsMonth);
                index++;
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> GetStatisticsFinanceIncomeMonthPaging(GetStatisticsFinanceIncomeMonthPagingRequest request)
        {
            var pagingData = await _statisticsFinanceIncomeDAL.GetStatisticsFinanceIncomeMonthPaging(request);
            var output = new List<GetStatisticsFinanceIncomeMonthPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var allProjectType = await _incomeProjectTypeDAL.GetAllIncomeProjectType();
                foreach (var p in pagingData.Item1)
                {
                    var projectTypeDesc = EmIncomeLogProjectType.GetIncomeLogProjectType(allProjectType, p.ProjectType);
                    output.Add(new GetStatisticsFinanceIncomeMonthPagingOutput()
                    {
                        ProjectType = p.ProjectType,
                        ProjectTypeDesc = projectTypeDesc,
                        Month = p.Month,
                        Ot = p.Ot,
                        TotalCount = p.TotalCount,
                        TotalSum = p.TotalSum,
                        Type = p.Type,
                        Year = p.Year
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<GetStatisticsFinanceIncomeMonthPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> GetStatisticsFinanceIncomeYear(GetStatisticsFinanceIncomeYearRequest request)
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

            var value = 0M;
            var statisticsFinanceIncomeYearBucket = await _statisticsFinanceIncomeDAL.GetStatisticsFinanceIncomeYear(year);
            if (statisticsFinanceIncomeYearBucket != null)
            {
                if (request.Type == EmIncomeLogType.AccountIn)
                {
                    value = statisticsFinanceIncomeYearBucket.TotalSumIn;
                }
                else
                {
                    value = statisticsFinanceIncomeYearBucket.TotalSumOut;
                }
            }
            return ResponseBase.Success(new GetStatisticsFinanceIncomeYearOutput()
            {
                FinanceIncomeValue = value.ToString("F2")
            });
        }
    }
}
