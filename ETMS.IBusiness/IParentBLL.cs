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

        ResponseBase ParentRefreshToken(ParentRefreshTokenRequest request);

        Task<ResponseBase> ParentGetAuthorizeUrl(ParentGetAuthorizeUrlRequest request);

        Task<ResponseBase> ParentLoginByCode(ParentLoginByCodeRequest request);

        Task<ResponseBase> ParentInfoGet(ParentInfoGetRequest request);

        Task<ResponseBase> CheckParentCanLogin(ParentRequestBase request);
    }
}
