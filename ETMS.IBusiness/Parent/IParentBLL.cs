using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IParentBLL
    {
        Task<IEnumerable<ParentStudentInfo>> GetMyStudent(ParentRequestBase request);

        Task<ResponseBase> ParentLoginSendSms(ParentLoginSendSmsRequest request);

        Task<ResponseBase> ParentLoginBySms(ParentLoginBySmsRequest request);

        Task<ResponseBase> ParentLoginByPwd(ParentLoginByPwdRequest request);

        ResponseBase ParentRefreshToken(ParentRefreshTokenRequest request);

        Task<ResponseBase> ParentGetAuthorizeUrl(ParentGetAuthorizeUrlRequest request);

        Task<ResponseBase> ParentLoginByCode(ParentLoginByCodeRequest request);

        Task<ResponseBase> ParentGetAuthorizeUrl2(ParentGetAuthorizeUrl2Request request);

        Task<ResponseBase> ParentBindingWeChat(ParentBindingWeChatRequest request);

        Task<ResponseBase> ParentInfoGet(ParentInfoGetRequest request);

        Task<ResponseBase> CheckParentCanLogin(ParentRequestBase request);

        Task<ResponseBase> ParentLoginout(ParentLoginoutRequest request);

        Task<ResponseBase> GetTenantInfoByNo(GetTenantInfoByNoRequest request);

        Task<ResponseBase> GetTenantInfo(GetTenantInfoRequest request);

        Task<ResponseBase> ParentGetCurrentTenant(ParentRequestBase request);

        Task<ResponseBase> ParentGetTenants(ParentRequestBase request);

        Task<ResponseBase> ParentTenantEntrance(ParentTenantEntranceRequest request);

        Task<ResponseBase> StudentRecommendRuleGet(ParentRequestBase request);

        ResponseBase UploadConfigGet(ParentRequestBase request);

        Task<ResponseBase> ParentRegister(ParentRegisterRequest request);
    }
}
