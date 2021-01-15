using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class OrderTransferCoursesLogGetOutput
    {
        public string OtDesc { get; set; }

        public long UnionOrderId { get; set; }

        public string UnionOrderNo { get; set; }

        public string ProductName { get; set; }

        public string OutQuantity { get; set; }

        public string OutQuantityDesc { get; set; }

        public decimal ItemAptSum { get; set; }
    }
}
