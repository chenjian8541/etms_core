using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSalaryPayroll")]
    public class EtTeacherSalaryPayroll : Entity<long>
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int UserCount { get; set; }

        public string UserIds { get; set; }

        public DateTime? PayDate { get; set; }

        public decimal PaySum { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryPayrollStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public DateTime Ot { get; set; }

        public long OpUserId { get; set; }
    }
}
