using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Dto.Educational2.Request;
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
    [Route("api/educational2/[action]")]
    [ApiController]
    [Authorize]
    public class Educational2Controller : ControllerBase
    {
        private readonly IClassReservationBLL _classReservationBLL;

        private readonly ITeacherSchooltimeConfigBLL _teacherSchooltimeConfigBLL;
        public Educational2Controller(IClassReservationBLL classReservationBLL, ITeacherSchooltimeConfigBLL teacherSchooltimeConfigBLL)
        {
            this._classReservationBLL = classReservationBLL;
            this._teacherSchooltimeConfigBLL = teacherSchooltimeConfigBLL;
        }

        public async Task<ResponseBase> ClassReservationRuleGet(RequestBase request)
        {
            try
            {
                _classReservationBLL.InitTenantId(request.LoginTenantId);
                return await _classReservationBLL.ClassReservationRuleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassReservationRuleSave(ClassReservationRuleSaveRequest request)
        {
            try
            {
                _classReservationBLL.InitTenantId(request.LoginTenantId);
                return await _classReservationBLL.ClassReservationRuleSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassReservationLogGetPaging(ClassReservationLogGetPagingRequest request)
        {
            try
            {
                _classReservationBLL.InitTenantId(request.LoginTenantId);
                return await _classReservationBLL.ClassReservationLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ReservationCourseSetGet(RequestBase request)
        {
            try
            {
                _classReservationBLL.InitTenantId(request.LoginTenantId);
                return await _classReservationBLL.ReservationCourseSetGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ReservationCourseSetAdd(ReservationCourseSetAddRequest request)
        {
            try
            {
                _classReservationBLL.InitTenantId(request.LoginTenantId);
                return await _classReservationBLL.ReservationCourseSetAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ReservationCourseSetEdit(ReservationCourseSetEditRequest request)
        {
            try
            {
                _classReservationBLL.InitTenantId(request.LoginTenantId);
                return await _classReservationBLL.ReservationCourseSetEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ReservationCourseSetDel(ReservationCourseSetDelRequest request)
        {
            try
            {
                _classReservationBLL.InitTenantId(request.LoginTenantId);
                return await _classReservationBLL.ReservationCourseSetDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigGetPaging(TeacherSchooltimeConfigGetPagingRequest request)
        {
            try
            {
                _teacherSchooltimeConfigBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSchooltimeConfigBLL.TeacherSchooltimeConfigGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigGet(TeacherSchooltimeConfigGetRequest request)
        {
            try
            {
                _teacherSchooltimeConfigBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSchooltimeConfigBLL.TeacherSchooltimeConfigGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigAdd(TeacherSchooltimeConfigAddRequest request)
        {
            try
            {
                _teacherSchooltimeConfigBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSchooltimeConfigBLL.TeacherSchooltimeConfigAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigDel(TeacherSchooltimeConfigDelRequest request)
        {
            try
            {
                _teacherSchooltimeConfigBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSchooltimeConfigBLL.TeacherSchooltimeConfigDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigExcludeSave(TeacherSchooltimeConfigExcludeSaveRequest request)
        {
            try
            {
                _teacherSchooltimeConfigBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSchooltimeConfigBLL.TeacherSchooltimeConfigExcludeSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSchooltimeSetBatch(TeacherSchooltimeSetBatchRequest request)
        {
            try
            {
                _teacherSchooltimeConfigBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSchooltimeConfigBLL.TeacherSchooltimeSetBatch(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
