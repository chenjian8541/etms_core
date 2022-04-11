using ETMS.Entity.Alien.Common;
using ETMS.Entity.Alien.Dto.Tenant.Request;
using ETMS.Entity.Alien.Dto.TenantStatistics.Request;
using ETMS.Entity.Alien.Dto.User.Request;
using ETMS.Entity.Common;
using ETMS.IBusiness.Alien;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETMS.Alien.Webapi.Controllers
{
    [Route("api/tenantStats/[action]")]
    [ApiController]
    [Authorize]
    public class TenantStatsController : ControllerBase
    {
        private readonly IAlienTenantStatisticsBLL _alienTenantStatisticsBLL;

        private readonly IAlienTenantStatistics2BLL _alienTenantStatistics2BLL;
        public TenantStatsController(IAlienTenantStatisticsBLL alienTenantStatisticsBLL, IAlienTenantStatistics2BLL alienTenantStatistics2BLL)
        {
            this._alienTenantStatisticsBLL = alienTenantStatisticsBLL;
            this._alienTenantStatistics2BLL = alienTenantStatistics2BLL;
        }

        public async Task<ResponseBase> AlTenantStatisticsFinanceInGet(AlTenantStatisticsFinanceInGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsFinanceInGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsFinanceOutGet(AlTenantStatisticsFinanceOutGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsFinanceOutGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsFinanceIncomeYearGet(AlTenantStatisticsFinanceIncomeYearGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsFinanceIncomeYearGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsFinanceIncomeMonthGetPaging(AlTenantStatisticsFinanceIncomeMonthGetPagingRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsFinanceIncomeMonthGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsFinanceIncomeMonthGet(AlTenantStatisticsFinanceIncomeMonthGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsFinanceIncomeMonthGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantIncomeLogGetPaging(AlTenantIncomeLogGetPagingRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantIncomeLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentCountGet(AlTenantStatisticsStudentCountGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsStudentCountGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentTrackCountGet(AlTenantStatisticsStudentTrackCountGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsStudentTrackCountGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentSourceGet(AlTenantStatisticsStudentSourceGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsStudentSourceGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentTypeGet(AlTenantStatisticsStudentTypeGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsStudentTypeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentCountPagingGet(AlTenantStatisticsStudentCountPagingGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsStudentCountPagingGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentCountMonthGet(AlTenantStatisticsStudentCountMonthGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsStudentCountMonthGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsStudentCountMonthPagingGet(AlTenantStatisticsStudentCountMonthPagingGetRequest request)
        {
            try
            {
                _alienTenantStatisticsBLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatisticsBLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatisticsBLL.AlTenantStatisticsStudentCountMonthPagingGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesProductGet(AlTenantStatisticsSalesProductGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantStatisticsSalesProductGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesTenantEchartsBarMulti1(AlTenantStatisticsSalesTenantEchartsBarMulti1Request request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantStatisticsSalesTenantEchartsBarMulti1(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesTenantEchartsBarMulti2(AlTenantStatisticsSalesTenantEchartsBarMulti2Request request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantStatisticsSalesTenantEchartsBarMulti2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesTenantGet(AlTenantStatisticsSalesTenantGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantStatisticsSalesTenantGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesProductMonthGet(AlTenantStatisticsSalesProductMonthGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantStatisticsSalesProductMonthGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantStatisticsSalesProductMonthPagingGet(AlTenantStatisticsSalesProductMonthPagingGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantStatisticsSalesProductMonthPagingGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantOrderGetPagingGet(AlTenantOrderGetPagingGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantOrderGetPagingGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantOrderGetDetailGet(AlTenantOrderGetDetailGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantOrderGetDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantOrderReturnLogGet(AlTenantOrderReturnLogGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantOrderReturnLogGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantOrderTransferCoursesLogGet(AlTenantOrderTransferCoursesLogGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantOrderTransferCoursesLogGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantOrderTransferCoursesGetDetailGet(AlTenantOrderTransferCoursesGetDetailGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantOrderTransferCoursesGetDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlTenantOrderGetDetailAccountRechargeGet(AlTenantOrderGetDetailAccountRechargeGetRequest request)
        {
            try
            {
                _alienTenantStatistics2BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics2BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics2BLL.AlTenantOrderGetDetailAccountRechargeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
