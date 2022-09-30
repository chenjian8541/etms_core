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

        public TenantAgtPayAccountInfo MyAgtPayAccountInfo { get; set; }

        public SysTenantLcsAccount MyLcsAccount { get; set; }

        public SysTenantFubeiAccount MyFubeiAccount { get; set; }

        public BaseTenantSuixingAccount MySysTenantSuixingAccount { get; set; }

        public string ErrMsg { get; set; }
    }

    public class TenantAgtPayAccountInfo
    {
        public string MerchantName { get; set; }

        public string MerchantNo { get; set; }

        public int MerchantType { get; set; }

        public string TerminalId { get; set; }
    }
}
