using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.SysOp;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETMS.WebApi.Controllers
{
    [Route("api/hisData/[action]")]
    [ApiController]
    [Authorize]
    public class HisDataController : ControllerBase
    {
        private readonly IOrderBLL _orderBLL;

        private readonly IIncomeLogBLL _incomeLogBLL;

        private readonly IStatisticsStudentBLL _statisticsStudentBLL;

        private readonly IStatisticsSalesBLL _statisticsSalesBLL;

        private readonly IStatisticsFinanceBLL _statisticsFinanceBLL;

        private readonly IStatisticsClassBLL _statisticsClassBLL;

        private readonly IStatisticsSalesCourseBLL _statisticsSalesCourseBLL;

        private readonly IStatisticsTenantBLL _statisticsTenantBLL;

        private readonly ISysDataClearBLL _sysDataClearBLL;

        public HisDataController(IOrderBLL orderBLL, IIncomeLogBLL incomeLogBLL, IStatisticsStudentBLL statisticsStudentBLL, IStatisticsSalesBLL statisticsSalesBLL,
            IStatisticsFinanceBLL statisticsFinanceBLL, IStatisticsClassBLL statisticsClassBLL, IStatisticsSalesCourseBLL statisticsSalesCourseBLL,
            IStatisticsTenantBLL statisticsTenantBLL, ISysDataClearBLL sysDataClearBLL)
        {
            this._orderBLL = orderBLL;
            this._incomeLogBLL = incomeLogBLL;
            this._statisticsStudentBLL = statisticsStudentBLL;
            this._statisticsSalesBLL = statisticsSalesBLL;
            this._statisticsFinanceBLL = statisticsFinanceBLL;
            this._statisticsClassBLL = statisticsClassBLL;
            this._statisticsSalesCourseBLL = statisticsSalesCourseBLL;
            this._statisticsTenantBLL = statisticsTenantBLL;
            this._sysDataClearBLL = sysDataClearBLL;
        }

        public async Task<ResponseBase> OrderGetPaging(OrderGetPagingRequest request)
        {
            try
            {
                _orderBLL.InitTenantId(request.LoginTenantId);
                return await _orderBLL.OrderGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrderGetDetail(OrderGetDetailRequest request)
        {
            try
            {
                _orderBLL.InitTenantId(request.LoginTenantId);
                return await _orderBLL.OrderGetDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrderPayment(OrderPaymentRequest request)
        {
            try
            {
                _orderBLL.InitTenantId(request.LoginTenantId);
                return await _orderBLL.OrderPayment(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrderGetSimpleDetail(OrderGetSimpleDetailRequest request)
        {
            try
            {
                _orderBLL.InitTenantId(request.LoginTenantId);
                return await _orderBLL.OrderGetSimpleDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrderEditRemark(OrderEditRemarkRequest request)
        {
            try
            {
                _orderBLL.InitTenantId(request.LoginTenantId);
                return await _orderBLL.OrderEditRemark(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrderEditCommission(OrderEditCommissionRequest request)
        {
            try
            {
                _orderBLL.InitTenantId(request.LoginTenantId);
                return await _orderBLL.OrderEditCommission(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrderOperationLogGetPaging(OrderOperationLogGetPagingRequest request)
        {
            try
            {
                _orderBLL.InitTenantId(request.LoginTenantId);
                return await _orderBLL.OrderOperationLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> OrderRepeal(OrderRepealRequest request)
        {
            try
            {
                _orderBLL.InitTenantId(request.LoginTenantId);
                return await _orderBLL.OrderRepeal(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> IncomeLogGetPaging(IncomeLogGetPagingPagingRequest request)
        {
            try
            {
                _incomeLogBLL.InitTenantId(request.LoginTenantId);
                return await _incomeLogBLL.IncomeLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> IncomeLogAdd(IncomeLogAddRequest request)
        {
            try
            {
                _incomeLogBLL.InitTenantId(request.LoginTenantId);
                return await _incomeLogBLL.IncomeLogAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> IncomeLogRevoke(IncomeLogRevokeRequest request)
        {
            try
            {
                _incomeLogBLL.InitTenantId(request.LoginTenantId);
                return await _incomeLogBLL.IncomeLogRevoke(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsStudentCount(GetStatisticsStudentCountRequest request)
        {
            try
            {
                _statisticsStudentBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsStudentBLL.GetStatisticsStudentCount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsStudentTrackCount(GetStatisticsStudentTrackCountRequest request)
        {
            try
            {
                _statisticsStudentBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsStudentBLL.GetStatisticsStudentTrackCount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsStudentSource(GetStatisticsStudentRequest request)
        {
            try
            {
                _statisticsStudentBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsStudentBLL.GetStatisticsStudentSource(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsStudentType(GetStatisticsStudentRequest request)
        {
            try
            {
                _statisticsStudentBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsStudentBLL.GetStatisticsStudentType(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsSalesProduct(GetStatisticsSalesProductRequest request)
        {
            try
            {
                _statisticsSalesBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsSalesBLL.GetStatisticsSalesProduct(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsSalesProductProportion(GetStatisticsSalesProductProportionRequest request)
        {
            try
            {
                _statisticsSalesBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsSalesBLL.GetStatisticsSalesProductProportion(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsFinanceIn(GetStatisticsFinanceInRequest request)
        {
            try
            {
                _statisticsFinanceBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsFinanceBLL.GetStatisticsFinanceIn(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsFinanceInProjectType(GetStatisticsFinanceInProjectTypeRequest request)
        {
            try
            {
                _statisticsFinanceBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsFinanceBLL.GetStatisticsFinanceInProjectType(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsFinanceInPayType(GetStatisticsFinanceInPayTypeRequest request)
        {
            try
            {
                _statisticsFinanceBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsFinanceBLL.GetStatisticsFinanceInPayType(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsFinanceOut(GetStatisticsFinanceOutRequest request)
        {
            try
            {
                _statisticsFinanceBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsFinanceBLL.GetStatisticsFinanceOut(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsFinanceOutProjectType(GetStatisticsFinanceOutProjectTypeRequest request)
        {
            try
            {
                _statisticsFinanceBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsFinanceBLL.GetStatisticsFinanceOutProjectType(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsFinanceOutPayType(GetStatisticsFinanceOutPayTypeRequest request)
        {
            try
            {
                _statisticsFinanceBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsFinanceBLL.GetStatisticsFinanceOutPayType(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsClassAttendanceGet(StatisticsClassAttendanceRequest request)
        {
            try
            {
                _statisticsClassBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsClassBLL.StatisticsClassAttendanceGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsClassTimesGet(StatisticsClassTimesGetRequest request)
        {
            try
            {
                _statisticsClassBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsClassBLL.StatisticsClassTimesGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsClassCourseGet(StatisticsClassCourseGetRequest request)
        {
            try
            {
                _statisticsClassBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsClassBLL.StatisticsClassCourseGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsClassTeacherGet(StatisticsClassTeacherGetRequest request)
        {
            try
            {
                _statisticsClassBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsClassBLL.StatisticsClassTeacherGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsSalesCourseForAmount(GetStatisticsSalesCourseForAmountRequest request)
        {
            try
            {
                _statisticsSalesCourseBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsSalesCourseBLL.GetStatisticsSalesCourseForAmount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetStatisticsSalesCourseForCount(GetStatisticsSalesCourseForCountRequest request)
        {
            try
            {
                _statisticsSalesCourseBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsSalesCourseBLL.GetStatisticsSalesCourseForCount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsClassAttendanceTagGet(StatisticsClassAttendanceTagGetRequest request)
        {
            try
            {
                _statisticsClassBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsClassBLL.StatisticsClassAttendanceTagGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsTenantGet(StatisticsTenantGetRequest request)
        {
            try
            {
                _statisticsTenantBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsTenantBLL.StatisticsTenantGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantToDoThingGet(TenantToDoThingGetRequest request)
        {
            try
            {
                _statisticsTenantBLL.InitTenantId(request.LoginTenantId);
                return await _statisticsTenantBLL.TenantToDoThingGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClearDataSendSms(ClearDataSendSmsRequest request)
        {
            try
            {
                _sysDataClearBLL.InitTenantId(request.LoginTenantId);
                return await _sysDataClearBLL.ClearDataSendSms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClearData(ClearDataRequest request)
        {
            try
            {
                _sysDataClearBLL.InitTenantId(request.LoginTenantId);
                return await _sysDataClearBLL.ClearData(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
