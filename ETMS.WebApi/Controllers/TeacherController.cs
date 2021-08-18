using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Dto.Teacher.Request;
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
    [Route("api/teacher/[action]")]
    [ApiController]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherSalaryBLL _teacherSalaryBLL;

        public TeacherController(ITeacherSalaryBLL teacherSalaryBLL)
        {
            this._teacherSalaryBLL = teacherSalaryBLL;
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsGet(TeacherSalaryFundsItemsGetRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryFundsItemsGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsAdd(TeacherSalaryFundsItemsAddRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryFundsItemsAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsDel(TeacherSalaryFundsItemsDelRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryFundsItemsDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryFundsItemsChangeStatus(TeacherSalaryFundsItemsChangeStatusRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryFundsItemsChangeStatus(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryClassDayGetPaging(TeacherSalaryClassDayGetPagingRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryClassDayGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryGlobalRuleGet(TeacherSalaryGlobalRuleGetRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryGlobalRuleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryPerformanceRuleSave(TeacherSalaryPerformanceRuleSaveRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryPerformanceRuleSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryIncludeArrivedRuleSave(TeacherSalaryIncludeArrivedRuleSaveRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryIncludeArrivedRuleSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryContractGetPaging(TeacherSalaryContractGetPagingRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryContractGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryContractGetDetail(TeacherSalaryContractGetDetailRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryContractGetDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryContractChangeComputeType(TeacherSalaryContractChangeComputeTypeRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryContractChangeComputeType(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryContractSave(TeacherSalaryContractSaveRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryContractSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryPayrollGoSettlement(TeacherSalaryPayrollGoSettlementRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryPayrollGoSettlement(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryPayrollGetPaging(TeacherSalaryPayrollGetPagingRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryPayrollGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryPayrollGet(TeacherSalaryPayrollGetRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryPayrollGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryPayrollSetOK(TeacherSalaryPayrollSetOKRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryPayrollSetOK(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryPayrollDel(TeacherSalaryPayrollDelRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryPayrollDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherSalaryPayrollRepeal(TeacherSalaryPayrollRepealRequest request)
        {
            try
            {
                _teacherSalaryBLL.InitTenantId(request.LoginTenantId);
                return await _teacherSalaryBLL.TeacherSalaryPayrollRepeal(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
