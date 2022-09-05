using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Dto.Educational2.Request;
using ETMS.Entity.Dto.Educational3.Request;
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
    [Route("api/educational3/[action]")]
    [ApiController]
    [Authorize]
    public class Educational3Controller : ControllerBase
    {
        private readonly IAchievementBLL _achievementBLL;

        public Educational3Controller(IAchievementBLL achievementBLL)
        {
            this._achievementBLL = achievementBLL;
        }

        public async Task<ResponseBase> AchievementGetPaging(AchievementGetPagingRequest request)
        {
            try
            {
                _achievementBLL.InitTenantId(request.LoginTenantId);
                return await _achievementBLL.AchievementGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AchievementDetailGetPaging(AchievementDetailGetPagingRequest request)
        {
            try
            {
                _achievementBLL.InitTenantId(request.LoginTenantId);
                return await _achievementBLL.AchievementDetailGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AchievementGet(AchievementGetRequest request)
        {
            try
            {
                _achievementBLL.InitTenantId(request.LoginTenantId);
                return await _achievementBLL.AchievementGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AchievementAdd(AchievementAddRequest request)
        {
            try
            {
                _achievementBLL.InitTenantId(request.LoginTenantId);
                return await _achievementBLL.AchievementAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AchievementDel(AchievementDelRequest request)
        {
            try
            {
                _achievementBLL.InitTenantId(request.LoginTenantId);
                return await _achievementBLL.AchievementDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AchievementEdit(AchievementEditRequest request)
        {
            try
            {
                _achievementBLL.InitTenantId(request.LoginTenantId);
                return await _achievementBLL.AchievementEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AchievementPush(AchievementPushRequest request)
        {
            try
            {
                _achievementBLL.InitTenantId(request.LoginTenantId);
                return await _achievementBLL.AchievementPush(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AchievementStudentIncreaseGet(AchievementStudentIncreaseGetRequest request)
        {
            try
            {
                _achievementBLL.InitTenantId(request.LoginTenantId);
                return await _achievementBLL.AchievementStudentIncreaseGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
