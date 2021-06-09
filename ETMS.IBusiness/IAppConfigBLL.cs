using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IAppConfigBLL : IBaseBLL
    {
        Task<TenantConfig> TenantConfigGet(int tenantId);

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

        Task<ResponseBase> GetTenantInfoH5ByNo(GetTenantInfoH5ByNoRequest request);

        Task<ResponseBase> GetTenantInfoH5(int tenantId);

        Task<ResponseBase> GetTenantInfoH5(GetTenantInfoH5Request request);

        Task<ResponseBase> StudentCheckInConfigSave(StudentCheckInConfigSaveRequest request);

        Task<ResponseBase> StudentRecommendConfigSave(StudentRecommendConfigSaveRequest request);

        Task<ResponseBase> StudentRecommendConfigSave2(StudentRecommendConfigSave2Request request);

        Task<ResponseBase> TenantOtherConfigSave(TenantOtherConfigSaveRequest request);
    }
}
