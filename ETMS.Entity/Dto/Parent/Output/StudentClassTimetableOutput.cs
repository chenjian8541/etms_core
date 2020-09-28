using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentClassTimetableOutput
    {
        public long Id { get; set; }

        public string StudentName { get; set; }

        public long ClassId { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public string ClassOt { get; set; }

        public string ClassOtShort { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        public string WeekDesc { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public string EndTime { get; set; }

        public string ClassRoomIds { get; set; }

        public string ClassRoomIdsDesc { get; set; }

        public string CourseStyleColor { get; set; }

        public string CourseList { get; set; }

        public string CourseListDesc { get; set; }

        public string Teachers { get; set; }

        public string TeachersDesc { get; set; }

        /// <summary>
        /// 状态   <see cref="ETMS.Entity.Enum.EmClassTimesStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }
    }
}
