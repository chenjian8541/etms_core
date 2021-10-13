using ETMS.Entity.Common;
using ETMS.Entity.Dto.PaymentService.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IPaymentStatisticsBLL : IBaseBLL
    {
        Task<ResponseBase> StatisticsLcsPayDayGetLine(StatisticsLcsPayDayGetLineRequest request);

        Task<ResponseBase> StatisticsLcsPayDayGetPaging(StatisticsLcsPayDayGetPagingRequest request);

        Task<ResponseBase> StatisticsLcsPayMonthGetLine(StatisticsLcsPayMonthGetLineRequest request);

        Task<ResponseBase> StatisticsLcsPayMonthGetPaging(StatisticsLcsPayMonthGetPagingRequest request);

        Task<ResponseBase> StatisticsLcsPayYear(StatisticsLcsPayYearRequest request);
    }
}
