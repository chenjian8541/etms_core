using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Parent2.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using ETMS.WebApi.FilterAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/parent2/[action]")]
    [ApiController]
    [EtmsSignatureParentAuthorize]
    public class Parent2Controller : ControllerBase
    {
        private readonly IParentData3BLL _parentData3BLL;

        public Parent2Controller(IParentData3BLL parentData3BLL)
        {
            this._parentData3BLL = parentData3BLL;
        }

        public async Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeGetRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentAccountRechargeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeRuleGet(StudentAccountRechargeRuleGetRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentAccountRechargeRuleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeLogGetPaging(StudentAccountRechargeLogGetPagingRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentAccountRechargeLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherGetPaging(TeacherGetPagingRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.TeacherGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservationTimetable(StudentReservationTimetableRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentReservationTimetable(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservationTimetableDetail(StudentReservationTimetableDetailRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentReservationTimetableDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservationDetail(StudentReservationDetailRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentReservationDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservationLogGetPaging(StudentReservationLogGetPagingRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentReservationLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservationLogGetPaging2(StudentReservationLogGetPaging2Request request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentReservationLogGetPaging2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservationSubmit(StudentReservationSubmitRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentReservationSubmit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservationCancel(StudentReservationCancelRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.StudentReservationCancel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
