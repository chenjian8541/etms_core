using ETMS.Entity.Config;
using ETMS.Utility;

namespace ETMS.Alien.Webapi.Core
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// 员工信息
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static Tuple<int, long, string> GetTokenInfo(this HttpRequest @this)
        {
            var tokenValue = @this.HttpContext.User?.Claims?.FirstOrDefault(p => p.Type.Equals(SystemConfig.AuthenticationConfig.ClaimType))?.Value;
            var values = tokenValue.Split(',');
            return Tuple.Create(values[1].ToInt(), values[2].ToLong(), values[3]);
        }
    }
}
