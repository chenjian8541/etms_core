using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View.Persistence
{
    public class GetStatisticsSalesCourseByAmountView
    {
        public long CourseId { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
