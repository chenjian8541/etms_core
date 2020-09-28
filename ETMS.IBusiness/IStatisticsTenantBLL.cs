using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
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
    }
}
