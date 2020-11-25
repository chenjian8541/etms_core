using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
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
    [Route("api/educational/[action]")]
    [ApiController]
    [Authorize]
    public class EducationalController : ControllerBase
    {
        private readonly IClassBLL _classBLL;

        private readonly IClassTimesBLL _classTimesBLL;

        private readonly IClassCheckSignBLL _classCheckSignBLL;

        private readonly IClassRecordBLL _classRecordBLL;

        private readonly ITryCalssLogBLL _tryCalssLogBLL;

        private readonly ITryCalssApplyLogBLL _tryCalssApplyLogBLL;

        private readonly IClassCheckSignRevokeBLL _classCheckSignRevokeBLL;

        public EducationalController(IClassBLL classBLL, IClassTimesBLL classTimesBLL, IClassCheckSignBLL classCheckSignBLL, IClassRecordBLL classRecordBLL,
            ITryCalssLogBLL tryCalssLogBLL, ITryCalssApplyLogBLL tryCalssApplyLogBLL, IClassCheckSignRevokeBLL classCheckSignRevokeBLL)
        {
            this._classBLL = classBLL;
            this._classTimesBLL = classTimesBLL;
            this._classCheckSignBLL = classCheckSignBLL;
            this._classRecordBLL = classRecordBLL;
            this._tryCalssLogBLL = tryCalssLogBLL;
            this._tryCalssApplyLogBLL = tryCalssApplyLogBLL;
            this._classCheckSignRevokeBLL = classCheckSignRevokeBLL;
        }

        public async Task<ResponseBase> ClassAdd(ClassAddRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassEdit(ClassEditRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassGet(ClassGetRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassViewGet(ClassViewGetRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassViewGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassBascGet(ClassBascGetRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassBascGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassDel(ClassDelRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassOverOneToMany(ClassOverRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassOverOneToMany(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassOverOneToOne(ClassOverOneToOneRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassOverOneToOne(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassSetTeachers(SetClassTeachersRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassSetTeachers(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassGetPaging(ClassGetPagingRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassStudentAdd(ClassStudentAddRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassStudentAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassStudentRemove(ClassStudentRemoveRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassStudentRemove(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassStudentGet(ClassStudentGetRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassStudentGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesRuleAdd1(ClassTimesRuleAdd1Request request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassTimesRuleAdd1(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesRuleAdd2(ClassTimesRuleAdd2Request request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassTimesRuleAdd2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesRuleDel(ClassTimesRuleDelRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassTimesRuleDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesRuleGet(ClassTimesRuleGetRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassTimesRuleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesGetView(ClassTimesGetViewRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesGetView(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesStudentGet(ClassTimesStudentGetRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesStudentGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesClassStudentGet(ClassTimesClassStudentGetRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesClassStudentGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesGetEditView(ClassTimesGetEditViewRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesGetEditView(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesEdit(ClassTimesEditRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesDel(ClassTimesDelRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesAddTempStudent(ClassTimesAddTempStudentRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesAddTempStudent(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesAddTryStudent2(ClassTimesAddTryStudent2Request request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesAddTryStudent2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesAddTryStudent(ClassTimesAddTryStudentRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesAddTryStudent(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesAddTryStudentOneToOne(ClassTimesAddTryStudentOneToOneRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesAddTryStudentOneToOne(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesDelTempOrTryStudent(ClassTimesDelTempOrTryStudentRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesDelTempOrTryStudent(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesGetPaging(ClassTimesGetPagingRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesGetOfWeekTime(ClassTimesGetOfWeekTimeRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesGetOfWeekTime(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesGetMy(ClassTimesGetMyRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesGetMyWeek(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesGetMyOt(ClassTimesGetMyOtRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesGetMyOt(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesGetOfWeekTimeTeacher(ClassTimesGetOfWeekTimeTeacherRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesGetOfWeekTimeTeacher(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesGetOfWeekTimeRoom(ClassTimesGetOfWeekTimeRoomRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesGetOfWeekTimeRoom(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassCheckSign(ClassCheckSignRequest request)
        {
            try
            {
                _classCheckSignBLL.InitTenantId(request.LoginTenantId);
                return await _classCheckSignBLL.ClassCheckSign(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordGetPaging(ClassRecordGetPagingRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordGetPagingH5(ClassRecordGetPagingH5Request request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordGetPagingH5(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordGet(ClassRecordGetRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordStudentGet(ClassRecordStudentGetRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordStudentGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordOperationLogGet(ClassRecordOperationLogGetRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordOperationLogGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordAbsenceLogPaging(ClassRecordAbsenceLogPagingRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordAbsenceLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordAbsenceLogHandle(ClassRecordAbsenceLogHandleRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordAbsenceLogHandle(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordPointsApplyHandle(ClassRecordPointsApplyHandleRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordPointsApplyHandle(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentClassRecordGetPaging(StudentClassRecordGetPagingRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.StudentClassRecordGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordPointsApplyLogPaging(ClassRecordPointsApplyLogPagingRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordPointsApplyLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordStudentChange(ClassRecordStudentChangeRequest request)
        {
            try
            {
                _classRecordBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordBLL.ClassRecordStudentChange(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryCalssLogGetPaging(TryCalssLogGetPagingRequest request)
        {
            try
            {
                _tryCalssLogBLL.InitTenantId(request.LoginTenantId);
                return await _tryCalssLogBLL.TryCalssLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassTimesCancelTryClassStudent(ClassTimesCancelTryClassStudentRequest request)
        {
            try
            {
                _classTimesBLL.InitTenantId(request.LoginTenantId);
                return await _classTimesBLL.ClassTimesCancelTryClassStudent(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryCalssApplyLogPaging(TryCalssApplyLogPagingRequest request)
        {
            try
            {
                _tryCalssApplyLogBLL.InitTenantId(request.LoginTenantId);
                return await _tryCalssApplyLogBLL.TryCalssApplyLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryCalssApplyLogHandle(TryCalssApplyLogHandleRequest request)
        {
            try
            {
                _tryCalssApplyLogBLL.InitTenantId(request.LoginTenantId);
                return await _tryCalssApplyLogBLL.TryCalssApplyLogHandle(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassCheckSignRevoke(ClassCheckSignRevokeRequest request)
        {
            try
            {
                _classCheckSignRevokeBLL.InitTenantId(request.LoginTenantId);
                return await _classCheckSignRevokeBLL.ClassCheckSignRevoke(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassMyGet(ClassMyGetRequest request)
        {
            try
            {
                _classBLL.InitTenantId(request.LoginTenantId);
                return await _classBLL.ClassMyGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
