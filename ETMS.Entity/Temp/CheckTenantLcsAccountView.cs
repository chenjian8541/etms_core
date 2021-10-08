using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Temp
{
    public class CheckTenantLcsAccountView
    {
        public CheckTenantLcsAccountView()
        {
        }
        public CheckTenantLcsAccountView(string msg)
        {
            this.ErrMsg = msg;
        }

        public SysTenant MyTenant { get; set; }

        public SysTenantLcsAccount MyLcsAccount { get; set; }

        public string ErrMsg { get; set; }
    }
}
