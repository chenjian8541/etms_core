using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlTenantStatisticsSalesTenantGetOutput
    {
        public AlTenantStatisticsSalesTenantValue ThisWeek { get; set; }

        public AlTenantStatisticsSalesTenantValue ThisMonth { get; set; }

        public AlTenantStatisticsSalesTenantValue LastWeek { get; set; }

        public AlTenantStatisticsSalesTenantValue LastMonth { get; set; }
    }

    public class AlTenantStatisticsSalesTenantValue
    {
        public string DateDesc { get; set; }

        public int OrderNewCount { get; set; }

        public int OrderRenewCount { get; set; }

        public int OrderBuyCount { get; set; }

        public decimal OrderNewSum { get; set; }

        public decimal OrderRenewSum { get; set; }

        public decimal OrderTransferOutSum { get; set; }

        public decimal OrderReturnSum { get; set; }

        public decimal OrderSum { get; set; }
    }
}
