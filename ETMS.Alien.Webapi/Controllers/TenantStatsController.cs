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

        public TenantStatsController(IAlienTenantStatisticsBLL alienTenantStatisticsBLL)
        {
            this._alienTenantStatisticsBLL = alienTenantStatisticsBLL;
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
    }
}
