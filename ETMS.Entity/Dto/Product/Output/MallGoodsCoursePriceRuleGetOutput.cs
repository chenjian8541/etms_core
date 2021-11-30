using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Product.Output
{
    public class MallGoodsCoursePriceRuleGetOutput
    {
        public bool IsByClassTimes { get; set; }

        public List<MallGoodsCoursePriceRuleItem> ByClassTimes { get; set; }

        public bool IsByMonth { get; set; }

        public List<MallGoodsCoursePriceRuleItem> ByMonth { get; set; }

        public bool IsByDay { get; set; }

        public List<MallGoodsCoursePriceRuleItem> ByDay { get; set; }

        public bool ByClassTimesIsCanModify { get; set; }

        public bool ByMonthIsCanModify { get; set; }

        public bool ByDayIsCanModify { get; set; }

        public decimal ByClassTimesTotalM { get; set; }

        public decimal ByMonthTotalM { get; set; }

        public decimal ByDayTotalM { get; set; }
    }

    public class MallGoodsCoursePriceRuleItem
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
