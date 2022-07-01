using ETMS.Entity.Common;
using ETMS.Entity.Dto.OpenParent.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IOpenParentBLL
    {
        Task<ResponseBase> ParentLoginSendSms(ParentOpenLoginSendSmsRequest request);

        Task<ResponseBase> ParentLoginBySms(ParentOpenLoginBySmsRequest request);

        Task<ResponseBase> ParentRegisterSendSms(ParentRegisterSendSmsRequest request);

        Task<ResponseBase> ParentRegister(ParentRegisterOpenRequest request);

        Task<ResponseBase> StudentOpenRegisterInit(StudentOpenRegisterInitRequest request);

        Task<ResponseBase> StudentOpenRegisterSendSms(StudentOpenRegisterSendSmsRequest request);

        Task<ResponseBase> StudentOpenRegisterSubmit(StudentOpenRegisterSubmitRequest request);
    }
}
