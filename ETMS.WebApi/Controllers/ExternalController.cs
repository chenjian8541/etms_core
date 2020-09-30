using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.External.Request;
using ETMS.Entity.Dto.Product.Request;
using ETMS.IBusiness;
using ETMS.LOG;
using ETMS.WebApi.Controllers.External;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers
{
    [Route("api/external/[action]")]
    [ApiController]
    [Authorize]
    public class ExternalController : ControllerBase
    {
        private readonly IImportBLL _importBLL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        public ExternalController(IImportBLL importBLL, IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._importBLL = importBLL;
            this._appConfigurtaionServices = appConfigurtaionServices;
        }

        public async Task<ResponseBase> ImportStudentTemplateGet(GetImportStudentExcelTemplateRequest request)
        {
            try
            {
                _importBLL.InitTenantId(request.LoginTenantId);
                return await _importBLL.GetImportStudentExcelTemplate(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return null;
            }
        }

        public async Task<ResponseBase> ImportStudent([FromForm]IFormCollection collection)
        {
            try
            {
                var action = new ImportStudentAction();
                return await action.ProcessAction(collection, _appConfigurtaionServices.AppSettings);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex, this.GetType());
                return new ResponseBase().GetResponseCodeError();
            }
        }
    }
}
