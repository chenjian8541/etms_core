using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.EtmsManage.Dto.User.Request;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysUserBLL
    {
        Task<ResponseBase> UserGet(UserGetRequest request);

        Task<ResponseBase> UserAdd(UserAddRequest request);

        Task<ResponseBase> UserEdit(UserEditRequest request);

        Task<ResponseBase> UserDel(UserDelRequest request);

        Task<ResponseBase> UserGetPaging(UserGetPagingRequest request);

        Task<ResponseBase> UserMyRoleListGet(UserMyRoleListGetRequest request);

        Task<ResponseBase> UserRoleAdd(UserRoleAddRequest request);

        Task<ResponseBase> UserRoleEdit(UserRoleEditRequest request);

        Task<ResponseBase> UserRoleGet(UserRoleGetRequest request);

        Task<ResponseBase> UserRoleDefaultGet(UserRoleDefaultGetRequest request);

        Task<ResponseBase> UserRoleDel(UserRoleDelRequest request);

        Task<ResponseBase> UserRoleGetPaging(UserRoleGetPagingRequest request);
    }
}
