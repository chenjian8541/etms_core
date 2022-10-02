using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.EtmsManage;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/basic3/[action]")]
    [ApiController]
    [Authorize]
    public class BasicData3Controller : ControllerBase
    {
        private readonly ITenant2BLL _tenant2BLL;

        public BasicData3Controller(ITenant2BLL tenant2BLL)
        {
            this._tenant2BLL = tenant2BLL;
        }

        public async Task<ResponseBase> TenantStatisticsGet(RequestBase request)
        {
            try
            {
                this._tenant2BLL.InitTenantId(request.LoginTenantId);
                return await _tenant2BLL.TenantStatisticsGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityConfigGet(RequestBase request)
        {
            try
            {
                this._tenant2BLL.InitTenantId(request.LoginTenantId);
                return await _tenant2BLL.ActivityConfigGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityConfigSave(ActivityConfigSaveRequest request)
        {
            try
            {
                this._tenant2BLL.InitTenantId(request.LoginTenantId);
                return await _tenant2BLL.ActivityConfigSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase TenantCustomized(RequestBase request)
        {
            try
            {
                this._tenant2BLL.InitTenantId(request.LoginTenantId);
                return _tenant2BLL.TenantCustomized(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
