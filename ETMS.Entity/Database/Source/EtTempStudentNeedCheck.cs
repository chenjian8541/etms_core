using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTempStudentNeedCheck")]
    public class EtTempStudentNeedCheck : Entity<long>
    {
        public long StudentId { get; set; }

        public string ClassDesc { get; set; }

        public int StartTime { get; set; }

        public DateTime Ot { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsCheckIn { get; set; }

        public DateTime? CheckInOt { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsCheckOut { get; set; }

        public DateTime? CheckOutOt { get; set; }
    }
}
