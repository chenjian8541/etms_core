
using ETMS.Entity.Common;
using ETMS.Entity.Dto.User;
using ETMS.Entity.Dto.User.Request;
using System;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IUserLoginBLL
    {
        Task<ResponseBase> UserLogin(UserLoginRequest request);

        Task<ResponseBase> UserLoginSendSms(UserLoginSendSmsRequest request);

        Task<ResponseBase> UserLoginBySms(UserLoginBySmsRequest request);

        Task<ResponseBase> CheckUserCanLogin(RequestBase request);

        Task<bool> GetUserDataLimit(RequestBase request);
    }
}
