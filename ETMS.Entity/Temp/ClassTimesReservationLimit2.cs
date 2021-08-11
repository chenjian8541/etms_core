using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class ClassTimesReservationLimit2
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentReservationTimetableOutputStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public bool IsCanReservation { get; set; }

        public string StudentCountLimitDesc { get; set; }

        public string StudentCountSurplusDesc { get; set; }

        public string RuleStartClassReservaLimitDesc { get; set; } = "不限制";

        public string RuleDeadlineClassReservaLimitDesc { get; set; } = "不限制";

        public string RuleMaxCountClassReservaLimitDesc { get; set; } = "不限制";

        public string RuleCancelClassReservaDesc { get; set; } = "不限制";

        public string CantReservationErrDesc { get; set; }

        public string CancelDesc { get; set; }

        public bool IsCanCancel { get; set; }

        public long CourseId { get; set; }
    }
}
