using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.Wechart;
using ETMS.LOG;
using ETMS.Utility;
using ETMS.WebApi.Controllers.Open;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Open.Containers;
using Senparc.Weixin.Open.Entities.Request;
using System;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/open2/[action]")]
    [ApiController]
    public class Open2Controller : ControllerBase
    {
        private readonly IOpenBLL _openBLL;

        private readonly IOpen2BLL _open2BLL;

        public Open2Controller(IOpenBLL openBLL, IOpen2BLL open2BLL)
        {
            this._openBLL = openBLL;
            this._open2BLL = open2BLL;
        }

        public async Task<ResponseBase> TenantInfoGet(Open2Base request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.TenantInfoGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebHomeGet(Open2Base request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebHomeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnSingleArticleGet(MicroWebColumnSingleArticleGetRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebColumnSingleArticleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebArticleGet(MicroWebArticleGetRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebArticleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebArticleGetPaging(MicroWebArticleGetPagingRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebArticleGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetJsSdkUiPackage(GetJsSdkUiPackageRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.GetJsSdkUiPackage(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetAuthorizeUrl(GetAuthorizeUrlRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.GetAuthorizeUrl(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> GetWxOpenId(GetWxOpenIdRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.GetWxOpenId(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryClassApply(TryCalssApplyRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.TryCalssApply(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryClassApplySupplement(TryCalssApplySupplementRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.TryCalssApplySupplement(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnGet(MicroWebColumnGetRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebColumnGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebArticleReading(MicroWebArticleReadingRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MicroWebArticleReading(request);
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
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MallGoodsGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MallGoodsDetailGet(MallGoodsDetailGetRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MallGoodsDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MallCartAdd(MallCartAddRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MallCartAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MallCartInfoGet(MallCartInfoGetRequest request)
        {
            try
            {
                _openBLL.InitTenantId(request.LoginTenantId);
                return await _openBLL.MallCartInfoGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TenantSimpleInfoGetMI(TenantSimpleInfoGetMIRequest request)
        {
            try
            {
                return await _openBLL.TenantSimpleInfoGetMI(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ClassRecordDetailGet(ClassRecordDetailGetOpenRequest request)
        {
            try
            {
                _open2BLL.InitTenantId(request.LoginTenantId);
                return await _open2BLL.ClassRecordDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CheckPhoneSmsSend(CheckPhoneSmsSendRequest request)
        {
            try
            {
                return await _open2BLL.CheckPhoneSmsSend(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> TryApplyLogAdd(TryApplyLogAddRequest request)
        {
            try
            {
                return await _open2BLL.TryApplyLogAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> EvaluateStudentDetail(EvaluateStudentDetailRequest request)
        {
            try
            {
                _open2BLL.InitTenantId(request.LoginTenantId);
                return await _open2BLL.EvaluateStudentDetail(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ShareContentGet(ShareContentGetRequest request)
        {
            try
            {
                _open2BLL.InitTenantId(request.LoginTenantId);
                return await _open2BLL.ShareContentGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlbumDetailGet(AlbumDetailGetRequest request)
        {
            try
            {
                _open2BLL.InitTenantId(request.LoginTenantId);
                return await _open2BLL.AlbumDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase AlbumShare(AlbumShareRequest request)
        {
            try
            {
                _open2BLL.InitTenantId(request.LoginTenantId);
                return _open2BLL.AlbumShare(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> AlbumInfoGet(AlbumInfoGetRequest request)
        {
            try
            {
                _open2BLL.InitTenantId(request.LoginTenantId);
                return await _open2BLL.AlbumInfoGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public ResponseBase PhoneVerificationCodeGet(PhoneVerificationCodeGetRequest request)
        {
            try
            {
                return _open2BLL.PhoneVerificationCodeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CheckPhoneSmsSafe(CheckPhoneSmsSafeRequest request)
        {
            try
            {
                return await _open2BLL.CheckPhoneSmsSafe(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SendSmsCodeAboutRegister(SendSmsCodeAboutRegisterRequest request)
        {
            try
            {
                return await _open2BLL.SendSmsCodeAboutRegister(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> CheckTenantAccount(CheckTenantAccountRequest request)
        {
            try
            {
                return await _open2BLL.CheckTenantAccount(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ChangeTenantUserPwd(ChangeTenantUserPwdRequest request)
        {
            try
            {
                return await _open2BLL.ChangeTenantUserPwd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    
    }
}
