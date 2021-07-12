using ETMS.Entity.Common;
using ETMS.Entity.Dto.Product.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/product/[action]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IGoodsBLL _goodsBLL;

        private readonly ICostBLL _costBLL;

        private readonly ICourseBLL _courseBLL;

        private readonly ISuitBLL _suitBLL;

        public ProductController(IGoodsBLL goodsBLL, ICostBLL costBLL, ICourseBLL courseBLL, ISuitBLL suitBLL)
        {
            this._goodsBLL = goodsBLL;
            this._costBLL = costBLL;
            this._courseBLL = courseBLL;
            this._suitBLL = suitBLL;
        }

        [HttpPost]
        public async Task<ResponseBase> GoodsAdd(GoodsAddRequest request)
        {
            try
            {
                _goodsBLL.InitTenantId(request.LoginTenantId);
                return await _goodsBLL.GoodsAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> GoodsEdit(GoodsEditRequest request)
        {
            try
            {
                _goodsBLL.InitTenantId(request.LoginTenantId);
                return await _goodsBLL.GoodsEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> GoodsGet(GoodsGetRequest request)
        {
            try
            {
                _goodsBLL.InitTenantId(request.LoginTenantId);
                return await _goodsBLL.GoodsGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> GoodsDel(GoodsDelRequest request)
        {
            try
            {
                _goodsBLL.InitTenantId(request.LoginTenantId);
                return await _goodsBLL.GoodsDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> GoodsStatusChange(GoodsStatusChangeRequest request)
        {
            try
            {
                _goodsBLL.InitTenantId(request.LoginTenantId);
                return await _goodsBLL.GoodsStatusChange(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> GoodsGetPaging(GoodsGetPagingRequest request)
        {
            try
            {
                _goodsBLL.InitTenantId(request.LoginTenantId);
                return await _goodsBLL.GoodsGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> GoodsInventoryLogAdd(GoodsInventoryLogAddRequest request)
        {
            try
            {
                _goodsBLL.InitTenantId(request.LoginTenantId);
                return await _goodsBLL.GoodsInventoryLogAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> GoodsInventoryLogGetPaging(GoodsInventoryLogGetPagingRequest request)
        {
            try
            {
                _goodsBLL.InitTenantId(request.LoginTenantId);
                return await _goodsBLL.GoodsInventoryLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CostAdd(CostAddRequest request)
        {
            try
            {
                _costBLL.InitTenantId(request.LoginTenantId);
                return await _costBLL.CostAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CostEdit(CostEditRequest request)
        {
            try
            {
                _costBLL.InitTenantId(request.LoginTenantId);
                return await _costBLL.CostEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CostGet(CostGetRequest request)
        {
            try
            {
                _costBLL.InitTenantId(request.LoginTenantId);
                return await _costBLL.CostGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CostDel(CostDelRequest request)
        {
            try
            {
                _costBLL.InitTenantId(request.LoginTenantId);
                return await _costBLL.CostDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CostStatusChange(CostStatusChangeRequest request)
        {
            try
            {
                _costBLL.InitTenantId(request.LoginTenantId);
                return await _costBLL.CostStatusChange(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CostGetPaging(CostGetPagingRequest request)
        {
            try
            {
                _costBLL.InitTenantId(request.LoginTenantId);
                return await _costBLL.CostGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CourseAdd(CourseAddRequest request)
        {
            try
            {
                _courseBLL.InitTenantId(request.LoginTenantId);
                return await _courseBLL.CourseAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CourseEdit(CourseEditRequest request)
        {
            try
            {
                _courseBLL.InitTenantId(request.LoginTenantId);
                return await _courseBLL.CourseEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CourseGet(CourseGetRequest request)
        {
            try
            {
                _courseBLL.InitTenantId(request.LoginTenantId);
                return await _courseBLL.CourseGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CourseDel(CourseDelRequest request)
        {
            try
            {
                _courseBLL.InitTenantId(request.LoginTenantId);
                return await _courseBLL.CourseDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CourseChangeStatus(CourseChangeStatusRequest request)
        {
            try
            {
                _courseBLL.InitTenantId(request.LoginTenantId);
                return await _courseBLL.CourseChangeStatus(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CourseGetPaging(CourseGetPagingRequest request)
        {
            try
            {
                _courseBLL.InitTenantId(request.LoginTenantId);
                return await _courseBLL.CourseGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CourseGetPagingSimple(CourseGetPagingRequest request)
        {
            try
            {
                _courseBLL.InitTenantId(request.LoginTenantId);
                return await _courseBLL.CourseGetPagingSimple(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CourseViewGet(CourseViewGetRequest request)
        {
            try
            {
                _courseBLL.InitTenantId(request.LoginTenantId);
                return await _courseBLL.CourseViewGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SuitAdd(SuitAddRequest request)
        {
            try
            {
                _suitBLL.InitTenantId(request.LoginTenantId);
                return await _suitBLL.SuitAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SuitEdit(SuitEditRequest request)
        {
            try
            {
                _suitBLL.InitTenantId(request.LoginTenantId);
                return await _suitBLL.SuitEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SuitGet(SuitGetRequest request)
        {
            try
            {
                _suitBLL.InitTenantId(request.LoginTenantId);
                return await _suitBLL.SuitGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SuitDel(SuitDelRequest request)
        {
            try
            {
                _suitBLL.InitTenantId(request.LoginTenantId);
                return await _suitBLL.SuitDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SuitChangeStatus(SuitChangeStatusRequest request)
        {
            try
            {
                _suitBLL.InitTenantId(request.LoginTenantId);
                return await _suitBLL.SuitChangeStatus(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SuitGetPaging(SuitGetPagingRequest request)
        {
            try
            {
                _suitBLL.InitTenantId(request.LoginTenantId);
                return await _suitBLL.SuitGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
