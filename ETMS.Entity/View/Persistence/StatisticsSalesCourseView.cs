using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View.Persistence
{
    public class StatisticsSalesCourseView
    {
        public long CourseId { get; set; }

        public int TotalCount { get; set; }

        public decimal TotalAmount { get; set; }

        public byte BugUnit { get; set; }
    }
}
