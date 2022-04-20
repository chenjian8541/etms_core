using ETMS.Entity.Alien.Common;
using ETMS.Entity.Alien.Dto.Tenant.Request;
using ETMS.Entity.Alien.Dto.TenantStatistics.Request;
using ETMS.Entity.Alien.Dto.User.Request;
using ETMS.Entity.Common;
using ETMS.IBusiness.Alien;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETMS.Alien.Webapi.Controllers
{
    [Route("api/tenantStats2/[action]")]
    [ApiController]
    [Authorize]
    public class TenantStats2Controller : ControllerBase
    {
        private readonly IAlienTenantStatistics3BLL _alienTenantStatistics3BLL;

        public TenantStats2Controller(IAlienTenantStatistics3BLL alienTenantStatistics3BLL)
        {
            this._alienTenantStatistics3BLL = alienTenantStatistics3BLL;
        }

        public async Task<ResponseBase> AlienTenantStatisticsClassTimesGet(AlienTenantStatisticsClassTimesGetRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsClassTimesGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantStatisticsClassAttendanceTagGet(AlienTenantStatisticsClassAttendanceTagGetRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsClassAttendanceTagGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantStatisticsClassAttendanceGet(AlienTenantStatisticsClassAttendanceGetRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsClassAttendanceGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantStatisticsEducationMonthGet(AlienTenantStatisticsEducationMonthGetRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsEducationMonthGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantStatisticsEducationStudentMonthGetPaging(AlienTenantStatisticsEducationStudentMonthGetPagingRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsEducationStudentMonthGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantStatisticsEducationTeacherMonthGetPaging(AlienTenantStatisticsEducationTeacherMonthGetPagingRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsEducationTeacherMonthGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantStatisticsEducationCourseMonthGetPaging(AlienTenantStatisticsEducationCourseMonthGetPagingRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsEducationCourseMonthGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantStatisticsEducationClassMonthGetPaging(AlienTenantStatisticsEducationClassMonthGetPagingRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsEducationClassMonthGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantClassRecordGetPaging(AlienTenantClassRecordGetPagingRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantClassRecordGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantClassRecordGet(AlienTenantClassRecordGetRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantClassRecordGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantClassRecordStudentGet(AlienTenantClassRecordStudentGetRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                _alienTenantStatistics3BLL.InitTenant(request.TenantId.Value);
                return await _alienTenantStatistics3BLL.AlienTenantClassRecordStudentGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }


        public async Task<ResponseBase> AlienTenantStatisticsWeekGet(AlienTenantStatisticsWeekGetRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsWeekGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlienTenantStatisticsMonthGet(AlienTenantStatisticsMonthGetRequest request)
        {
            try
            {
                _alienTenantStatistics3BLL.InitHeadId(request.LoginHeadId);
                return await _alienTenantStatistics3BLL.AlienTenantStatisticsMonthGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

    }
}
