using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStatisticsTenantBLL : IBaseBLL
    {
        Task<ResponseBase> StatisticsTenantGet(StatisticsTenantGetRequest request);

        Task<ResponseBase> TenantToDoThingGet(TenantToDoThingGetRequest request);

        Task ResetTenantToDoThingConsumerEvent(ResetTenantToDoThingEvent request);

        Task StatisticsStudentAccountRechargeConsumerEvent(StatisticsStudentAccountRechargeEvent request);
    }
}
