using ETMS.Entity.Common;
using ETMS.Entity.Dto.Activity.Request;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/activity/[action]")]
    [ApiController]
    [Authorize]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityBLL _activityBLL;

        public ActivityController(IActivityBLL activityBLL)
        {
            this._activityBLL = activityBLL;
        }

        public async Task<ResponseBase> ActivitySystemGetPaging(ActivitySystemGetPagingRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivitySystemGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainCreateInit(ActivityMainCreateInitRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainCreateInit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainSaveOfGroupPurchase(ActivityMainSaveOfGroupPurchaseRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainSaveOfGroupPurchase(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainSaveAndPublishOfGroupPurchase(ActivityMainSaveOfGroupPurchaseRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainSaveAndPublishOfGroupPurchase(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainSaveOfHaggle(ActivityMainSaveOrPublishOfHaggleRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainSaveOfHaggle(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainSaveAndPublishOfHaggle(ActivityMainSaveOrPublishOfHaggleRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainSaveAndPublishOfHaggle(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainEdit(ActivityMainEditRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainGetPaging(ActivityMainGetPagingRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainPublish(ActivityMainPublishRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainPublish(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainOff(ActivityMainOffRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainOff(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainSetOn(ActivityMainSetOnRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainSetOn(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainSetShowInParent(ActivityMainSetShowInParentRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainSetShowInParent(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainGetSimple(ActivityMainGetSimpleRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainGetSimple(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainDel(ActivityMainDelRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMainGetForEdit(ActivityMainGetForEditRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityMainGetForEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityRouteGetPaging(ActivityRouteGetPagingRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityRouteGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityRouteItemGet(ActivityRouteItemGetRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityRouteItemGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityHaggleLogGet(ActivityHaggleLogGetRequest request)
        {
            try
            {
                _activityBLL.InitTenantId(request.LoginTenantId);
                return await _activityBLL.ActivityHaggleLogGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
