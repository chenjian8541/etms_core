using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
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
    [Route("api/student/[action]")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentBLL _studentBLL;

        private readonly IStudentContractsBLL _studentContractsBLL;

        private readonly IStudentCourseBLL _studentCourseBLL;

        private readonly IStudentPointBLL _studentPointBLL;

        private readonly IStudent2BLL _student2BLL;

        public StudentController(IStudentBLL studentBLL, IStudentContractsBLL studentContractsBLL, IStudentCourseBLL studentCourseBLL,
            IStudentPointBLL studentPointBLL, IStudent2BLL student2BLL)
        {
            this._studentBLL = studentBLL;
            this._studentContractsBLL = studentContractsBLL;
            this._studentCourseBLL = studentCourseBLL;
            this._studentPointBLL = studentPointBLL;
            this._student2BLL = student2BLL;
        }

        [ActionName("studentAdd")]
        [HttpPost]
        public async Task<ResponseBase> StudentAdd(StudentAddRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentEdit")]
        [HttpPost]
        public async Task<ResponseBase> StudentEdit(StudentEditRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentDel")]
        [HttpPost]
        public async Task<ResponseBase> StudentDel(StudentDelRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentGet")]
        [HttpPost]
        public async Task<ResponseBase> StudentGet(StudentGetRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentGetForEdit(StudentGetForEditReuqest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentGetForEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentGetPaging")]
        [HttpPost]
        public async Task<ResponseBase> StudentGetPaging(StudentGetPagingRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentSetTrackUser")]
        [HttpPost]
        public async Task<ResponseBase> StudentSetTrackUser(StudentSetTrackUserRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentSetTrackUser(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentSetLearningManager")]
        [HttpPost]
        public async Task<ResponseBase> StudentSetLearningManager(StudentSetLearningManagerRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentSetLearningManager(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentTrackLogAdd")]
        [HttpPost]
        public async Task<ResponseBase> StudentTrackLogAdd(StudentTrackLogAddRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentTrackLogAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentTrackLogGetLast")]
        [HttpPost]
        public async Task<ResponseBase> StudentTrackLogGetLast(StudentTrackLogGetLastRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentTrackLogGetLast(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentTrackLogGetList")]
        [HttpPost]
        public async Task<ResponseBase> StudentTrackLogGetList(StudentTrackLogGetListRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentTrackLogGetList(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentTrackLogDel")]
        [HttpPost]
        public async Task<ResponseBase> StudentTrackLogDel(StudentTrackLogDelRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentTrackLogDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentTrackLogGetPaging")]
        [HttpPost]
        public async Task<ResponseBase> StudentTrackLogGetPaging(StudentTrackLogGetPagingRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentTrackLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentOperationLogPaging")]
        [HttpPost]
        public async Task<ResponseBase> StudentOperationLogPaging(StudentOperationLogPagingRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentOperationLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [ActionName("studentOperationLogTypeGet")]
        [HttpPost]
        public ResponseBase StudentOperationLogTypeGet(RequestBase request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return _studentBLL.StudentOperationLogTypeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentEnrolment(StudentEnrolmentRequest request)
        {
            try
            {
                _studentContractsBLL.InitTenantId(request.LoginTenantId);
                return await _studentContractsBLL.StudentEnrolment(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseGetPaging(StudentCourseGetPagingRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseOwnerGetPaging(StudentCourseOwnerGetPagingRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseOwnerGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseDetailGet(StudentCourseDetailGetRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentLeaveApplyLogPaging(StudentLeaveApplyLogPagingRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentLeaveApplyLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentLeaveApplyHandle(StudentLeaveApplyHandleRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentLeaveApplyHandle(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentExtendFieldInit(StudentExtendFieldInitRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentExtendFieldInit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentMarkGraduation(StudentMarkGraduationRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentMarkGraduation(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentMarkReading(StudentMarkReadingRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentMarkReading(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentMarkHidden(StudentMarkHiddenRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentMarkHidden(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseStop(StudentCourseStopRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseStop(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseRestoreTime(StudentCourseRestoreTimeRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseRestoreTime(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseMarkExceedClassTimes(StudentCourseMarkExceedClassTimesRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseMarkExceedClassTimes(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseSetExpirationDate(StudentCourseSetExpirationDateRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseSetExpirationDate(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseClassOver(StudentCourseClassOverRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseClassOver(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseChangeTimes(StudentCourseChangeTimesRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseChangeTimes(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentPointLogPaging(StudentPointLogPagingRequest request)
        {
            try
            {
                _studentPointBLL.InitTenantId(request.LoginTenantId);
                return await _studentPointBLL.StudentPointLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseSurplusGet(StudentCourseSurplusGetRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseSurplusGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseConsumeLogGetPaging(StudentCourseConsumeLogGetPagingRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseConsumeLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseHasGet(StudentCourseHasGetRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseHasGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseHasDetailGet(StudentCourseHasDetailGetRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return await _studentCourseBLL.StudentCourseHasDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        /// <summary>
        /// 学员课时不足提醒
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseBase StudentCourseNotEnoughRemind(StudentCourseNotEnoughRemindRequest request)
        {
            try
            {
                _studentCourseBLL.InitTenantId(request.LoginTenantId);
                return _studentCourseBLL.StudentCourseNotEnoughRemind(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentLeaveApplyPassGet(StudentLeaveApplyPassGetRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentLeaveApplyPassGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentLeaveAboutClassCheckSignGet(StudentLeaveAboutClassCheckSignGetRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentLeaveAboutClassCheckSignGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentGetByCardNo(StudentGetByCardNoRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentGetByCardNo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentRelieveCardNo(StudentRelieveCardNoRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentRelieveCardNo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentBindingCardNo(StudentBindingCardNoRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentBindingCardNo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentChangePoints(StudentChangePointsRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentChangePoints(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentFaceListGet(StudentFaceListGetRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentFaceListGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentRelieveFace(StudentRelieveFaceKeyRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentRelieveFace(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentBindingFace(StudentBindingFaceKeyRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentBindingFace(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCheckOnLogGetPaging(StudentCheckOnLogGetPagingRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentCheckOnLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCheckByCard(StudentCheckByCardRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentCheckByCard(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCheckByTeacher(StudentCheckByTeacherRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentCheckByTeacher(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCheckByFace(StudentCheckByFaceRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentCheckByFace(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCheckByFace2(StudentCheckByFace2Request request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentCheckByFace2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCheckChoiceClass(StudentCheckChoiceClassRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentCheckChoiceClass(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCheckOnLogRevoke(StudentCheckOnLogRevokeRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentCheckOnLogRevoke(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentNeedCheckStatistics(StudentNeedCheckStatisticsRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentNeedCheckStatistics(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentNeedCheckInGetPaging(StudentNeedCheckInGetPagingRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentNeedCheckInGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentNeedCheckOutGetPaging(StudentNeedCheckOutGetPagingRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentNeedCheckOutGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentNeedAttendClassGetPaging(StudentNeedAttendClassGetPagingRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentNeedAttendClassGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentNeedLogCheckIn(StudentNeedLogCheckInRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentNeedLogCheckIn(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentNeedLogCheckOut(StudentNeedLogCheckOutRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentNeedLogCheckOut(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentNeedLogAttendClass(StudentNeedLogAttendClassRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentNeedLogAttendClass(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCheckOnLogDel(StudentCheckOnLogDelRequest request)
        {
            try
            {
                _student2BLL.InitTenantId(request.LoginTenantId);
                return await _student2BLL.StudentCheckOnLogDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
