using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassTimesGetViewOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public string ClassOt { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        public string WeekDesc { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public int EndTime { get; set; }

        public string TimeDesc { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        /// <summary>
        /// 授课课程
        /// </summary>
        public string CourseList { get; set; }

        public string CourseListDesc { get; set; }

        /// <summary>
        /// 教室信息
        /// </summary>
        public string ClassRoomIds { get; set; }

        public string ClassRoomIdsDesc { get; set; }

        /// <summary>
        /// 老师
        /// </summary>
        public string Teachers { get; set; }

        public string TeachersDesc { get; set; }

        /// <summary>
        /// 老师个数
        /// </summary>
        public int TeacherNum { get; set; }

        /// <summary>
        /// 状态   <see cref="ETMS.Entity.Enum.EmClassTimesStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 默认课时
        /// </summary>
        public int DefaultClassTimes { get; set; }
    }
}
