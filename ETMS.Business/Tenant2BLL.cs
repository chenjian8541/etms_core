using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage.Statistics;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class Tenant2BLL : ITenant2BLL
    {
        private readonly ISysTenantStatistics2DAL _sysTenantStatistics2DAL;

        private readonly ISysTenantStatisticsWeekDAL _sysTenantStatisticsWeekDAL;

        private readonly ISysTenantStatisticsMonthDAL _sysTenantStatisticsMonthDAL;

        private readonly ITenantConfig2DAL _tenantConfig2DAL;
        public Tenant2BLL(ISysTenantStatistics2DAL sysTenantStatistics2DAL, ISysTenantStatisticsWeekDAL sysTenantStatisticsWeekDAL,
            ISysTenantStatisticsMonthDAL sysTenantStatisticsMonthDAL, ITenantConfig2DAL tenantConfig2DAL)
        {
            this._sysTenantStatistics2DAL = sysTenantStatistics2DAL;
            this._sysTenantStatisticsWeekDAL = sysTenantStatisticsWeekDAL;
            this._sysTenantStatisticsMonthDAL = sysTenantStatisticsMonthDAL;
            this._tenantConfig2DAL = tenantConfig2DAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _tenantConfig2DAL);
        }

        public async Task<ResponseBase> TenantStatisticsGet(RequestBase request)
        {
            var output = new TenantStatisticsGetOutput();
            var statistics2 = await _sysTenantStatistics2DAL.GetSysTenantStatistics(request.LoginTenantId);
            if (statistics2 != null)
            {
                output.BascInfo = new TenantStatisticsGetBascInfo()
                {
                    StudentHistoryCount = statistics2.StudentHistoryCount,
                    StudentPotentialCount = statistics2.StudentPotentialCount,
                    StudentReadCount = statistics2.StudentReadCount,
                    TeacherCount = statistics2.TeacherCount,
                    TenantSurplusSurplusMoney = statistics2.TenantSurplusSurplusMoney,
                    TenantSurplusClassTimesDesc = statistics2.TenantSurplusClassTimes.EtmsToString()
                };
            }
            else
            {
                output.BascInfo = new TenantStatisticsGetBascInfo();
            }

            var statisticsWeek = await _sysTenantStatisticsWeekDAL.GetSysTenantStatisticsWeek(request.LoginTenantId);
            if (statisticsWeek != null)
            {
                output.WeekInfo = new TenantStatisticsGetWeek()
                {
                    BuyCourseCountLast = statisticsWeek.BuyCourseCountLast,
                    BuyCourseCountThis = statisticsWeek.BuyCourseCountThis,
                    BuyCourseSumLast = statisticsWeek.BuyCourseSumLast,
                    BuyCourseSumThis = statisticsWeek.BuyCourseSumThis,
                    ClassDeSumLast = statisticsWeek.ClassDeSumLast,
                    ClassDeSumThis = statisticsWeek.ClassDeSumThis,
                    ClassDeTimesLast = statisticsWeek.ClassDeTimesLast,
                    ClassDeTimesThis = statisticsWeek.ClassDeTimesThis,
                    ExpensesLast = statisticsWeek.ExpensesLast,
                    ExpensesThis = statisticsWeek.ExpensesThis,
                    IncomeLast = statisticsWeek.IncomeLast,
                    IncomeThis = statisticsWeek.IncomeThis
                };
                output.WeekInfo.TotalIncomeCompareValue = ComBusiness5.GetCompareValue(statisticsWeek.IncomeLast, statisticsWeek.IncomeThis);
                output.WeekInfo.TotalExpensesCompareValue = ComBusiness5.GetCompareValue(statisticsWeek.ExpensesLast, statisticsWeek.ExpensesThis);
                output.WeekInfo.TotalClassDeSumCompareValue = ComBusiness5.GetCompareValue(statisticsWeek.ClassDeSumLast, statisticsWeek.ClassDeSumThis);
                output.WeekInfo.TotalBuyCourseCountCompareValue = ComBusiness5.GetCompareValue(statisticsWeek.BuyCourseCountLast, statisticsWeek.BuyCourseCountThis);
            }
            else
            {
                output.WeekInfo = new TenantStatisticsGetWeek();
            }

            var statisticsMonth = await _sysTenantStatisticsMonthDAL.GetSysTenantStatisticsMonth(request.LoginTenantId);
            if (statisticsMonth != null)
            {
                output.MonthInfo = new TenantStatisticsGetMonth()
                {
                    IncomeThis = statisticsMonth.IncomeThis,
                    BuyCourseSumThis = statisticsMonth.BuyCourseSumThis,
                    IncomeLast = statisticsMonth.IncomeLast,
                    ExpensesThis = statisticsMonth.ExpensesThis,
                    ExpensesLast = statisticsMonth.ExpensesLast,
                    ClassDeTimesThis = statisticsMonth.ClassDeTimesThis,
                    ClassDeTimesLast = statisticsMonth.ClassDeTimesLast,
                    BuyCourseCountLast = statisticsMonth.BuyCourseCountLast,
                    BuyCourseCountThis = statisticsMonth.BuyCourseCountThis,
                    BuyCourseSumLast = statisticsMonth.BuyCourseSumLast,
                    ClassDeSumLast = statisticsMonth.ClassDeSumLast,
                    ClassDeSumThis = statisticsMonth.ClassDeSumThis
                };
                output.MonthInfo.TotalIncomeCompareValue = ComBusiness5.GetCompareValue(statisticsMonth.IncomeLast, statisticsMonth.IncomeThis);
                output.MonthInfo.TotalExpensesCompareValue = ComBusiness5.GetCompareValue(statisticsMonth.ExpensesLast, statisticsMonth.ExpensesThis);
                output.MonthInfo.TotalClassDeSumCompareValue = ComBusiness5.GetCompareValue(statisticsMonth.ClassDeSumLast, statisticsMonth.ClassDeSumThis);
                output.MonthInfo.TotalBuyCourseCountCompareValue = ComBusiness5.GetCompareValue(statisticsMonth.BuyCourseCountLast, statisticsMonth.BuyCourseCountThis);
            }
            else
            {
                output.MonthInfo = new TenantStatisticsGetMonth();
            }

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActivityConfigGet(RequestBase request)
        {
            var tenantConfig = await _tenantConfig2DAL.GetTenantConfig();
            return ResponseBase.Success(new ActivityConfigGetOutput()
            {
                IsAutoRefund = tenantConfig.ActivityConfig.IsAutoRefund,
                PayTp = tenantConfig.ActivityConfig.PayTp,
            });
        }

        public async Task<ResponseBase> ActivityConfigSave(ActivityConfigSaveRequest request)
        {
            var tenantConfig = await _tenantConfig2DAL.GetTenantConfig();
            tenantConfig.ActivityConfig.IsAutoRefund = request.IsAutoRefund;
            tenantConfig.ActivityConfig.PayTp = request.PayTp;
            await _tenantConfig2DAL.SaveTenantConfig(tenantConfig);
            return ResponseBase.Success();
        }

        public ResponseBase TenantCustomized(RequestBase request)
        {
            var output = new TenantCustomizedOutput()
            {
                IsShow12104 = request.LoginTenantId == 12104
            };
            return ResponseBase.Success(output);
        }
    }
}
