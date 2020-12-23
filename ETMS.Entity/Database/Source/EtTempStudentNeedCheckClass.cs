using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTempStudentNeedCheckClass")]
    public class EtTempStudentNeedCheckClass : Entity<long>
    {
        public long StudentId { get; set; }

        public long ClassTimesId { get; set; }

        public long ClassId { get; set; }

        public long CourseId { get; set; }

        public DateTime ClassOt { get; set; }

        public byte Week { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public DateTime Ot { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTempStudentNeedCheckClassStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
