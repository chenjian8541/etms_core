using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class StatisticsSalesTenantGetOutput
    {
        public StatisticsSalesTenantGetOutput()
        {
            //ThisWeek = new StatisticsSalesTenantValue();
            //ThisMonth = new StatisticsSalesTenantValue();
            //LastWeek = new StatisticsSalesTenantValue();
            //LastMonth = new StatisticsSalesTenantValue();
        }
        public StatisticsSalesTenantValue ThisWeek { get; set; }

        public StatisticsSalesTenantValue ThisMonth { get; set; }

        public StatisticsSalesTenantValue LastWeek { get; set; }

        public StatisticsSalesTenantValue LastMonth { get; set; }
    }

    public class StatisticsSalesTenantValue
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
