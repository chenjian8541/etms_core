using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETMS.Business.Common;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Config;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Dto.User.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ETMS.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
        }

        [HttpGet]
        public List<MenuConfig> Get()
        {
            var menus = PermissionData.MenuConfigs;
            return menus;
            //return Utility.CryptogramHelper.Encrypt3DES("88888888", "etms88888888");
        }
    }
}
