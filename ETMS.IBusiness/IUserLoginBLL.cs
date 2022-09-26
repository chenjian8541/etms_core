
using ETMS.Entity.Common;
using ETMS.Entity.Dto.User;
using ETMS.Entity.Dto.User.Request;
using System;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IUserLoginBLL
    {
        Task<ResponseBase> UserGetAuthorizeUrl(UserGetAuthorizeUrlRequest request);

        Task<ResponseBase> UserGetAuthorizeUrl2(UserGetAuthorizeUrl2Request request);

        Task<ResponseBase> UserBindingWeChat(UserBindingWeChatRequest request);

        Task<ResponseBase> UserLogin(UserLoginRequest request);

        Task<ResponseBase> UserLoginSendSms(UserLoginSendSmsRequest request);

        Task<ResponseBase> UserLoginSendSmsCodeSafe(UserLoginSendSmsCodeSafeRequest request);

        Task<ResponseBase> UserLoginBySms(UserLoginBySmsRequest request);

        Task<ResponseBase> UserLoginBySmsH5(UserLoginBySmsH5Request request);

        Task<ResponseBase> UserLoginByH5(UserLoginByH5Request request);

        Task<ResponseBase> UserEnterH5(UserEnterH5Request request);

        Task<ResponseBase> CheckUserCanLogin(RequestBase request);

        //Task<bool> GetUserDataLimit(RequestBase request);

        ResponseBase UserCheck(UserCheckRequest request);

        Task<ResponseBase> UserGetCurrentTenant(RequestBase request);

        Task<ResponseBase> UserGetTenants(RequestBase request);

        Task<ResponseBase> UserTenantEntrance(UserTenantEntranceRequest request);

        Task<ResponseBase> UserTenantEntrancePC(UserTenantEntrancePCRequest request);

        ResponseBase UserTenantEntrancePCGate(UserTenantEntrancePCGateRequest request);
    }
}
