using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Dto.Parent.Request;
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
    [EtmsSignatureAuthorize]
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
    }
}
