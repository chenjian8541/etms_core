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
    [Route("api/student3/[action]")]
    [ApiController]
    [Authorize]
    public class Student3Controller : ControllerBase
    {
        private readonly IStudent3BLL _student3BLL;

        public Student3Controller(IStudent3BLL student3BLL)
        {
            this._student3BLL = student3BLL;
        }

        public async Task<ResponseBase> SendToClassNotice(SendToClassNoticeRequest request)
        {
            try
            {
                _student3BLL.InitTenantId(request.LoginTenantId);
                return await _student3BLL.SendToClassNotice(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
