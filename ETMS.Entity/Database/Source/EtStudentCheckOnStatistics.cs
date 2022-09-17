using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStudentCheckOnStatistics")]
    public class EtStudentCheckOnStatistics : Entity<long>
    {
        public int CheckInCount { get; set; }

        public int CheckOutCount { get; set; }

        public int CheckAttendClassCount { get; set; }

        public DateTime Ot { get; set; }
    }
}
