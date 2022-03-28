using ETMS.Entity.Alien.Common;
using ETMS.Entity.Alien.Dto.User.Request;
using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Alien
{
    public interface IAlienUserLoginBLL: IAlienBaseBLL
    {
        Task<ResponseBase> UserLogin(AlUserLoginRequest request);

        Task<ResponseBase> UserLoginGet(AlienRequestBase request);

        Task<ResponseBase> UserLoginPermissionGet(AlienRequestBase request);

        Task<ResponseBase> CheckUserCanLogin(AlienRequestBase request);
    }
}
