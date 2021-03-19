using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentReservationTimetableDetailOutput
    {
        public long ClassTimesId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentReservationTimetableOutputStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public string StartTimeDesc { get; set; }

        public string EndTimeDesc { get; set; }

        public string ClassName { get; set; }

        public string CourseListDesc { get; set; }

        public string TeachersDesc { get; set; }

        public string Color { get; set; }

        public int StudentCountFinish { get; set; }

        public string StudentCountLimitDesc { get; set; }

        public bool IsCanReservation { get; set; }
    }
}
