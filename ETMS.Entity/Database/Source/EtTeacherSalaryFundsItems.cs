using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSalaryFundsItems")]
    public class EtTeacherSalaryFundsItems : Entity<long>
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryFundsItemsType"/>
        /// </summary>
        public byte Type { get; set; }

        public string Name { get; set; }

        public int OrderIndex { get; set; }
    }
}
