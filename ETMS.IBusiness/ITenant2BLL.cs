using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Event.DataContract;

namespace ETMS.IBusiness
{
    public interface ITenant2BLL : IBaseBLL
    {
        Task<ResponseBase> TenantStatisticsGet(RequestBase request);

        Task<ResponseBase> ActivityConfigGet(RequestBase request);

        Task<ResponseBase> ActivityConfigSave(ActivityConfigSaveRequest request);

        ResponseBase TenantCustomized(RequestBase request);
    }
}
