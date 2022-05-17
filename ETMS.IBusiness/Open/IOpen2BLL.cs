using ETMS.Entity.Common;
using ETMS.Entity.Dto.Open2.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IOpen2BLL : IBaseBLL
    {
        Task<ResponseBase> ClassRecordDetailGet(ClassRecordDetailGetOpenRequest request);

        Task<ResponseBase> CheckPhoneSmsSend(CheckPhoneSmsSendRequest request);

        Task<ResponseBase> TryApplyLogAdd(TryApplyLogAddRequest request);

        Task<ResponseBase> EvaluateStudentDetail(EvaluateStudentDetailRequest request);

        Task<ResponseBase> ShareContentGet(ShareContentGetRequest request);

        Task<ResponseBase> AlbumDetailGet(AlbumDetailGetRequest request);

        ResponseBase AlbumShare(AlbumShareRequest request);

        Task<ResponseBase> AlbumInfoGet(AlbumInfoGetRequest request);

        ResponseBase PhoneVerificationCodeGet(PhoneVerificationCodeGetRequest request);

        Task<ResponseBase> CheckPhoneSmsSafe(CheckPhoneSmsSafeRequest request);

        Task<ResponseBase> SendSmsCodeAboutRegister(SendSmsCodeAboutRegisterRequest request);

        Task<ResponseBase> CheckTenantAccount(CheckTenantAccountRequest request);

        Task<ResponseBase> ChangeTenantUserPwd(ChangeTenantUserPwdRequest request);
    }
}
