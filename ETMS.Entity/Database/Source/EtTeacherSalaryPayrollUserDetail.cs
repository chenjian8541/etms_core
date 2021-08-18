using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSalaryPayrollUserDetail")]
    public class EtTeacherSalaryPayrollUserDetail : Entity<long>
    {
        public long TeacherSalaryPayrollId { get; set; }

        public long TeacherSalaryPayrollUserId { get; set; }

        public long UserId { get; set; }

        public long FundsItemsId { get; set; }

        public string FundsItemsName { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmTeacherSalaryFundsItemsType"/>
        /// </summary>
        public byte FundsItemsType { get; set; }

        /// <summary>
        /// <see cref="Enum.EmBool"/>
        /// </summary>
        public byte IsPerformance { get; set; }

        public decimal AmountSum { get; set; }

        public int OrderIndex { get; set; }
    }
}
