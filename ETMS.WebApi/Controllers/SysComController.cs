﻿using ETMS.Entity.Common;
using ETMS.Entity.Dto.SysCom.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/sysCom/[action]")]
    [ApiController]
    [Authorize]
    public class SysComController : ControllerBase
    {
        private readonly ISysComBLL _sysComBLL;

        public SysComController(ISysComBLL sysComBLL)
        {
            this._sysComBLL = sysComBLL;
        }

        public async Task<ResponseBase> SysUpgradeGet(SysUpgradeGetRequest request)
        {
            try
            {
                _sysComBLL.InitTenantId(request.LoginTenantId);
                return await _sysComBLL.SysUpgradeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysUpgradeSetRead(SysUpgradeSetReadRequest request)
        {
            try
            {
                _sysComBLL.InitTenantId(request.LoginTenantId);
                return await _sysComBLL.SysUpgradeSetRead(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
