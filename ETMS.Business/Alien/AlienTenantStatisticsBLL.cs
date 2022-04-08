using ETMS.Entity.Alien.Dto.TenantStatistics.Output;
using ETMS.Entity.Alien.Dto.TenantStatistics.Request;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Enum;
using ETMS.IBusiness.Alien;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    public class AlienTenantStatisticsBLL : IAlienTenantStatisticsBLL
    {
        private readonly IStatisticsFinanceIncomeDAL _statisticsFinanceIncomeDAL;

        private readonly IIncomeProjectTypeDAL _incomeProjectTypeDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;

        public AlienTenantStatisticsBLL(IStatisticsFinanceIncomeDAL statisticsFinanceIncomeDAL,
            IIncomeProjectTypeDAL incomeProjectTypeDAL, IIncomeLogDAL incomeLogDAL)
        {
            this._statisticsFinanceIncomeDAL = statisticsFinanceIncomeDAL;
            this._incomeProjectTypeDAL = incomeProjectTypeDAL;
            this._incomeLogDAL = incomeLogDAL;
        }

        public void InitHeadId(int headId)
        {
        }

        public void InitTenant(int tenantId)
        {
            this.InitTenantDataAccess(tenantId, _statisticsFinanceIncomeDAL, _incomeProjectTypeDAL,
                _incomeLogDAL);
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

        public async Task<ResponseBase> AlTenantStatisticsFinanceInGet(AlTenantStatisticsFinanceInGetRequest request)
        {
            return ResponseBase.Success(await GetStatisticsFinanceIncome(request.StartOt.Value, request.EndOt.Value, EmIncomeLogType.AccountIn));
        }

        public async Task<ResponseBase> AlTenantStatisticsFinanceOutGet(AlTenantStatisticsFinanceOutGetRequest request)
        {
            return ResponseBase.Success(await GetStatisticsFinanceIncome(request.StartOt.Value, request.EndOt.Value, EmIncomeLogType.AccountOut));
        }

        public async Task<ResponseBase> AlTenantStatisticsFinanceIncomeYearGet(AlTenantStatisticsFinanceIncomeYearGetRequest request)
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
            return ResponseBase.Success(new AlTenantStatisticsFinanceIncomeYearGetOutput()
            {
                FinanceIncomeValue = value.ToString("F2")
            });
        }

        public async Task<ResponseBase> AlTenantStatisticsFinanceIncomeMonthGetPaging(AlTenantStatisticsFinanceIncomeMonthGetPagingRequest request)
        {
            var pagingData = await _statisticsFinanceIncomeDAL.GetStatisticsFinanceIncomeMonthPaging(request);
            var output = new List<AlTenantStatisticsFinanceIncomeMonthGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var allProjectType = await _incomeProjectTypeDAL.GetAllIncomeProjectType();
                foreach (var p in pagingData.Item1)
                {
                    var projectTypeDesc = EmIncomeLogProjectType.GetIncomeLogProjectType(allProjectType, p.ProjectType);
                    output.Add(new AlTenantStatisticsFinanceIncomeMonthGetPagingOutput()
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
            return ResponseBase.Success(new ResponsePagingDataBase<AlTenantStatisticsFinanceIncomeMonthGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AlTenantStatisticsFinanceIncomeMonthGet(AlTenantStatisticsFinanceIncomeMonthGetRequest request)
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

        public async Task<ResponseBase> AlTenantIncomeLogGetPaging(AlTenantIncomeLogGetPagingRequest request)
        {
            var pagingData = await _incomeLogDAL.GetIncomeLogPaging(request);
            var output = new List<AlTenantIncomeLogGetPagingRequestOutput>();
            var allProjectType = await _incomeProjectTypeDAL.GetAllIncomeProjectType();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new AlTenantIncomeLogGetPagingRequestOutput()
                {
                    AccountNo = p.AccountNo,
                    CId = p.Id,
                    CreateOt = p.CreateOt,
                    No = p.No,
                    OrderId = p.OrderId,
                    Ot = p.Ot,
                    OtDesc = p.Ot.EtmsToDateString(),
                    ProjectType = p.ProjectType,
                    ProjectTypeDesc = EmIncomeLogProjectType.GetIncomeLogProjectType(allProjectType, p.ProjectType),
                    Remark = p.Remark,
                    Status = p.Status,
                    Sum = p.Sum,
                    Type = p.Type,
                    TypeDesc = EmIncomeLogType.GetIncomeLogType(p.Type),
                    StatusDesc = EmIncomeLogStatus.GetIncomeLogStatusDesc(p.Status)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AlTenantIncomeLogGetPagingRequestOutput>(pagingData.Item2, output));
        }
    }
}
