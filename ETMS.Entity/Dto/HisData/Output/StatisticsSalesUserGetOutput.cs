using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class StatisticsSalesUserGetOutput
    {
        public int OrderNewCount { get; set; }

        public int OrderRenewCount { get; set; }

        public int OrderBuyCount { get; set; }

        public decimal OrderNewSum { get; set; }

        public decimal OrderRenewSum { get; set; }

        public decimal OrderTransferOutSum { get; set; }

        public decimal OrderReturnSum { get; set; }

        public decimal OrderSum { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }
    }
}
