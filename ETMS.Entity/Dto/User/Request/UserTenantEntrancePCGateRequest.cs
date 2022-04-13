using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Request
{
    public class UserTenantEntrancePCGateRequest : IValidate
    {
        public string LoginNo { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(LoginNo))
            {
                return "登录信息不完整";
            }
            return string.Empty;
        }
    }
}
