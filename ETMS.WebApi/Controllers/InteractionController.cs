using ETMS.Entity.Common;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/interaction/[action]")]
    [ApiController]
    [Authorize]
    public class InteractionController : ControllerBase
    {
        private readonly IActiveHomeworkBLL _activeHomeworkBLL;

        public InteractionController(IActiveHomeworkBLL activeHomeworkBLL)
        {
            this._activeHomeworkBLL = activeHomeworkBLL;
        }

        public async Task<ResponseBase> ActiveHomeworkGetPaging(ActiveHomeworkGetPagingRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveHomeworkAdd(ActiveHomeworkAddRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveHomeworkGetBasc(ActiveHomeworkGetBascRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkGetBasc(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveHomeworkStudentGetAnswered(ActiveHomeworkGetAnsweredRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkStudentGetAnswered(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveHomeworkStudentGetUnanswered(ActiveHomeworkGetUnansweredRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkStudentGetUnanswered(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveHomeworkDel(ActiveHomeworkDelRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveHomeworkCommentAdd(ActiveHomeworkCommentAddRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkCommentAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveHomeworkCommentDel(ActiveHomeworkCommentDelRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkCommentDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
