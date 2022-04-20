using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlienTenantStatisticsMonthGetOutput
    {
        public decimal TotalIncomeThis { get; set; }

        public decimal TotalIncomeLast { get; set; }

        public CompareValue TotalIncomeCompareValue { get; set; }

        public decimal TotalExpensesThis { get; set; }

        public decimal TotalExpensesLast { get; set; }

        public CompareValue TotalExpensesCompareValue { get; set; }

        public decimal TotalClassDeSumThis { get; set; }

        public decimal TotalClassDeSumLast { get; set; }

        public CompareValue TotalClassDeSumCompareValue { get; set; }

        public int TotalBuyCourseCountThis { get; set; }

        public int TotalBuyCourseCountLast { get; set; }

        public CompareValue TotalBuyCourseCountCompareValue { get; set; }

        public List<AlienTenantStatisticsMonthItem> Items { get; set; }
    }

    public class AlienTenantStatisticsMonthItem
    {
        public string TenantName { get; set; }

        public string LinkMan { get; set; }

        public int StudentReadCount { get; set; }

        public int StudentPotentialCount { get; set; }

        public int StudentHistoryCount { get; set; }

        public int TeacherCount { get; set; }

        public decimal IncomeThis { get; set; }

        public decimal ExpensesThis { get; set; }

        public decimal ClassDeSumThis { get; set; }

        public string ClassDeTimesThisDesc { get; set; }

        public int BuyCourseCountThis { get; set; }

        public decimal BuyCourseSumThis { get; set; }
    }
}
