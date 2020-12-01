using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/basic/[action]")]
    [ApiController]
    [Authorize]
    public class BasicDataController : ControllerBase
    {
        private readonly IHolidaySettingBLL _holidaySettingBLL;

        private readonly IStudentExtendFieldBLL _studentExtendFieldBLL;

        private readonly ISubjectBLL _subjectBLL;

        private readonly IStudentSourceBLL _studentSourceBLL;

        private readonly IGradeBLL _gradeBLL;

        private readonly IClassSetBLL _classSetBLL;

        private readonly IStudentGrowingTagBLL _studentGrowingTagBLL;

        private readonly IClassRoomBLL _classRoomBLL;

        private readonly IStudentTagBLL _studentTagBLL;

        private readonly IStudentRelationshipBLL _studentRelationshipBLL;

        private readonly IClassCategoryBLL _classCategoryBLL;

        private readonly IGiftCategoryBLL _giftCategoryBLL;

        private readonly IAppConfigBLL _appConfigBLL;

        private readonly IIncomeProjectTypeBLL _incomeProjectTypeBLL;

        private readonly ITenantBLL _tenantBLL;

        public BasicDataController(IHolidaySettingBLL holidaySettingBLL, IStudentExtendFieldBLL studentExtendFieldBLL, ISubjectBLL subjectBLL,
            IStudentSourceBLL studentSourceBLL, IGradeBLL gradeBLL, IClassSetBLL classSetBLL, IStudentGrowingTagBLL studentGrowingTagBLL,
            IClassRoomBLL classRoomBLL, IStudentTagBLL studentTagBLL, IStudentRelationshipBLL studentRelationshipBLL, IClassCategoryBLL classCategoryBLL,
            IGiftCategoryBLL giftCategoryBLL, IAppConfigBLL appConfigBLL, IIncomeProjectTypeBLL incomeProjectTypeBLL, ITenantBLL tenantBLL)
        {
            this._holidaySettingBLL = holidaySettingBLL;
            this._studentExtendFieldBLL = studentExtendFieldBLL;
            this._subjectBLL = subjectBLL;
            this._studentSourceBLL = studentSourceBLL;
            this._gradeBLL = gradeBLL;
            this._classSetBLL = classSetBLL;
            this._studentGrowingTagBLL = studentGrowingTagBLL;
            this._classRoomBLL = classRoomBLL;
            this._studentTagBLL = studentTagBLL;
            this._studentRelationshipBLL = studentRelationshipBLL;
            this._classCategoryBLL = classCategoryBLL;
            this._giftCategoryBLL = giftCategoryBLL;
            this._appConfigBLL = appConfigBLL;
            this._incomeProjectTypeBLL = incomeProjectTypeBLL;
            this._tenantBLL = tenantBLL;
        }

        /// <summary>
        /// 节假日设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("holidaySettingAdd")]
        [HttpPost]
        public async Task<ResponseBase> HolidaySettingAdd(HolidaySettingAddRequest request)
        {
            try
            {
                this._holidaySettingBLL.InitTenantId(request.LoginTenantId);
                return await _holidaySettingBLL.HolidaySettingAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 获取节假日设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("holidaySettingGet")]
        [HttpPost]
        public async Task<ResponseBase> HolidaySettingGet(HolidaySettingGetRequest request)
        {
            try
            {
                this._holidaySettingBLL.InitTenantId(request.LoginTenantId);
                return await _holidaySettingBLL.HolidaySettingGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 删除节假日设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("holidaySettingDel")]
        [HttpPost]
        public async Task<ResponseBase> HolidaySettingDel(HolidaySettingDelRequest request)
        {
            try
            {
                this._holidaySettingBLL.InitTenantId(request.LoginTenantId);
                return await _holidaySettingBLL.HolidaySettingDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentExtendFieldAdd")]
        [HttpPost]
        public async Task<ResponseBase> StudentExtendFieldAdd(StudentExtendFieldAddRequest request)
        {
            try
            {
                this._studentExtendFieldBLL.InitTenantId(request.LoginTenantId);
                return await _studentExtendFieldBLL.StudentExtendFieldAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentExtendFieldGet")]
        [HttpPost]
        public async Task<ResponseBase> StudentExtendFieldGet(StudentExtendFieldGetRequest request)
        {
            try
            {
                this._studentExtendFieldBLL.InitTenantId(request.LoginTenantId);
                return await _studentExtendFieldBLL.StudentExtendFieldGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentExtendFieldDel")]
        [HttpPost]
        public async Task<ResponseBase> StudentExtendFieldDel(StudentExtendFieldDelRequest request)
        {
            try
            {
                this._studentExtendFieldBLL.InitTenantId(request.LoginTenantId);
                return await _studentExtendFieldBLL.StudentExtendFieldDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("subjectAdd")]
        [HttpPost]
        public async Task<ResponseBase> SubjectAdd(SubjectAddRequest request)
        {
            try
            {
                this._subjectBLL.InitTenantId(request.LoginTenantId);
                return await _subjectBLL.SubjectAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("subjectGet")]
        [HttpPost]
        public async Task<ResponseBase> SubjectGet(SubjectGetRequest request)
        {
            try
            {
                this._subjectBLL.InitTenantId(request.LoginTenantId);
                return await _subjectBLL.SubjectGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("subjectDel")]
        [HttpPost]
        public async Task<ResponseBase> SubjectDel(SubjectDelRequest request)
        {
            try
            {
                this._subjectBLL.InitTenantId(request.LoginTenantId);
                return await _subjectBLL.SubjectDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentSourceAdd")]
        [HttpPost]
        public async Task<ResponseBase> StudentSourceAdd(StudentSourceAddRequest request)
        {
            try
            {
                this._studentSourceBLL.InitTenantId(request.LoginTenantId);
                return await _studentSourceBLL.StudentSourceAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentSourceGet")]
        [HttpPost]
        public async Task<ResponseBase> StudentSourceGet(StudentSourceGetRequest request)
        {
            try
            {
                this._studentSourceBLL.InitTenantId(request.LoginTenantId);
                return await _studentSourceBLL.StudentSourceGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentSourceDel")]
        [HttpPost]
        public async Task<ResponseBase> StudentSourceDel(StudentSourceDelRequest request)
        {
            try
            {
                this._studentSourceBLL.InitTenantId(request.LoginTenantId);
                return await _studentSourceBLL.StudentSourceDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("gradeAdd")]
        [HttpPost]
        public async Task<ResponseBase> GradeAdd(GradeAddRequest request)
        {
            try
            {
                this._gradeBLL.InitTenantId(request.LoginTenantId);
                return await _gradeBLL.GradeAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("gradeGet")]
        [HttpPost]
        public async Task<ResponseBase> GradeGet(GradeGetRequest request)
        {
            try
            {
                this._gradeBLL.InitTenantId(request.LoginTenantId);
                return await _gradeBLL.GradeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("gradeDel")]
        [HttpPost]
        public async Task<ResponseBase> GradeDel(GradeDelRequest request)
        {
            try
            {
                this._gradeBLL.InitTenantId(request.LoginTenantId);
                return await _gradeBLL.GradeDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("classSetAdd")]
        [HttpPost]
        public async Task<ResponseBase> ClassSetAdd(ClassSetAddRequest request)
        {
            try
            {
                this._classSetBLL.InitTenantId(request.LoginTenantId);
                return await _classSetBLL.ClassSetAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("classSetGet")]
        [HttpPost]
        public async Task<ResponseBase> ClassSetGet(ClassSetGetRequest request)
        {
            try
            {
                this._classSetBLL.InitTenantId(request.LoginTenantId);
                return await _classSetBLL.ClassSetGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("classSetDel")]
        [HttpPost]
        public async Task<ResponseBase> ClassSetDel(ClassSetDelRequest request)
        {
            try
            {
                this._classSetBLL.InitTenantId(request.LoginTenantId);
                return await _classSetBLL.ClassSetDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentGrowingTagAdd")]
        [HttpPost]
        public async Task<ResponseBase> StudentGrowingTagAdd(StudentGrowingTagAddRequest request)
        {
            try
            {
                this._studentGrowingTagBLL.InitTenantId(request.LoginTenantId);
                return await _studentGrowingTagBLL.StudentGrowingTagAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentGrowingTagGet")]
        [HttpPost]
        public async Task<ResponseBase> StudentGrowingTagGet(StudentGrowingTagGetRequest request)
        {
            try
            {
                this._studentGrowingTagBLL.InitTenantId(request.LoginTenantId);
                return await _studentGrowingTagBLL.StudentGrowingTagGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentGrowingTagDel")]
        [HttpPost]
        public async Task<ResponseBase> StudentGrowingTagDel(StudentGrowingTagDelRequest request)
        {
            try
            {
                this._studentGrowingTagBLL.InitTenantId(request.LoginTenantId);
                return await _studentGrowingTagBLL.StudentGrowingTagDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("classRoomAdd")]
        [HttpPost]
        public async Task<ResponseBase> ClassRoomAdd(ClassRoomAddRequest request)
        {
            try
            {
                this._classRoomBLL.InitTenantId(request.LoginTenantId);
                return await _classRoomBLL.ClassRoomAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("classRoomGet")]
        [HttpPost]
        public async Task<ResponseBase> ClassRoomGet(ClassRoomGetRequest request)
        {
            try
            {
                this._classRoomBLL.InitTenantId(request.LoginTenantId);
                return await _classRoomBLL.ClassRoomGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("classRoomDel")]
        [HttpPost]
        public async Task<ResponseBase> ClassRoomDel(ClassRoomDelRequest request)
        {
            try
            {
                this._classRoomBLL.InitTenantId(request.LoginTenantId);
                return await _classRoomBLL.ClassRoomDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentTagAdd")]
        [HttpPost]
        public async Task<ResponseBase> StudentTagAdd(StudentTagAddRequest request)
        {
            try
            {
                this._studentTagBLL.InitTenantId(request.LoginTenantId);
                return await _studentTagBLL.StudentTagAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentTagGet")]
        [HttpPost]
        public async Task<ResponseBase> StudentTagGet(StudentTagGetRequest request)
        {
            try
            {
                this._studentTagBLL.InitTenantId(request.LoginTenantId);
                return await _studentTagBLL.StudentTagGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentTagDel")]
        [HttpPost]
        public async Task<ResponseBase> StudentTagDel(StudentTagDelRequest request)
        {
            try
            {
                this._studentTagBLL.InitTenantId(request.LoginTenantId);
                return await _studentTagBLL.StudentTagDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentRelationshipAdd")]
        [HttpPost]
        public async Task<ResponseBase> StudentRelationshipAdd(StudentRelationshipAddRequest request)
        {
            try
            {
                this._studentRelationshipBLL.InitTenantId(request.LoginTenantId);
                return await _studentRelationshipBLL.StudentRelationshipAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentRelationshipGet")]
        [HttpPost]
        public async Task<ResponseBase> StudentRelationshipGet(StudentRelationshipGetRequest request)
        {
            try
            {
                this._studentRelationshipBLL.InitTenantId(request.LoginTenantId);
                return await _studentRelationshipBLL.StudentRelationshipGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentRelationshipDel")]
        [HttpPost]
        public async Task<ResponseBase> StudentRelationshipDel(StudentRelationshipDelRequest request)
        {
            try
            {
                this._studentRelationshipBLL.InitTenantId(request.LoginTenantId);
                return await _studentRelationshipBLL.StudentRelationshipDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("classCategoryAdd")]
        [HttpPost]
        public async Task<ResponseBase> ClassCategoryAdd(ClassCategoryAddRequest request)
        {
            try
            {
                this._classCategoryBLL.InitTenantId(request.LoginTenantId);
                return await _classCategoryBLL.ClassCategoryAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("classCategoryGet")]
        [HttpPost]
        public async Task<ResponseBase> ClassCategoryGet(ClassCategoryGetRequest request)
        {
            try
            {
                this._classCategoryBLL.InitTenantId(request.LoginTenantId);
                return await _classCategoryBLL.ClassCategoryGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("classCategoryDel")]
        [HttpPost]
        public async Task<ResponseBase> ClassCategoryDel(ClassCategoryDelRequest request)
        {
            try
            {
                this._classCategoryBLL.InitTenantId(request.LoginTenantId);
                return await _classCategoryBLL.ClassCategoryDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("giftCategoryAdd")]
        [HttpPost]
        public async Task<ResponseBase> GiftCategoryAdd(GiftCategoryAddRequest request)
        {
            try
            {
                this._giftCategoryBLL.InitTenantId(request.LoginTenantId);
                return await _giftCategoryBLL.GiftCategoryAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("giftCategoryGet")]
        [HttpPost]
        public async Task<ResponseBase> GiftCategoryGet(GiftCategoryGetRequest request)
        {
            try
            {
                this._giftCategoryBLL.InitTenantId(request.LoginTenantId);
                return await _giftCategoryBLL.GiftCategoryGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("giftCategoryDel")]
        [HttpPost]
        public async Task<ResponseBase> GiftCategoryDel(GiftCategoryDelRequest request)
        {
            try
            {
                this._giftCategoryBLL.InitTenantId(request.LoginTenantId);
                return await _giftCategoryBLL.GiftCategoryDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantConfigGet(TenantConfigGetRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.TenantConfigGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassCheckSignConfigSave(ClassCheckSignConfigSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.ClassCheckSignConfigSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StartClassNoticeSave(StartClassNoticeSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.StartClassNoticeSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserNoticeConfigSave(UserNoticeConfigSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.UserNoticeConfigSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserStartClassNoticeSave(UserStartClassNoticeSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.UserStartClassNoticeSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> UserWeChatNoticeRemarkSave(UserWeChatNoticeRemarkSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.UserWeChatNoticeRemarkSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WeChatNoticeRemarkSave(WeChatNoticeRemarkSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.WeChatNoticeRemarkSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentNoticeConfigSave(StudentNoticeConfigSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.StudentNoticeConfigSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseRenewalConfigSave(StudentCourseRenewalConfigSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.StudentCourseRenewalConfigSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> IncomeProjectTypeAdd(IncomeProjectTypeAddRequest request)
        {
            try
            {
                this._incomeProjectTypeBLL.InitTenantId(request.LoginTenantId);
                return await _incomeProjectTypeBLL.IncomeProjectTypeAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> IncomeProjectTypeGet(IncomeProjectTypeGetRequest request)
        {
            try
            {
                this._incomeProjectTypeBLL.InitTenantId(request.LoginTenantId);
                return await _incomeProjectTypeBLL.IncomeProjectTypeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> IncomeProjectTypeDel(IncomeProjectTypeDelRequest request)
        {
            try
            {
                this._incomeProjectTypeBLL.InitTenantId(request.LoginTenantId);
                return await _incomeProjectTypeBLL.IncomeProjectTypeDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> PrintConfigGet(PrintConfigGetRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.PrintConfigGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> PrintConfigSave(PrintConfigSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.PrintConfigSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentBannerGet(ParentBannerGetRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.ParentBannerGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentBannerSave(ParentBannerSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.ParentBannerSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantGet(TenantGetRequest request)
        {
            try
            {
                this._tenantBLL.InitTenantId(request.LoginTenantId);
                return await _tenantBLL.TenantGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseNotEnoughCountSave(StudentCourseNotEnoughCountSaveRequest request)
        {
            try
            {
                this._appConfigBLL.InitTenantId(request.LoginTenantId);
                return await _appConfigBLL.StudentCourseNotEnoughCountSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
