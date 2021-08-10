using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSalaryContractFixed")]
    public class EtTeacherSalaryContractFixed : Entity<long>
    {
        public long TeacherId { get; set; }

        public long FundsItemsId { get; set; }

        public decimal AmountValue { get; set; }
    }
}
