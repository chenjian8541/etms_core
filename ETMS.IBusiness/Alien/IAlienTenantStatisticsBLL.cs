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

        Task<ResponseBase> AlTenantStatisticsStudentCountGet(AlTenantStatisticsStudentCountGetRequest request);

        Task<ResponseBase> AlTenantStatisticsStudentTrackCountGet(AlTenantStatisticsStudentTrackCountGetRequest request);

        Task<ResponseBase> AlTenantStatisticsStudentSourceGet(AlTenantStatisticsStudentSourceGetRequest request);

        Task<ResponseBase> AlTenantStatisticsStudentTypeGet(AlTenantStatisticsStudentTypeGetRequest request);

        Task<ResponseBase> AlTenantStatisticsStudentCountPagingGet(AlTenantStatisticsStudentCountPagingGetRequest request);

        Task<ResponseBase> AlTenantStatisticsStudentCountMonthGet(AlTenantStatisticsStudentCountMonthGetRequest request);

        Task<ResponseBase> AlTenantStatisticsStudentCountMonthPagingGet(AlTenantStatisticsStudentCountMonthPagingGetRequest request);
    }
}
