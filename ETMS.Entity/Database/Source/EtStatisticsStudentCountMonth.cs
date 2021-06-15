using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum;
namespace ETMS.Entity.Database.Source
{
    [Table("EtStatisticsStudentCountMonth")]
    public class EtStatisticsStudentCountMonth : Entity<long>
    {
        public int AddStudentCount { get; set; }

        public DateTime Ot { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }
    }
}
