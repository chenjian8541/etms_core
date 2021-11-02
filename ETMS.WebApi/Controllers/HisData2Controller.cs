using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.Dto.HisData2.Request;
using ETMS.Entity.Dto.Student.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.SysOp;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/hisData2/[action]")]
    [ApiController]
    [Authorize]
    public class HisData2Controller : ControllerBase
    {
        private readonly IMallOrderBLL _mallOrderBLL;

        public HisData2Controller(IMallOrderBLL mallOrderBLL)
        {
            this._mallOrderBLL = mallOrderBLL;
        }

        public async Task<ResponseBase> MallOrderGetPaging(MallOrderGetPagingRequest request)
        {
            try
            {
                _mallOrderBLL.InitTenantId(request.LoginTenantId);
                return await _mallOrderBLL.MallOrderGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MallOrderGet(MallOrderGetRequest request)
        {
            try
            {
                _mallOrderBLL.InitTenantId(request.LoginTenantId);
                return await _mallOrderBLL.MallOrderGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
