using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Student.Request;
using ETMS.IBusiness;
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
    [Route("api/parent/[action]")]
    [ApiController]
    [EtmsSignatureAuthorize]
    public class ParentController : ControllerBase
    {
        private readonly IParentBLL _parentBLL;

        private readonly IGiftBLL _giftBLL;

        private readonly ICouponsBLL _couponsBLL;

        private readonly IStudentBLL _studentBLL;

        private readonly IParentDataBLL _parentDataBLL;

        private readonly IParentData2BLL _parentData2BLL;

        private readonly IParentData3BLL _parentData3BLL;

        public ParentController(IParentBLL parentBLL, IGiftBLL giftBLL, ICouponsBLL couponsBLL, IStudentBLL studentBLL,
            IParentDataBLL parentDataBLL, IParentData2BLL parentData2BLL, IParentData3BLL parentData3BLL)
        {
            this._parentBLL = parentBLL;
            this._giftBLL = giftBLL;
            this._couponsBLL = couponsBLL;
            this._studentBLL = studentBLL;
            this._parentDataBLL = parentDataBLL;
            this._parentData2BLL = parentData2BLL;
            this._parentData3BLL = parentData3BLL;
        }

        [AllowAnonymous]
        [ActionName("parentLoginSendSms")]
        [HttpPost]
        public async Task<ResponseBase> ParentLoginSendSms(ParentLoginSendSmsRequest request)
        {
            try
            {
                return await _parentBLL.ParentLoginSendSms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        [ActionName("parentLoginBySms")]
        [HttpPost]
        public async Task<ResponseBase> ParentLoginBySms(ParentLoginBySmsRequest request)
        {
            try
            {
                return await _parentBLL.ParentLoginBySms(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }


        [AllowAnonymous]
        [ActionName("parentRefreshToken")]
        public ResponseBase ParentRefreshToken(ParentRefreshTokenRequest request)
        {
            try
            {
                return _parentBLL.ParentRefreshToken(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> ParentGetAuthorizeUrl(ParentGetAuthorizeUrlRequest request)
        {
            try
            {
                return await _parentBLL.ParentGetAuthorizeUrl(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> ParentLoginByCode(ParentLoginByCodeRequest request)
        {
            try
            {
                return await _parentBLL.ParentLoginByCode(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentGetAuthorizeUrl2(ParentGetAuthorizeUrl2Request request)
        {
            try
            {
                return await _parentBLL.ParentGetAuthorizeUrl2(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentBindingWeChat(ParentBindingWeChatRequest request)
        {
            try
            {
                return await _parentBLL.ParentBindingWeChat(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentInfoGet(ParentInfoGetRequest request)
        {
            try
            {
                return await _parentBLL.ParentInfoGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentLoginout(ParentLoginoutRequest request)
        {
            try
            {
                return await _parentBLL.ParentLoginout(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> GetTenantInfoByNo(GetTenantInfoByNoRequest request)
        {
            try
            {
                return await _parentBLL.GetTenantInfoByNo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetTenantInfo(GetTenantInfoRequest request)
        {
            try
            {
                return await _parentBLL.GetTenantInfo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentGetCurrentTenant(ParentRequestBase request)
        {
            try
            {
                return await _parentBLL.ParentGetCurrentTenant(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentGetTenants(ParentRequestBase request)
        {
            try
            {
                return await _parentBLL.ParentGetTenants(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ParentTenantEntrance(ParentTenantEntranceRequest request)
        {
            try
            {
                return await _parentBLL.ParentTenantEntrance(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentListGet(StudentListGetRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.StudentListGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentListDetailGet(StudentListDetailGetRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.StudentListDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> ParentCouponsReceive(ParentCouponsReceiveRequest request)
        {
            try
            {
                _couponsBLL.InitTenantId(request.LoginTenantId);
                return await _couponsBLL.ParentCouponsReceive(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [HttpPost]
        public async Task<ResponseBase> StudentLeaveApplyAdd(StudentLeaveApplyAddRequest request)
        {
            try
            {
                _studentBLL.InitTenantId(request.LoginTenantId);
                return await _studentBLL.StudentLeaveApplyAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentLeaveApplyGet(StudentLeaveApplyGetRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.StudentLeaveApplyGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentLeaveApplyDetailGet(StudentLeaveApplyDetailGetRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.StudentLeaveApplyDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentLeaveApplyRevoke(StudentLeaveApplyRevokeRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.StudentLeaveApplyRevoke(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentClassTimetableGet(StudentClassTimetableRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.StudentClassTimetableGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentClassTimetableDetailGet(StudentClassTimetableDetailGetRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.StudentClassTimetableDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> IndexBannerGet(IndexBannerGetRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.IndexBannerGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GiftCategoryGet(GiftCategoryGetParentRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftCategoryGetParent(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GiftGet(GiftGetParentRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftGetParent(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GiftDetailGet(GiftDetailGetParentRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftDetailGetParent(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GiftExchange(GiftExchangeRequest request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GiftExchangeSelfHelp(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordGet(ClassRecordGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.ClassRecordGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordDetailGet(ClassRecordDetailGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.ClassRecordDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCourseGet(StudentCourseGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentCourseGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentOrderGet(StudentOrderGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentOrderGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentOrderDetailGet(StudentOrderDetailGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentOrderDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentPointsLogGet(StudentPointsLogGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentPointsLogGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentGiftExchangeLogGet(StudentGiftExchangeLogGetReqeust request)
        {
            try
            {
                _giftBLL.InitTenantId(request.LoginTenantId);
                return await _giftBLL.GetExchangeLogDetailParentPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCouponsNormalGet(StudentCouponsNormalGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentCouponsNormalGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCouponsUsedGet(StudentCouponsUsedGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentCouponsUsedGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentCouponsExpiredGet(StudentCouponsExpiredGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentCouponsExpiredGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentDetailInfo(StudentDetailInfoRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentDetailInfo(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> EvaluateTeacherGet(EvaluateTeacherGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.EvaluateTeacherGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> EvaluateTeacherGetDetail(EvaluateTeacherGetDetailRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.EvaluateTeacherGetDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> EvaluateTeacherSubmit(EvaluateTeacherSubmitRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.EvaluateTeacherSubmit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HomeworkUnansweredGetPaging(HomeworkUnansweredGetPagingRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.HomeworkUnansweredGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HomeworkAnsweredGetPaging(HomeworkAnsweredGetPagingRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.HomeworkAnsweredGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HomeworkDetailGet(HomeworkDetailGetRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.HomeworkDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HomeworkDetailSetRead(HomeworkDetailSetReadRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.HomeworkDetailSetRead(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HomeworkSubmitAnswer(HomeworkSubmitAnswerRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.HomeworkSubmitAnswer(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HomeworkAddComment(HomeworkAddCommentRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.HomeworkAddComment(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> HomeworkDelComment(HomeworkDelCommentRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.HomeworkDelComment(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GrowthRecordGetPaging(GrowthRecordGetPagingRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.GrowthRecordGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GrowthRecordFavoriteGetPaging(GrowthRecordGetPagingRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.GrowthRecordFavoriteGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> GrowthRecordDetailGet(GrowthRecordDetailGetRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.TenantId);
                return await _parentDataBLL.GrowthRecordDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GrowthRecordChangeFavorite(GrowthRecordChangeFavoriteRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.GrowthRecordChangeFavorite(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GrowthRecordAddComment(GrowthRecordAddCommentRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.GrowthRecordAddComment(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GrowthRecordDelComment(GrowthRecordDelCommentRequest request)
        {
            try
            {
                _parentDataBLL.InitTenantId(request.LoginTenantId);
                return await _parentDataBLL.GrowthRecordDelComment(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMessageDetailPaging(WxMessageDetailPagingRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.WxMessageDetailPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMessageDetailGet(WxMessageDetailGetRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.WxMessageDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMessageDetailSetRead(WxMessageDetailSetReadRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.WxMessageDetailSetRead(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMessageDetailSetConfirm(WxMessageDetailSetConfirmRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.WxMessageDetailSetConfirm(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> WxMessageGetUnreadCount(WxMessageGetUnreadCountRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.WxMessageGetUnreadCount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        [AllowAnonymous]
        public async Task<ResponseBase> TryCalssApply(TryCalssApplyRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.TenantId);
                return await _parentData3BLL.TryCalssApply(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CheckOnLogGet(CheckOnLogGetRequest request)
        {
            try
            {
                _parentData3BLL.InitTenantId(request.LoginTenantId);
                return await _parentData3BLL.CheckOnLogGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentOrderTransferCoursesGetDetail(StudentOrderTransferCoursesGetDetailRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentOrderTransferCoursesGetDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentOrderReturnLogGet(StudentOrderReturnLogGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentOrderReturnLogGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentOrderTransferCoursesLogGet(StudentOrderTransferCoursesLogGetRequest request)
        {
            try
            {
                _parentData2BLL.InitTenantId(request.LoginTenantId);
                return await _parentData2BLL.StudentOrderTransferCoursesLogGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
