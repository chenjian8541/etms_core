using ETMS.Entity.Alien.Common;
using ETMS.Entity.Alien.Dto.Tenant.Request;
using ETMS.Entity.Alien.Dto.User.Request;
using ETMS.Entity.Common;
using ETMS.IBusiness.Alien;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETMS.Alien.Webapi.Controllers
{
    [Route("api/tenant/[action]")]
    [ApiController]
    [Authorize]
    public class TenantController : ControllerBase
    {
        private readonly IAlienTenantBLL _alienTenantBLL;

        public TenantController(IAlienTenantBLL alienTenantBLL)
        {
            this._alienTenantBLL = alienTenantBLL;
        }

        public async Task<ResponseBase> TenantOperationLogPaging(TenantOperationLogPagingRequest request)
        {
            try
            {
                _alienTenantBLL.InitHeadId(request.LoginHeadId);
                return await _alienTenantBLL.TenantOperationLogPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentGetPaging(AlStudentGetPagingRequest request)
        {
            try
            {
                _alienTenantBLL.InitHeadId(request.LoginHeadId);
                return await _alienTenantBLL.StudentGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassGetPaging(AlClassGetPagingRequest request)
        {
            try
            {
                _alienTenantBLL.InitHeadId(request.LoginHeadId);
                return await _alienTenantBLL.ClassGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CourseGetPaging(AlCourseGetPagingRequest request)
        {
            try
            {
                _alienTenantBLL.InitHeadId(request.LoginHeadId);
                return await _alienTenantBLL.CourseGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantRoleGet(TenantRoleGetRequest request)
        {
            try
            {
                _alienTenantBLL.InitHeadId(request.LoginHeadId);
                return await _alienTenantBLL.TenantRoleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantUserAdd(TenantUserAddRequest request)
        {
            try
            {
                _alienTenantBLL.InitHeadId(request.LoginHeadId);
                return await _alienTenantBLL.TenantUserAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
