using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IAppConfigBLL : IBaseBLL
    {
        Task<ResponseBase> TenantConfigGet(TenantConfigGetRequest request);

        Task<ResponseBase> ClassCheckSignConfigSave(ClassCheckSignConfigSaveRequest request);

        Task<ResponseBase> StudentNoticeConfigSave(StudentNoticeConfigSaveRequest request);

        Task<ResponseBase> StartClassNoticeSave(StartClassNoticeSaveRequest request);

        Task<ResponseBase> UserNoticeConfigSave(UserNoticeConfigSaveRequest request);

        Task<ResponseBase> UserStartClassNoticeSave(UserStartClassNoticeSaveRequest request);

        Task<ResponseBase> UserWeChatNoticeRemarkSave(UserWeChatNoticeRemarkSaveRequest request);

        Task<ResponseBase> WeChatNoticeRemarkSave(WeChatNoticeRemarkSaveRequest request);

        Task<ResponseBase> StudentCourseRenewalConfigSave(StudentCourseRenewalConfigSaveRequest request);

        Task<ResponseBase> PrintConfigGet(PrintConfigGetRequest request);

        Task<ResponseBase> PrintConfigSave(PrintConfigSaveRequest request);

        Task<ResponseBase> ParentBannerGet(ParentBannerGetRequest request);

        Task<ResponseBase> ParentBannerSave(ParentBannerSaveRequest request);

        Task<ResponseBase> TenantShowSave(TenantShowSaveRequest request);

        Task<ResponseBase> StudentCourseNotEnoughCountSave(StudentCourseNotEnoughCountSaveRequest request);
    }
}
