using ETMS.Entity.Common;
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
    [Route("api/interaction/[action]")]
    [ApiController]
    [Authorize]
    public class InteractionController : ControllerBase
    {
        private readonly IActiveHomeworkBLL _activeHomeworkBLL;

        private readonly IActiveGrowthRecordBLL _activeGrowthRecordBLL;

        private readonly IActiveWxMessageBLL _activeWxMessageBLL;

        private readonly ISmsLogBLL _smsLogBLL;

        private readonly IClassRecordEvaluateBLL _classRecordEvaluateBLL;

        public InteractionController(IActiveHomeworkBLL activeHomeworkBLL, IActiveGrowthRecordBLL activeGrowthRecordBLL, IActiveWxMessageBLL activeWxMessageBLL,
            ISmsLogBLL smsLogBLL, IClassRecordEvaluateBLL classRecordEvaluateBLL)
        {
            this._activeHomeworkBLL = activeHomeworkBLL;
            this._activeGrowthRecordBLL = activeGrowthRecordBLL;
            this._activeWxMessageBLL = activeWxMessageBLL;
            this._smsLogBLL = smsLogBLL;
            this._classRecordEvaluateBLL = classRecordEvaluateBLL;
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

        public async Task<ResponseBase> ActiveHomeworkAdd2(ActiveHomeworkAdd2Request request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkAdd2(request);
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

        public async Task<ResponseBase> ActiveHomeworkStudentGetPaging(ActiveHomeworkStudentGetPagingRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkStudentGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveHomeworkStudentGet(ActiveHomeworkStudentGetRequest request)
        {
            try
            {
                _activeHomeworkBLL.InitTenantId(request.LoginTenantId);
                return await _activeHomeworkBLL.ActiveHomeworkStudentGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthRecordClassGetPaging(ActiveGrowthRecordClassGetPagingRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthRecordClassGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthRecordStudentGetPaging(ActiveGrowthRecordStudentGetPagingRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthRecordStudentGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthRecordStudentStatusGet(ActiveGrowthRecordStudentStatusGetRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthRecordStudentStatusGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthRecordClassAdd(ActiveGrowthRecordClassAddRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthRecordClassAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthRecordStudentAdd(ActiveGrowthRecordStudentAddRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthRecordStudentAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthRecordGet(ActiveGrowthRecordGetRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthRecordGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthRecordDel(ActiveGrowthRecordDelRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthRecordDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthCommentAdd(ActiveGrowthCommentAddRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthCommentAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthCommentDel(ActiveGrowthCommentDelRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthCommentDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveGrowthStudentGetPaging(ActiveGrowthStudentGetPagingRequest request)
        {
            try
            {
                _activeGrowthRecordBLL.InitTenantId(request.LoginTenantId);
                return await _activeGrowthRecordBLL.ActiveGrowthStudentGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveWxMessageAdd(ActiveWxMessageAddRequest request)
        {
            try
            {
                _activeWxMessageBLL.InitTenantId(request.LoginTenantId);
                return await _activeWxMessageBLL.ActiveWxMessageAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveWxMessageEdit(ActiveWxMessageEditRequest request)
        {
            try
            {
                _activeWxMessageBLL.InitTenantId(request.LoginTenantId);
                return await _activeWxMessageBLL.ActiveWxMessageEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveWxMessageDel(ActiveWxMessageDelRequest request)
        {
            try
            {
                _activeWxMessageBLL.InitTenantId(request.LoginTenantId);
                return await _activeWxMessageBLL.ActiveWxMessageDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveWxMessageGet(ActiveWxMessageGetRequest request)
        {
            try
            {
                _activeWxMessageBLL.InitTenantId(request.LoginTenantId);
                return await _activeWxMessageBLL.ActiveWxMessageGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveWxMessageGetPaging(ActiveWxMessageGetPagingRequest request)
        {
            try
            {
                _activeWxMessageBLL.InitTenantId(request.LoginTenantId);
                return await _activeWxMessageBLL.ActiveWxMessageGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActiveWxMessageDetailGetPaging(ActiveWxMessageDetailGetPagingRequest request)
        {
            try
            {
                _activeWxMessageBLL.InitTenantId(request.LoginTenantId);
                return await _activeWxMessageBLL.ActiveWxMessageDetailGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentSmsLogGetPaging(StudentSmsLogGetPagingRequest request)
        {
            try
            {
                _smsLogBLL.InitTenantId(request.LoginTenantId);
                return await _smsLogBLL.StudentSmsLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentSmsSend(StudentSmsSendRequest request)
        {
            try
            {
                _smsLogBLL.InitTenantId(request.LoginTenantId);
                return await _smsLogBLL.StudentSmsBatchSend(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateGetPaging(TeacherClassRecordEvaluateGetPagingRequest request)
        {
            try
            {
                _classRecordEvaluateBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordEvaluateBLL.TeacherClassRecordEvaluateGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateGetDetail(TeacherClassRecordEvaluateGetDetailRequest request)
        {
            try
            {
                _classRecordEvaluateBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordEvaluateBLL.TeacherClassRecordEvaluateGetDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateStudent(TeacherClassRecordEvaluateStudentRequest request)
        {
            try
            {
                _classRecordEvaluateBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordEvaluateBLL.TeacherClassRecordEvaluateStudent(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateStudentDetail(TeacherClassRecordEvaluateStudentDetailRequest request)
        {
            try
            {
                _classRecordEvaluateBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordEvaluateBLL.TeacherClassRecordEvaluateStudentDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateSubmit(TeacherClassRecordEvaluateSubmitRequest request)
        {
            try
            {
                _classRecordEvaluateBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordEvaluateBLL.TeacherClassRecordEvaluateSubmit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherEvaluateLogGetPaging(TeacherEvaluateLogGetPagingRequest request)
        {
            try
            {
                _classRecordEvaluateBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordEvaluateBLL.TeacherEvaluateLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentEvaluateLogGetPaging(StudentEvaluateLogGetPagingRequest request)
        {
            try
            {
                _classRecordEvaluateBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordEvaluateBLL.StudentEvaluateLogGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TeacherClassRecordEvaluateDel(TeacherClassRecordEvaluateDelRequest request)
        {
            try
            {
                _classRecordEvaluateBLL.InitTenantId(request.LoginTenantId);
                return await _classRecordEvaluateBLL.TeacherClassRecordEvaluateDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
