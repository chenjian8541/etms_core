using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassTimesGetOfWeekTime2Output
    {
        public ClassTimesGetOfWeekTime2Output()
        {
            this.MondyList = new List<ClassTimesGetOfWeekTimeTeacherItems>();
            this.TuesdayList = new List<ClassTimesGetOfWeekTimeTeacherItems>();
            this.WednesdayList = new List<ClassTimesGetOfWeekTimeTeacherItems>();
            this.ThursdayList = new List<ClassTimesGetOfWeekTimeTeacherItems>();
            this.FridayList = new List<ClassTimesGetOfWeekTimeTeacherItems>();
            this.SaturdayList = new List<ClassTimesGetOfWeekTimeTeacherItems>();
            this.SundayList = new List<ClassTimesGetOfWeekTimeTeacherItems>();
        }

        public string Name { get; set; }

        public int TotalCount { get; set; }

        public IEnumerable<ClassTimesGetOfWeekTimeTeacherItems> MondyList { get; set; }

        public IEnumerable<ClassTimesGetOfWeekTimeTeacherItems> TuesdayList { get; set; }

        public IEnumerable<ClassTimesGetOfWeekTimeTeacherItems> WednesdayList { get; set; }

        public IEnumerable<ClassTimesGetOfWeekTimeTeacherItems> ThursdayList { get; set; }

        public IEnumerable<ClassTimesGetOfWeekTimeTeacherItems> FridayList { get; set; }

        public IEnumerable<ClassTimesGetOfWeekTimeTeacherItems> SaturdayList { get; set; }

        public IEnumerable<ClassTimesGetOfWeekTimeTeacherItems> SundayList { get; set; }
    }

    public class ClassTimesGetOfWeekTimeTeacherItems
    {
        public long CId { get; set; }
        public string ClassName { get; set; }

        public string TimeDesc { get; set; }

        public string Color { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

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

        public int DefaultClassTimes { get; set; }
    }
}
