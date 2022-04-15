using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
using ETMS.Entity.EtmsManage.Dto.Agent3.Request;
using ETMS.Entity.EtmsManage.Dto.DataLog.Request;
using ETMS.Entity.EtmsManage.Dto.Explain.Request;
using ETMS.Entity.EtmsManage.Dto.Head.Request;
using ETMS.Entity.EtmsManage.Dto.SysCommon.Request;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
using ETMS.Entity.EtmsManage.Dto.User.Request;
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
    [Route("api/agent4/[action]")]
    [ApiController]
    [Authorize]
    public class Agent4Controller : ControllerBase
    {
        private readonly IHeadBLL _headBLL;

        public Agent4Controller(IHeadBLL headBLL)
        {
            this._headBLL = headBLL;
        }

        public async Task<ResponseBase> HeadGetSimple(HeadGetRequest request)
        {
            try
            {
                return await _headBLL.HeadGetSimple(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HeadGet(HeadGetRequest request)
        {
            try
            {
                return await _headBLL.HeadGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HeadGetPaging(HeadGetPagingRequest request)
        {
            try
            {
                return await _headBLL.HeadGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HeadAdd(HeadAddRequest request)
        {
            try
            {
                return await _headBLL.HeadAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HeadEdit(HeadEditRequest request)
        {
            try
            {
                return await _headBLL.HeadEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HeadDel(HeadDelRequest request)
        {
            try
            {
                return await _headBLL.HeadDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HeadAddTenant(HeadAddTenantRequest request)
        {
            try
            {
                return await _headBLL.HeadAddTenant(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HeadRemoveTenant(HeadRemoveTenantRequest request)
        {
            try
            {
                return await _headBLL.HeadRemoveTenant(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HeadUserOpLogGetPaging(HeadUserOpLogGetPagingRequest request)
        {
            try
            {
                return await _headBLL.HeadUserOpLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
