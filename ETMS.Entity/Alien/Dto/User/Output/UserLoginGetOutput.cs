using ETMS.Entity.Config.Router;
using ETMS.Entity.Dto.Common.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.User.Output
{
    public class UserLoginGetOutput
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        public string HeadName { get; set; }

        public string HeadCode { get; set; }

        public int TenantCount { get; set; }

        public byte? Gender { get; set; }

        public List<RouteConfig> RouteConfigs { get; set; }

        public List<SelectItem2> Tenants { get; set; }
    }
}
