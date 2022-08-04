using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.Entity.Dto.Open3.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.Open;
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
    [Route("api/open3/[action]")]
    [ApiController]
    public class Open3Controller : ControllerBase
    {
        private readonly IOpen3BLL _open3BLL;

        public Open3Controller(IOpen3BLL _open3BLL)
        {
            this._open3BLL = _open3BLL;
        }

        public async Task<ResponseBase> AchievementDetailGet(AchievementDetailGetRequest request)
        {
            try
            {
                _open3BLL.InitTenantId(request.LoginTenantId);
                return await _open3BLL.AchievementDetailGet(request);
            }
            catch (Exception ex)
            {
                Log.Error(request, ex, this.GetType());
                return ResponseBase.UnKnownError();
            }
        }
    }
}
