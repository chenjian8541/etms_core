using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStatisticsSalesTenant")]
    public class EtStatisticsSalesTenant : Entity<long>
    {
        public int OrderNewCount { get; set; }

        public int OrderRenewCount { get; set; }

        public int OrderBuyCount { get; set; }

        public decimal OrderNewSum { get; set; }

        public decimal OrderRenewSum { get; set; }

        public decimal OrderTransferOutSum { get; set; }

        public decimal OrderReturnSum { get; set; }

        public decimal OrderSum { get; set; }

        public DateTime Ot { get; set; }
    }
}
