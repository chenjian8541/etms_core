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
    [Route("api/student2/[action]")]
    [ApiController]
    [Authorize]
    public class Student2Controller : ControllerBase
    {
        private readonly IStudentAccountRechargeBLL _studentAccountRechargeBLL;

        public Student2Controller(IStudentAccountRechargeBLL studentAccountRechargeBLL)
        {
            this._studentAccountRechargeBLL = studentAccountRechargeBLL;
        }

        public async Task<ResponseBase> StudentAccountRechargeRuleGet(StudentAccountRechargeRuleGetRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeRuleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeRuleSave(StudentAccountRechargeRuleSaveRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeRuleSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StatisticsStudentAccountRechargeGet(StatisticsStudentAccountRechargeGetRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StatisticsStudentAccountRechargeGet(request);
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
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeGetRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeGetPaging(StudentAccountRechargeGetPagingRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeGetByPhone(StudentAccountRechargeGetByPhoneRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeGetByPhone(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeGetByStudentId(StudentAccountRechargeGetByStudentIdRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeGetByStudentId(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
        public async Task<ResponseBase> StudentAccountRechargeCreate(StudentAccountRechargeCreateRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeCreate(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeEditPhone(StudentAccountRechargeEditPhoneRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeEditPhone(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRecharge(StudentAccountRechargeRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRecharge(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRefund(StudentAccountRefundRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRefund(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeGetDetail(StudentAccountRechargeGetDetailRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeGetDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeBinderAdd(StudentAccountRechargeBinderAddRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeBinderAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentAccountRechargeBinderRemove(StudentAccountRechargeBinderRemoveRequest request)
        {
            try
            {
                _studentAccountRechargeBLL.InitTenantId(request.LoginTenantId);
                return await _studentAccountRechargeBLL.StudentAccountRechargeBinderRemove(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
