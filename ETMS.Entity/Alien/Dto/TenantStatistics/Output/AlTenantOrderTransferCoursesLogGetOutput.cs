using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlTenantOrderTransferCoursesLogGetOutput
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
