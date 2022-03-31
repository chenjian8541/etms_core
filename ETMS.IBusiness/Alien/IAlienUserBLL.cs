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
    public interface IAlienUserBLL : IAlienBaseBLL
    {
        Task<ResponseBase> ChangPwd(ChangPwdRequest request);

        Task<ResponseBase> ChangUserPwd(ChangUserPwdRequest request);

        Task<ResponseBase> RoleListGet(RoleListGetRequest request);

        Task<ResponseBase> RoleAdd(RoleAddRequest request);

        Task<ResponseBase> RoleEdit(RoleEditRequest request);

        Task<ResponseBase> RoleGet(RoleGetRequest request);

        ResponseBase RoleDefaultGet(RoleDefaultGetRequest request);

        Task<ResponseBase> RoleDel(RoleDelRequest request);

        Task<ResponseBase> OrgGetAll(OrgGetAllRequest request);

        Task<ResponseBase> OrgAdd(OrgAddRequest request);

        Task<ResponseBase> OrgEdit(OrgEditRequest request);

        Task<ResponseBase> OrgDel(OrgDelRequest request);

        Task<ResponseBase> UserGetPaging(UserGetPagingRequest request);

        Task<ResponseBase> UserGet(UserGetRequest request);

        Task<ResponseBase> UserAdd(UserAddRequest request);

        Task<ResponseBase> UserDel(UserDelRequest request);

        Task<ResponseBase> UserEdit(UserEditRequest request);

        Task<ResponseBase> UserOperationLogGetPaging(UserOperationLogGetPagingRequest request);

        ResponseBase UserLogTypeGet(AlienRequestBase request);
    }
}
