using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
using ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Request;
using ETMS.Entity.EtmsManage.Dto.Explain.Request;
using ETMS.Entity.EtmsManage.Dto.SysCommon.Request;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etms.Agent.WebApi.Controllers
{
    [Route("api/basicData/[action]")]
    [ApiController]
    [Authorize]
    public class BasicDataController : ControllerBase
    {
        private readonly ISysClientUpgradeBLL _sysClientUpgradeBLL;

        public BasicDataController(ISysClientUpgradeBLL sysClientUpgradeBLL)
        {
            this._sysClientUpgradeBLL = sysClientUpgradeBLL;
        }

        public async Task<ResponseBase> SysClientUpgradeAdd(SysClientUpgradeAddRequest request)
        {
            try
            {
                return await _sysClientUpgradeBLL.SysClientUpgradeAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysClientUpgradeEdit(SysClientUpgradeEditRequest request)
        {
            try
            {
                return await _sysClientUpgradeBLL.SysClientUpgradeEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysClientUpgradeDel(SysClientUpgradeDelRequest request)
        {
            try
            {
                return await _sysClientUpgradeBLL.SysClientUpgradeDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysClientUpgradePaging(SysClientUpgradePagingRequest request)
        {
            try
            {
                return await _sysClientUpgradeBLL.SysClientUpgradePaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
