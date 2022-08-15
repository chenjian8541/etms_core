using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class ClassTimesReservationLimit
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentReservationTimetableOutputStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public bool IsCanReservation { get; set; }

        public string StudentCountLimitDesc { get; set; }

        public string StudentCountSurplusDesc { get; set; }

        public string CantReservationErrDesc { get; set; }

        public int NewStudentCount { get; set; }
    }
}
