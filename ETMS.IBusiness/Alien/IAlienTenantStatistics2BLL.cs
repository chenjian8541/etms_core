using ETMS.Entity.Alien.Dto.TenantStatistics.Request;
using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Alien
{
    public interface IAlienTenantStatistics2BLL: IAlienBaseBLL, IAlienTenantBaseBLL
    {
        Task<ResponseBase> AlTenantStatisticsSalesProductGet(AlTenantStatisticsSalesProductGetRequest request);

        Task<ResponseBase> AlTenantStatisticsSalesTenantEchartsBarMulti1(AlTenantStatisticsSalesTenantEchartsBarMulti1Request request);

        Task<ResponseBase> AlTenantStatisticsSalesTenantEchartsBarMulti2(AlTenantStatisticsSalesTenantEchartsBarMulti2Request request);

        Task<ResponseBase> AlTenantStatisticsSalesTenantGet(AlTenantStatisticsSalesTenantGetRequest request);

        Task<ResponseBase> AlTenantStatisticsSalesProductMonthGet(AlTenantStatisticsSalesProductMonthGetRequest request);

        Task<ResponseBase> AlTenantStatisticsSalesProductMonthPagingGet(AlTenantStatisticsSalesProductMonthPagingGetRequest request);

        Task<ResponseBase> AlTenantOrderGetPagingGet(AlTenantOrderGetPagingGetRequest request);

        Task<ResponseBase> AlTenantOrderGetDetailGet(AlTenantOrderGetDetailGetRequest request);

        Task<ResponseBase> AlTenantOrderReturnLogGet(AlTenantOrderReturnLogGetRequest request);

        Task<ResponseBase> AlTenantOrderTransferCoursesLogGet(AlTenantOrderTransferCoursesLogGetRequest request);

        Task<ResponseBase> AlTenantOrderTransferCoursesGetDetailGet(AlTenantOrderTransferCoursesGetDetailGetRequest request);

        Task<ResponseBase> AlTenantOrderGetDetailAccountRechargeGet(AlTenantOrderGetDetailAccountRechargeGetRequest request);
    }
}
