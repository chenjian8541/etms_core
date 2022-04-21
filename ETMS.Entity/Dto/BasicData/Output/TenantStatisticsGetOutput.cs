using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class TenantStatisticsGetOutput
    {
        public TenantStatisticsGetBascInfo BascInfo { get; set; }

        public TenantStatisticsGetWeek WeekInfo { get; set; }

        public TenantStatisticsGetMonth MonthInfo { get; set; }
    }

    public class TenantStatisticsGetBascInfo
    {
        public int StudentReadCount { get; set; }

        public int StudentPotentialCount { get; set; }

        public int StudentHistoryCount { get; set; }

        public int TeacherCount { get; set; }

        public string TenantSurplusClassTimesDesc { get; set; }

        public decimal TenantSurplusSurplusMoney { get; set; }

    }

    public class TenantStatisticsGetWeek
    {
        public decimal IncomeThis { get; set; }

        public decimal IncomeLast { get; set; }

        public decimal ExpensesThis { get; set; }

        public decimal ExpensesLast { get; set; }

        public decimal ClassDeSumThis { get; set; }

        public decimal ClassDeSumLast { get; set; }

        public decimal ClassDeTimesThis { get; set; }

        public decimal ClassDeTimesLast { get; set; }

        public int BuyCourseCountThis { get; set; }

        public int BuyCourseCountLast { get; set; }

        public decimal BuyCourseSumThis { get; set; }

        public decimal BuyCourseSumLast { get; set; }

        public CompareValue TotalIncomeCompareValue { get; set; }
        public CompareValue TotalExpensesCompareValue { get; set; }
        public CompareValue TotalClassDeSumCompareValue { get; set; }
        public CompareValue TotalBuyCourseCountCompareValue { get; set; }
    }

    public class TenantStatisticsGetMonth
    {
        public decimal IncomeThis { get; set; }

        public decimal IncomeLast { get; set; }

        public decimal ExpensesThis { get; set; }

        public decimal ExpensesLast { get; set; }

        public decimal ClassDeSumThis { get; set; }

        public decimal ClassDeSumLast { get; set; }

        public decimal ClassDeTimesThis { get; set; }

        public decimal ClassDeTimesLast { get; set; }

        public int BuyCourseCountThis { get; set; }

        public int BuyCourseCountLast { get; set; }

        public decimal BuyCourseSumThis { get; set; }

        public decimal BuyCourseSumLast { get; set; }

        public CompareValue TotalIncomeCompareValue { get; set; }
        public CompareValue TotalExpensesCompareValue { get; set; }
        public CompareValue TotalClassDeSumCompareValue { get; set; }
        public CompareValue TotalBuyCourseCountCompareValue { get; set; }
    }
}
