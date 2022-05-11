using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent2.Output
{
    public class StudentReservationDetailOutput
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentReservationTimetableOutputStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 班级类型  <see cref="ETMS.Entity.Enum.EmClassType"/>
        /// </summary>
        public byte ClassType { get; set; }

        public long ClassTimesId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public string ClassOt { get; set; }

        public string WeekDesc { get; set; }

        public string TimeDesc { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        public string CourseListDesc { get; set; }

        public string ClassRoomIdsDesc { get; set; }

        public string TeachersDesc { get; set; }

        public int StudentCountFinish { get; set; }

        public string StudentCountLimitDesc { get; set; }

        public string StudentCountSurplusDesc { get; set; }

        public long CourseId { get; set; }

        public bool IsCanReservation { get; set; }

        public string RuleStartClassReservaLimitDesc { get; set; }

        public string RuleDeadlineClassReservaLimitDesc { get; set; }

        public string RuleMaxCountClassReservaLimitDesc { get; set; }

        public string RuleCancelClassReservaDesc { get; set; }

        public string CantReservationErrDesc { get; set; }

        public string CancelDesc { get; set; }

        public bool IsCanCancel { get; set; }

        public StudentReservationSuccess StudentReservationSuccess { get; set; }
    }

    public class StudentReservationSuccess {

        public string StudentName { get; set; }

        public string StudentAvatarUrl { get; set; }

        public string CancelDesc { get; set; }
    }
}
