using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class GetStatisticsSalesProductMonthPagingOutput
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
