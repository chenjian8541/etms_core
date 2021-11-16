using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Parent2.Request;
using ETMS.Entity.Dto.Parent3.Request;
using ETMS.Entity.Dto.Parent4.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.Parent;
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
    [Route("api/parent3/[action]")]
    [ApiController]
    [EtmsSignatureAuthorize]
    public class Parent3Controller : ControllerBase
    {
        private readonly IParentData4BLL _parentData4BLL;

        public Parent3Controller(IParentData4BLL parentData4BLL)
        {
            this._parentData4BLL = parentData4BLL;
        }

        public async Task<ResponseBase> ClassCanChooseGet(ClassCanChooseGetRequest request)
        {
            try
            {
                _parentData4BLL.InitTenantId(request.LoginTenantId);
                return await _parentData4BLL.ClassCanChooseGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentBuyMallGoodsPrepay(ParentBuyMallGoodsPrepayRequest request)
        {
            try
            {
                _parentData4BLL.InitTenantId(request.LoginTenantId);
                return await _parentData4BLL.ParentBuyMallGoodsPrepay(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentBuyMallGoodsPayInit(ParentBuyMallGoodsSubmitRequest request)
        {
            try
            {
                _parentData4BLL.InitTenantId(request.LoginTenantId);
                return await _parentData4BLL.ParentBuyMallGoodsPayInit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentBuyMallGoodsSubmit(ParentBuyMallGoodsSubmitRequest request)
        {
            try
            {
                _parentData4BLL.InitTenantId(request.LoginTenantId);
                return await _parentData4BLL.ParentBuyMallGoodsSubmit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MallGoodsGetPaging(MallGoodsGetPagingRequest request)
        {
            try
            {
                _parentData4BLL.InitTenantId(request.LoginTenantId);
                return await _parentData4BLL.MallGoodsGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
