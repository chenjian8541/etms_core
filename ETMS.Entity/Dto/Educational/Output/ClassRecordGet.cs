using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassRecordGet
    {
        public long CId { get; set; }

        public long ClassId { get; set; }

        public string ClassName { get; set; }

        public DateTime CheckOt { get; set; }

        public string ClassOtDesc { get; set; }

        public string ClassTimeDesc { get; set; }

        public string CourseListDesc { get; set; }

        public string ClassRoomIdsDesc { get; set; }

        public string TeachersDesc { get; set; }

        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public long CheckUserId { get; set; }

        public string CheckUserDesc { get; set; }

        public int AttendNumber { get; set; }

        public int NeedAttendNumber { get; set; }

        public string ClassTimes { get; set; }

        public string DeSum { get; set; }

        public string ClassContent { get; set; }

        public string WeekDesc { get; set; }

        /// <summary>
        /// 请假是否收费
        /// </summary>
        public bool IsLeaveCharge { get; set; }

        /// <summary>
        /// 未到是否收费
        /// </summary>
        public bool IsNotComeCharge { get; set; }

        public string Remark { get; set; }
    }
}
