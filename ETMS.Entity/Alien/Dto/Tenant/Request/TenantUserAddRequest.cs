using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.Tenant.Request
{
    public class TenantUserAddRequest : AlienRequestBase
    {
        public long UserId { get; set; }

        public int TenantId { get; set; }

        public long RoleId { get; set; }

        public override string Validate()
        {
            if (UserId <= 0)
            {
                return "请选择员工";
            }
            if (TenantId <= 0)
            {
                return "请选择校区";
            }
            if (RoleId <= 0)
            {
                return "请选择角色";
            }
            return base.Validate();
        }
    }
}
