using ETMS.Entity.Common;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.MicroWeb;
using ETMS.LOG;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


namespace ETMS.WebApi.Controllers
{
    [Route("api/interaction2/[action]")]
    [ApiController]
    [Authorize]
    public class Interaction2Controller : ControllerBase
    {
        private readonly IMicroWebBLL _microWebBLL;

        private readonly IMicroWebConfigBLL _microWebConfigBLL;

        private readonly IElectronicAlbumBLL _electronicAlbumBLL;

        public Interaction2Controller(IMicroWebBLL microWebBLL, IMicroWebConfigBLL microWebConfigBLL,
            IElectronicAlbumBLL electronicAlbumBLL)
        {
            this._microWebBLL = microWebBLL;
            this._microWebConfigBLL = microWebConfigBLL;
            this._electronicAlbumBLL = electronicAlbumBLL;
        }

        public async Task<ResponseBase> MicroWebBannerGet(RequestBase request)
        {
            try
            {
                _microWebConfigBLL.InitTenantId(request.LoginTenantId);
                return await _microWebConfigBLL.MicroWebBannerGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebBannerSave(MicroWebBannerSaveRequest request)
        {
            try
            {
                _microWebConfigBLL.InitTenantId(request.LoginTenantId);
                return await _microWebConfigBLL.MicroWebBannerSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebTenantAddressGet(RequestBase request)
        {
            try
            {
                _microWebConfigBLL.InitTenantId(request.LoginTenantId);
                return await _microWebConfigBLL.MicroWebTenantAddressGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebTenantAddressSave(MicroWebTenantAddressSaveRequest request)
        {
            try
            {
                _microWebConfigBLL.InitTenantId(request.LoginTenantId);
                return await _microWebConfigBLL.MicroWebTenantAddressSave(request);
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
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnGetList(MicroWebColumnGetListRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnGetList(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnChangeStatus(MicroWebColumnChangeStatusRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnChangeStatus(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnAdd(MicroWebColumnAddRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnEdit(MicroWebColumnEditRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnDel(MicroWebColumnDelRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnSinglePageArticleGet(MicroWebColumnSinglePageGetRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnSinglePageArticleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnSinglePageArticleSave(MicroWebColumnSinglePageSaveRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnSinglePageArticleSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnArticleGetPaging(MicroWebColumnArticleGetPagingRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnArticleGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnArticleGet(MicroWebColumnArticleGetRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnArticleGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnArticleAdd(MicroWebColumnArticleAddRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnArticleAdd(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnArticleEdit(MicroWebColumnArticleEditRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnArticleEdit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnArticleDel(MicroWebColumnArticleDelRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnArticleDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebColumnArticleChangeStatus(MicroWebColumnArticleChangeStatusRequest request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebColumnArticleChangeStatus(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> MicroWebHomeGet(RequestBase request)
        {
            try
            {
                _microWebBLL.InitTenantId(request.LoginTenantId);
                return await _microWebBLL.MicroWebHomeGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> SysElectronicAlbumGetPaging(SysElectronicAlbumGetPagingRequest request)
        {
            try
            {
                _electronicAlbumBLL.InitTenantId(request.LoginTenantId);
                return await _electronicAlbumBLL.SysElectronicAlbumGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ElectronicAlbumGetPaging(ElectronicAlbumGetPagingRequest request)
        {
            try
            {
                _electronicAlbumBLL.InitTenantId(request.LoginTenantId);
                return await _electronicAlbumBLL.ElectronicAlbumGetPaging(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ElectronicAlbumCreateInit(ElectronicAlbumCreateInitRequest request)
        {
            try
            {
                _electronicAlbumBLL.InitTenantId(request.LoginTenantId);
                return await _electronicAlbumBLL.ElectronicAlbumCreateInit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ElectronicAlbumPageInit(ElectronicAlbumPageInitRequest request)
        {
            try
            {
                _electronicAlbumBLL.InitTenantId(request.LoginTenantId);
                return await _electronicAlbumBLL.ElectronicAlbumPageInit(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ElectronicAlbumSave(ElectronicAlbumSaveRequest request)
        {
            try
            {
                _electronicAlbumBLL.InitTenantId(request.LoginTenantId);
                return await _electronicAlbumBLL.ElectronicAlbumSave(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ElectronicAlbumPublish(ElectronicAlbumPublishRequest request)
        {
            try
            {
                _electronicAlbumBLL.InitTenantId(request.LoginTenantId);
                return await _electronicAlbumBLL.ElectronicAlbumPublish(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }

        public async Task<ResponseBase> ElectronicAlbumDel(ElectronicAlbumDelRequest request)
        {
            try
            {
                _electronicAlbumBLL.InitTenantId(request.LoginTenantId);
                return await _electronicAlbumBLL.ElectronicAlbumDel(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
