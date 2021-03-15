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
    [Route("api/educational2/[action]")]
    [ApiController]
    [Authorize]
    public class Educational2Controller : ControllerBase
    {
        private readonly IClassReservationBLL _classReservationBLL;

        public Educational2Controller(IClassReservationBLL classReservationBLL)
        {
            this._classReservationBLL = classReservationBLL;
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
    }
}
