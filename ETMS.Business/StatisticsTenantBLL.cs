using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.Utility;
using ETMS.IDataAccess.Statistics;

namespace ETMS.Business
{
    public class StatisticsTenantBLL : IStatisticsTenantBLL
    {
        private readonly IStatisticsFinanceIncomeDAL _statisticsFinanceIncomeDAL;

        private readonly IStatisticsStudentCountDAL _statisticsStudentCountDAL;

        private readonly IStatisticsClassAttendanceTagDAL _statisticsClassAttendanceTagDAL;

        private readonly IStatisticsClassDAL _statisticsClassDAL;

        private readonly ITenantToDoThingDAL _tenantToDoThingDAL;

        private readonly IStatisticsStudentAccountRechargeDAL _statisticsStudentAccountRechargeDAL;

        private readonly IStatisticsLcsPayDAL _statisticsLcsPayDAL;

        public StatisticsTenantBLL(IStatisticsFinanceIncomeDAL statisticsFinanceIncomeDAL, IStatisticsStudentCountDAL statisticsStudentCountDAL,
            IStatisticsClassAttendanceTagDAL statisticsClassAttendanceTagDAL, IStatisticsClassDAL statisticsClassDAL, ITenantToDoThingDAL tenantToDoThingDAL,
            IStatisticsStudentAccountRechargeDAL statisticsStudentAccountRechargeDAL, IStatisticsLcsPayDAL statisticsLcsPayDAL)
        {
            this._statisticsFinanceIncomeDAL = statisticsFinanceIncomeDAL;
            this._statisticsStudentCountDAL = statisticsStudentCountDAL;
            this._statisticsClassAttendanceTagDAL = statisticsClassAttendanceTagDAL;
            this._statisticsClassDAL = statisticsClassDAL;
            this._tenantToDoThingDAL = tenantToDoThingDAL;
            this._statisticsStudentAccountRechargeDAL = statisticsStudentAccountRechargeDAL;
            this._statisticsLcsPayDAL = statisticsLcsPayDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsFinanceIncomeDAL, _statisticsStudentCountDAL, _statisticsClassAttendanceTagDAL,
                _statisticsClassDAL, _tenantToDoThingDAL, _statisticsStudentAccountRechargeDAL, _statisticsLcsPayDAL);
        }

        public async Task<ResponseBase> StatisticsTenantGet(StatisticsTenantGetRequest request)
        {
            var now = DateTime.Now.Date;
            var startTime = new DateTime(now.Year, now.Month, 1);
            var endTime = startTime.AddMonths(1).AddDays(-1);
            var accountTotal = await _statisticsFinanceIncomeDAL.GetStatisticsFinanceIncome(startTime, endTime);
            var studentCount = await _statisticsStudentCountDAL.GetStatisticsStudentCount(startTime, endTime);
            var statisticsClassTimes = await _statisticsClassDAL.StatisticsClassTimesGet(startTime, endTime);
            var classAttendanceTag = await _statisticsClassAttendanceTagDAL.GetStatisticsClassAttendanceTag(now, now);
            var classActuallyCountToday = 0;
            var classLeaveCountToday = 0;
            var classNotArrivedToday = 0;
            if (classAttendanceTag != null && classAttendanceTag.Any())
            {
                foreach (var p in classAttendanceTag)
                {
                    switch (p.StudentCheckStatus)
                    {
                        case EmClassStudentCheckStatus.Arrived:
                            classActuallyCountToday += p.TotalCount;
                            break;
                        case EmClassStudentCheckStatus.BeLate:
                            classActuallyCountToday += p.TotalCount;
                            break;
                        case EmClassStudentCheckStatus.Leave:
                            classLeaveCountToday += p.TotalCount;
                            break;
                        case EmClassStudentCheckStatus.NotArrived:
                            classNotArrivedToday += p.TotalCount;
                            break;
                    }
                }
            }
            var classTimesThisMonth = 0M;
            var deSumThisMonth = 0M;
            if (statisticsClassTimes.Any())
            {
                foreach (var p in statisticsClassTimes)
                {
                    classTimesThisMonth += p.ClassTimes;
                    deSumThisMonth += p.DeSum;
                }
            }
            var incomeThisMonth = 0M;
            var incomeOutThisMonth = 0M;
            if (accountTotal.Any())
            {
                foreach (var p in accountTotal)
                {
                    if (p.Type == EmIncomeLogType.AccountIn)
                    {
                        incomeThisMonth += p.TotalSum;
                    }
                    else
                    {
                        incomeOutThisMonth += p.TotalSum;
                    }
                }
            }
            var output = new StatisticsTenantGetOutput()
            {
                IncomeThisMonth = incomeThisMonth,
                IncomeOutThisMonth = incomeOutThisMonth,
                AddStudentThisMonth = studentCount.Sum(p => p.AddStudentCount),
                ClassActuallyCountToday = classActuallyCountToday,
                ClassLeaveCountToday = classLeaveCountToday,
                ClassNotArrivedToday = classNotArrivedToday,
                ClassTimesThisMonth = classTimesThisMonth.EtmsToString(),
                DeSumThisMonth = deSumThisMonth,
                IsDataLimit = request.GetIsDataLimit()
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TenantToDoThingGet(TenantToDoThingGetRequest request)
        {
            var data = await _tenantToDoThingDAL.GetTenantToDoThing();
            return ResponseBase.Success(new TenantToDoThingGetOutput()
            {
                ClassNotScheduled = data.ClassNotScheduled,
                ClassRecordAbsent = data.ClassRecordAbsent,
                ClassTimesTimeOutNotCheckSign = data.ClassTimesTimeOutNotCheckSign,
                GoodsInventoryNotEnough = data.GoodsInventoryNotEnough,
                StudentCourseNotEnough = data.StudentCourseNotEnough,
                StudentLeaveApplyLogCount = data.StudentLeaveApplyLogCount,
                StudentOrderArrears = data.StudentOrderArrears,
                TryCalssApplyLogCount = data.TryCalssApplyLogCount
            });
        }

        public async Task ResetTenantToDoThingConsumerEvent(ResetTenantToDoThingEvent request)
        {
            await _tenantToDoThingDAL.ResetTenantToDoThing();
        }

        public async Task StatisticsStudentAccountRechargeConsumerEvent(StatisticsStudentAccountRechargeEvent request)
        {
            await _statisticsStudentAccountRechargeDAL.UpdateStatisticsStudentAccountRecharge();
        }

        public async Task StatisticsLcsPayConsumerEvent(StatisticsLcsPayEvent request)
        {
            await _statisticsLcsPayDAL.UpdateStatisticsLcsPayDay(request.StatisticsDate);
            await _statisticsLcsPayDAL.UpdateStatisticsLcsPayMonth(request.StatisticsDate);
        }
    }
}
