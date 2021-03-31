using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class StatisticsSalesTenantView
    {
        public int TotalOrderNewCount { get; set; }

        public int TotalOrderRenewCount { get; set; }

        public int TotalOrderBuyCount { get; set; }

        public decimal TotalOrderNewSum { get; set; }

        public decimal TotalOrderRenewSum { get; set; }

        public decimal TotalOrderTransferOutSum { get; set; }

        public decimal TotalOrderReturnSum { get; set; }

        public decimal TotalOrderSum { get; set; }
    }
}
