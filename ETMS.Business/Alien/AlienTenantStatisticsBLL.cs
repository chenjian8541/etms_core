using ETMS.Entity.Alien.Dto.TenantStatistics.Output;
using ETMS.Entity.Alien.Dto.TenantStatistics.Request;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.View.Persistence;
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

        private readonly IStatisticsStudentCountDAL _statisticsStudentCountDAL;

        private readonly IStatisticsStudentTrackCountDAL _statisticsStudentTrackCountDAL;

        private readonly IStudentSourceDAL _studentSourceDAL;

        private readonly IStatisticsStudentSourceDAL _statisticsStudentSourceDAL;

        private readonly IStatisticsStudentDAL _statisticsStudentDAL;

        public AlienTenantStatisticsBLL(IStatisticsFinanceIncomeDAL statisticsFinanceIncomeDAL, IIncomeProjectTypeDAL incomeProjectTypeDAL, IIncomeLogDAL incomeLogDAL,
            IStatisticsStudentCountDAL statisticsStudentCountDAL, IStatisticsStudentTrackCountDAL statisticsStudentTrackCountDAL,
            IStudentSourceDAL studentSourceDAL, IStatisticsStudentSourceDAL statisticsStudentSourceDAL,
            IStatisticsStudentDAL statisticsStudentDAL)
        {
            this._statisticsFinanceIncomeDAL = statisticsFinanceIncomeDAL;
            this._incomeProjectTypeDAL = incomeProjectTypeDAL;
            this._incomeLogDAL = incomeLogDAL;
            this._statisticsStudentCountDAL = statisticsStudentCountDAL;
            this._statisticsStudentTrackCountDAL = statisticsStudentTrackCountDAL;
            this._studentSourceDAL = studentSourceDAL;
            this._statisticsStudentSourceDAL = statisticsStudentSourceDAL;
            this._statisticsStudentDAL = statisticsStudentDAL;
        }

        public void InitHeadId(int headId)
        {
        }

        public void InitTenant(int tenantId)
        {
            this.InitTenantDataAccess(tenantId, _statisticsFinanceIncomeDAL, _incomeProjectTypeDAL,
                _incomeLogDAL, _statisticsStudentCountDAL, _statisticsStudentTrackCountDAL,
                _studentSourceDAL, _statisticsStudentSourceDAL, _statisticsStudentDAL);
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

        public async Task<ResponseBase> AlTenantStatisticsStudentCountGet(AlTenantStatisticsStudentCountGetRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsStudentCount = await _statisticsStudentCountDAL.GetStatisticsStudentCount(currentDate, endDate);
            var echartsBar = new EchartsBar<int>();
            while (currentDate <= endDate)
            {
                var myStatisticsStudentCount = statisticsStudentCount.FirstOrDefault(p => p.Ot == currentDate);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myStatisticsStudentCount == null ? 0 : myStatisticsStudentCount.AddStudentCount);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentTrackCountGet(AlTenantStatisticsStudentTrackCountGetRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsStudentTrackCount = await _statisticsStudentTrackCountDAL.GetStatisticsStudentTrackCount(currentDate, endDate);
            var echartsBar = new EchartsBar<int>();
            while (currentDate <= endDate)
            {
                var myStatisticsStudentTrackCount = statisticsStudentTrackCount.FirstOrDefault(p => p.Ot == currentDate);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myStatisticsStudentTrackCount == null ? 0 : myStatisticsStudentTrackCount.TrackCount);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentSourceGet(AlTenantStatisticsStudentSourceGetRequest request)
        {
            var statisticsStudentDataStudentSource = await _statisticsStudentSourceDAL.GetStatisticsStudentSource(request.StartOt.Value, request.EndOt.Value);
            var studentSourceAll = await _studentSourceDAL.GetAllStudentSource();
            var echartsPieStudentSource = new EchartsPie<int>();
            const string otherTagName = "其他";
            var echartsPieDataStudentSourceOther = new EchartsPieData<int>() { Name = otherTagName, Value = 0 };
            foreach (var p in statisticsStudentDataStudentSource)
            {
                if (p.SourceId == null)
                {
                    echartsPieDataStudentSourceOther.Value += p.TotalCount;
                    continue;
                }
                var mySource = studentSourceAll.FirstOrDefault(j => j.Id == p.SourceId.Value);
                if (mySource == null)
                {
                    echartsPieDataStudentSourceOther.Value += p.TotalCount;
                    continue;
                }
                echartsPieStudentSource.LegendData.Add(mySource.Name);
                echartsPieStudentSource.MyData.Add(new EchartsPieData<int>()
                {
                    Name = mySource.Name,
                    Value = p.TotalCount
                });
            }
            if (echartsPieDataStudentSourceOther.Value > 0)
            {
                echartsPieStudentSource.LegendData.Add(otherTagName);
                echartsPieStudentSource.MyData.Add(echartsPieDataStudentSourceOther);
            }
            return ResponseBase.Success(echartsPieStudentSource);
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentTypeGet(AlTenantStatisticsStudentTypeGetRequest request)
        {
            var statisticsStudentDataStudentType = await _statisticsStudentDAL.GetStatisticsStudent((int)EmStatisticsStudentType.StudentType);
            var statisticsStudentType = new List<StatisticsStudentType>();
            if (statisticsStudentDataStudentType != null && !string.IsNullOrEmpty(statisticsStudentDataStudentType.ContentData))
            {
                statisticsStudentType = EtmsHelper.EtmsDeserializeObject<List<StatisticsStudentType>>(statisticsStudentDataStudentType.ContentData);
            }
            var echartsPieStudentType = new EchartsPie<int>();
            var allType = EmStudentType.GetAllStudentType();
            foreach (var p in allType)
            {
                var tempName = EmStudentType.GetStudentTypeDesc(p);
                var myData = statisticsStudentType.FirstOrDefault(j => j.StudentType == p);
                if (myData == null)
                {
                    continue;
                }
                echartsPieStudentType.LegendData.Add(tempName);
                echartsPieStudentType.MyData.Add(new EchartsPieData<int>()
                {
                    Name = tempName,
                    Value = myData.TotalCount
                });
            }
            return ResponseBase.Success(echartsPieStudentType);
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentCountPagingGet(AlTenantStatisticsStudentCountPagingGetRequest request)
        {
            var pagingData = await _statisticsStudentCountDAL.GetStatisticsStudentCountPaging(request);
            var output = new List<AlTenantStatisticsStudentCountPagingGetOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new AlTenantStatisticsStudentCountPagingGetOutput()
                {
                    AddStudentCount = p.AddStudentCount,
                    Id = p.Id,
                    Ot = p.Ot
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AlTenantStatisticsStudentCountPagingGetOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentCountMonthGet(AlTenantStatisticsStudentCountMonthGetRequest request)
        {
            DateTime startTime;
            DateTime endTime;
            if (request.Year != null)
            {
                startTime = new DateTime(request.Year.Value, 1, 1);
                endTime = startTime.AddYears(1).AddDays(-1);
            }
            else
            {
                startTime = new DateTime(DateTime.Now.Year, 1, 1);
                endTime = startTime.AddYears(1).AddDays(-1);
            }
            var statisticsStudentCountMonth = await _statisticsStudentCountDAL.GetStatisticsStudentCountMonth(startTime, endTime);
            var echartsBar = new EchartsBar<int>();
            var index = 1;
            while (index <= 12)
            {
                var myStatisticsStudentCount = statisticsStudentCountMonth.FirstOrDefault(p => p.Month == index);
                echartsBar.XData.Add($"{index}月");
                echartsBar.MyData.Add(myStatisticsStudentCount == null ? 0 : myStatisticsStudentCount.AddStudentCount);
                index++;
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentCountMonthPagingGet(AlTenantStatisticsStudentCountMonthPagingGetRequest request)
        {
            var pagingData = await _statisticsStudentCountDAL.GetStatisticsStudentCountMonthPaging(request);
            var output = new List<AlTenantStatisticsStudentCountMonthPagingGetOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new AlTenantStatisticsStudentCountMonthPagingGetOutput()
                {
                    Id = p.Id,
                    AddStudentCount = p.AddStudentCount,
                    Month = p.Month,
                    Year = p.Year
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AlTenantStatisticsStudentCountMonthPagingGetOutput>(pagingData.Item2, output));
        }
    }
}
