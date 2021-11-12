using ETMS.Entity.Common;
using ETMS.Entity.Dto.OpenApi99.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.OpenApi99
{
    public interface ITenantOpenBLL : IBaseBLL
    {
        Task<ResponseBase> TenantInfoGet(OpenApi99Base request);

        Task<ResponseBase> TenantLcsAccountGet(OpenApi99Base request);

        Task<ResponseBase> SmsSend(SmsSendRequest request);

        Task<ResponseBase> SmsSendValidCode(SmsSendValidCodeRequest request);
    }
}
