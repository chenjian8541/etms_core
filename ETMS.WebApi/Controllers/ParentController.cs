﻿using ETMS.Entity.Common;
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

        public ParentController(IParentBLL parentBLL, IGiftBLL giftBLL, ICouponsBLL couponsBLL, IStudentBLL studentBLL,
            IParentDataBLL parentDataBLL, IParentData2BLL parentData2BLL)
        {
            this._parentBLL = parentBLL;
            this._giftBLL = giftBLL;
            this._couponsBLL = couponsBLL;
            this._studentBLL = studentBLL;
            this._parentDataBLL = parentDataBLL;
            this._parentData2BLL = parentData2BLL;
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
    }
}
