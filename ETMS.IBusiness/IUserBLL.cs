using ETMS.Entity.Common;
using ETMS.Entity.Dto.User;
using ETMS.Entity.Dto.User.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IUserBLL : IBaseBLL
    {
        Task<ResponseBase> GetOemLogin(GetOemLoginRequest request);

        Task<ResponseBase> GetOemLogin2(RequestBase request);

        Task<ResponseBase> GetLoginInfo(GetLoginInfoRequest request);

        Task<ResponseBase> GetLoginPermission(GetLoginPermissionRequest request);

        Task<ResponseBase> UpdateUserInfo(UpdateUserInfoRequest request);

        Task<ResponseBase> GetLoginInfoH5(GetLoginInfoRequest request);

        Task<ResponseBase> GetUserImportantInfo(RequestBase request);

        //Task<ResponseBase> ChangPwdSendSms(ChangPwdSendSmsRequest request);

        Task<ResponseBase> ChangPwd(ChangPwdRequest request);

        Task<ResponseBase> ChangUserPwd(ChangUserPwdRequest request);

        Task<ResponseBase> RoleListGet(RoleListGetRequest request);

        Task<ResponseBase> RoleAdd(RoleAddRequest request);

        Task<ResponseBase> RoleEdit(RoleEditRequest request);

        Task<ResponseBase> RoleGet(RoleGetRequest request);

        Task<ResponseBase> RoleDefaultGet(RoleDefaultGetRequest request);

        Task<ResponseBase> RoleDel(RoleDelRequest request);

        Task<ResponseBase> UserAdd(UserAddRequest request);

        Task<ResponseBase> UserEdit(UserEditRequest request);

        Task<ResponseBase> UserDel(UserDelRequest request);

        Task<ResponseBase> UserGetPaging(UserGetPagingRequest request);

        Task<ResponseBase> TeacherGetPaging(UserGetPagingRequest request);

        Task<ResponseBase> UserOperationLogGetPaging(UserOperationLogGetPagingRequest request);

        ResponseBase UserOperationLogTypeGet(RequestBase request);

        Task<ResponseBase> TeacherEdit(TeacherEditRequest request);

        Task<ResponseBase> TeacherRemove(TeacherRemoveRequest request);

        Task<ResponseBase> UserGet(UserGetRequest request);

        Task<ResponseBase> TeacherClassTimesGetPaging(TeacherClassTimesGetPagingRequest request);

        Task<ResponseBase> UserFeedback(UserFeedbackRequest request);
    }
}
