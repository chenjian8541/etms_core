using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleAdd2Request : RequestBase
    {
        public long ClassId { get; set; }

        public List<string> ClassDate { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public List<long> ClassRoomIds { get; set; }

        public List<long> TeacherIds { get; set; }

        public List<long> CourseIds { get; set; }

        public string ClassContent { get; set; }

        public bool IsJumpHoliday { get; set; }

        /// <summary>
        /// 预约类型  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        public bool IsJumpTeacherLimit { get; set; }

        public bool IsJumpStudentLimit { get; set; }

        public bool IsJumpClassRoomLimit { get; set; }

        public override string Validate()
        {
            if (ClassId <= 0)
            {
                return "请选择班级";
            }
            if (ClassDate == null || !ClassDate.Any())
            {
                return "请选择上课日期";
            }
            if (ClassDate.Count > 100)
            {
                return "一次性最多选择50个日期";
            }
            if (StartTime <= 0 || EndTime <= 0)
            {
                return "请选择上课时间";
            }
            if (StartTime >= EndTime)
            {
                return "上课时间格式不正确";
            }
            return string.Empty;
        }
    }
}
