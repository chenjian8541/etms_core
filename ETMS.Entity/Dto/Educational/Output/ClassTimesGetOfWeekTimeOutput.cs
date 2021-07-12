using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassTimesGetOfWeekTimeOutput
    {
        public List<ClassTimesGetOfWeekTimeItem> MondyList { get; set; }

        public List<ClassTimesGetOfWeekTimeItem> TuesdayList { get; set; }
        public List<ClassTimesGetOfWeekTimeItem> WednesdayList { get; set; }

        public List<ClassTimesGetOfWeekTimeItem> ThursdayList { get; set; }

        public List<ClassTimesGetOfWeekTimeItem> FridayList { get; set; }

        public List<ClassTimesGetOfWeekTimeItem> SaturdayList { get; set; }

        public List<ClassTimesGetOfWeekTimeItem> SundayList { get; set; }

    }

    public class ClassTimesGetOfWeekTimeItem
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

        public string TimeDesc { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public string StartTime { get; set; }

        public string Startop { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        /// <summary>
        /// 授课课程
        /// </summary>
        public string CourseList { get; set; }

        public string CourseListDesc { get; set; }

        public string ClassTimesColor { get; set; }

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

        public string Duration { get; set; }

        public string ClassTimesDesc { get; set; }

        public string DefaultClassTimes { get; set; }

        /// <summary>
        /// 预约类型  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        public bool IsTry { get; set; }

        public string LimitStudentNumsDesc { get; set; }

        /// <summary>
        /// 班级类型  <see cref="ETMS.Entity.Enum.EmClassType"/>
        /// </summary>
        public byte Type { get; set; }
    }
}
