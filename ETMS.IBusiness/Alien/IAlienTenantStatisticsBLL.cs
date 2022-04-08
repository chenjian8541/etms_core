using ETMS.Entity.Alien.Dto.TenantStatistics.Request;
using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Alien
{
    public interface IAlienTenantStatisticsBLL : IAlienBaseBLL, IAlienTenantBaseBLL
    {
        Task<ResponseBase> AlTenantStatisticsFinanceInGet(AlTenantStatisticsFinanceInGetRequest request);

        Task<ResponseBase> AlTenantStatisticsFinanceOutGet(AlTenantStatisticsFinanceOutGetRequest request);

        Task<ResponseBase> AlTenantStatisticsFinanceIncomeYearGet(AlTenantStatisticsFinanceIncomeYearGetRequest request);

        Task<ResponseBase> AlTenantStatisticsFinanceIncomeMonthGetPaging(AlTenantStatisticsFinanceIncomeMonthGetPagingRequest request);

        Task<ResponseBase> AlTenantStatisticsFinanceIncomeMonthGet(AlTenantStatisticsFinanceIncomeMonthGetRequest request);

        Task<ResponseBase> AlTenantIncomeLogGetPaging(AlTenantIncomeLogGetPagingRequest request);
    }
}
