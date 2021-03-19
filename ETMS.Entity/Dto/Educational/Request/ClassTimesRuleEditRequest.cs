using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleEditRequest : RequestBase
    {
        public long ClassRuleId { get; set; }

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

        public override string Validate()
        {
            if (ClassRuleId <= 0)
            {
                return "数据格式错误";
            }
            if (CourseIds == null || CourseIds.Count == 0)
            {
                return "请选择授课课程";
            }
            if (TeacherIds == null || TeacherIds.Count == 0)
            {
                return "请选择上课老师";
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
