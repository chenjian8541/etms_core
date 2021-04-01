using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStatisticsSalesBLL : IBaseBLL
    {
        Task StatisticsSalesProductConsumeEvent(StatisticsSalesProductEvent request);

        Task<ResponseBase> GetStatisticsSalesProduct(GetStatisticsSalesProductRequest request);

        Task<ResponseBase> GetStatisticsSalesProductProportion(GetStatisticsSalesProductProportionRequest request);

        Task StatisticsSalesOrderConsumerEvent(StatisticsSalesOrderEvent request);

        Task<ResponseBase> StatisticsSalesTenantGet(RequestBase request);

        Task<ResponseBase> StatisticsSalesTenantGet2(StatisticsSalesTenantGet2Request request);

        Task<ResponseBase> StatisticsSalesUserGet(StatisticsSalesUserGetRequest request);

        Task<ResponseBase> StatisticsSalesTenantEchartsBarMulti1(StatisticsSalesTenantEchartsBarMulti1Request request);

        Task<ResponseBase> StatisticsSalesTenantEchartsBarMulti2(StatisticsSalesTenantEchartsBarMulti2Request request);
    }
}
