using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlTenantStatisticsSalesProductMonthPagingGetOutput
    {
        public long Id { get; set; }

        public decimal SalesTotalSum { get; set; }

        public decimal CourseSum { get; set; }

        public decimal GoodsSum { get; set; }

        public decimal CostSum { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }
    }
}
