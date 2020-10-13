using Microsoft.AspNetCore.Mvc;

namespace ETMS.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public HomeController()
        {
        }

        [HttpGet]
        public string Index()
        {
            return "hello etms";
        }
    }
}
