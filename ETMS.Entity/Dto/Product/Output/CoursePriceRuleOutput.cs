using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class CoursePriceRuleOutput
    {
        public bool IsByClassTimes { get; set; }

        public List<CoursePriceRuleOutputItem> ByClassTimes { get; set; }

        public bool ByClassTimesIsCanModify { get; set; }

        public bool IsByMonth { get; set; }

        public List<CoursePriceRuleOutputItem> ByMonth { get; set; }

        public bool ByMonthIsCanModify { get; set; }

        public bool IsByDay { get; set; }

        public List<CoursePriceRuleOutputItem> ByDay { get; set; }

        public bool ByDayIsCanModify { get; set; }
    }

    public class CoursePriceRuleOutputItem
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Price { get; set; }

        public int Points { get; set; }

        public bool IsCanModify { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmCoursePriceRuleExpiredType"/>
        /// </summary>
        public byte? ExpiredType { get; set; }

        public int? ExpiredValue { get; set; }
    }
}
