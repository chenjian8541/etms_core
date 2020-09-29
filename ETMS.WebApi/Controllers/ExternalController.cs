using ETMS.Entity.Common;
using ETMS.Entity.Dto.External.Request;
using ETMS.Entity.Dto.Product.Request;
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
    [Route("api/external/[action]")]
    [ApiController]
    [Authorize]
    public class ExternalController : ControllerBase
    {
        private readonly IImportBLL _importBLL;

        public ExternalController(IImportBLL importBLL)
        {
            this._importBLL = importBLL;
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
    }
}
