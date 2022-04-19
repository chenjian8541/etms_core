using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantStatisticsMonth")]
    public class SysTenantStatisticsMonth : EManageEntity<long>
    {
        public int TenantId { get; set; }

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
    }
}
