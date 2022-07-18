using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Parent2.Request;
using ETMS.Entity.Dto.Parent3.Request;
using ETMS.Entity.Dto.Parent4.Request;
using ETMS.Entity.Dto.Parent5.Request;
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
    [EtmsSignatureParentAuthorize]
    public class Parent3Controller : ControllerBase
    {
        private readonly IParentData4BLL _parentData4BLL;

        private readonly IParentData5BLL _parentData5BLL;
        public Parent3Controller(IParentData4BLL parentData4BLL, IParentData5BLL parentData5BLL)
        {
            this._parentData4BLL = parentData4BLL;
            this._parentData5BLL = parentData5BLL;
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

        public async Task<ResponseBase> TeacherEvaluateGetPaging(TeacherEvaluateGetPagingRequest request)
        {
            try
            {
                _parentData4BLL.InitTenantId(request.LoginTenantId);
                return await _parentData4BLL.TeacherEvaluateGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlbumGetPaging(AlbumGetPagingRequest request)
        {
            try
            {
                _parentData4BLL.InitTenantId(request.LoginTenantId);
                return await _parentData4BLL.AlbumGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservation1v1Check(StudentReservation1v1CheckRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.StudentReservation1v1Check(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservation1v1Init(StudentReservation1v1InitRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.StudentReservation1v1Init(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservation1v1DateCheck(StudentReservation1v1DateCheckRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.StudentReservation1v1DateCheck(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservation1v1LessonsGet(StudentReservation1v1LessonsGetRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.StudentReservation1v1LessonsGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentReservation1v1LessonsSubmit(StudentReservation1v1LessonsSubmitRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.StudentReservation1v1LessonsSubmit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentTryClassGetPaging(StudentTryClassGetPagingRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.StudentTryClassGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentTryClassGet(StudentTryClassGetRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.StudentTryClassGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentTryClassSubmit(StudentTryClassSubmitRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.StudentTryClassSubmit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> StudentTryClassCancel(StudentTryClassCancelRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.StudentTryClassCancel(request);
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
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.ActivityMainGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ActivityMyGetPaging(ActivityMyGetPagingRequest request)
        {
            try
            {
                _parentData5BLL.InitTenantId(request.LoginTenantId);
                return await _parentData5BLL.ActivityMyGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
