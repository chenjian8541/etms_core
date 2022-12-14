using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ITenantBLL : IBaseBLL
    {
        Task<ResponseBase> TenantGet(TenantGetRequest request);

        Task<ResponseBase> TenantGetView(TenantGetRequest request);

        Task TenantSmsDeductionEventConsume(TenantSmsDeductionEvent request);

        Task<ResponseBase> SysSafeSmsSend(SysSafeSmsSendRequest request);

        ResponseBase SysSafeSmsCheck(SysSafeSmsCheckRequest request);

        Task<ResponseBase> NoticeConfigGet(NoticeConfigGetRequest request);

        Task<ResponseBase> NoticeConfigSave(NoticeConfigSaveRequest request);

        Task<ResponseBase> PageBascDataGet(RequestBase request);

        ResponseBase AppConfigGet(RequestBase request);
    }
}
