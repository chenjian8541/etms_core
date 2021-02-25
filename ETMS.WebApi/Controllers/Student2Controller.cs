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
    }
}
