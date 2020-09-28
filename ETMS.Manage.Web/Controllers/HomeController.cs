using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ETMS.Manage.Web.Models;
using ETMS.Utility;
using ETMS.IBusiness.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
using ETMS.Entity.Common;

namespace ETMS.Manage.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ISysTenantManage _sysTenantManage;

        public HomeController(ILogger<HomeController> logger, ISysTenantManage sysTenantManage)
        {
            _logger = logger;
            this._sysTenantManage = sysTenantManage;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Etms()
        {
            return View();
        }

        public async Task<IActionResult> CreateTenant(TenantAddRequest request)
        {
            var msg = request.Check();
            if (!string.IsNullOrEmpty(msg))
            {
                return Content(msg);
            }
            var res = await _sysTenantManage.TenantAdd(request);
            if (res.IsResponseSuccess())
            {
                return Content("操作成功");
            }
            return Content(res.message);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
