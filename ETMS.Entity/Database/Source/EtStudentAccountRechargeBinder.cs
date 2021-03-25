using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStudentAccountRechargeBinder")]
    public class EtStudentAccountRechargeBinder: Entity<long>
    {
        public long StudentAccountRechargeId { get; set; }

        public long StudentId { get; set; }
    }
}
