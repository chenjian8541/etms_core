using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Common
{
    public class AlienTenantRequestBase : AlienRequestBase
    {
        public int? TenantId { get; set; }

        public override string Validate()
        {
            if (TenantId == null || TenantId <= 0)
            {
                return "请选择校区";
            }
            return base.Validate();
        }
    }
}
