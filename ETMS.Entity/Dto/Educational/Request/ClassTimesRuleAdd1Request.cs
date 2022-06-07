using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleAdd1Request : RequestBase
    {
        public long ClassId { get; set; }

        public DateTime? StartDate { get; set; }

        /// <summary>
        /// <see cref="ClassTimesRuleEndType"/>
        /// </summary>
        public byte EndType { get; set; }

        public string EndValue { get; set; }

        /// <summary>
        /// 每周几上课
        /// </summary>
        public List<int> Weeks { get; set; }

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

        public override string Validate()
        {
            if (ClassId <= 0)
            {
                return "请选择班级";
            }
            if (StartDate == null)
            {
                return "请选择开始日期";
            }
            var maxDate = DateTime.Now.AddYears(2);
            if (StartDate > maxDate)
            {
                return "开始日期距离现在不能超过2年";
            }
            if (EndType == ClassTimesRuleEndType.DateTime)
            {
                if (string.IsNullOrEmpty(EndValue) || !DateTime.TryParse(EndValue, out var tempTime) || tempTime <= StartDate)
                {
                    return "请选择结束日期";
                }
            }
            if (EndType == ClassTimesRuleEndType.Count)
            {
                if (string.IsNullOrEmpty(EndValue) || !int.TryParse(EndValue, out var tempCount) || tempCount <= 0)
                {
                    return "请输入课次";
                }
                if (tempCount > 100)
                {
                    return "课次最大值100";
                }
            }
            if (Weeks == null || !Weeks.Any())
            {
                return "请选择周几上课";
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

    public struct ClassTimesRuleEndType
    {

        /// <summary>
        /// 不结束
        /// </summary>
        public const byte NotOver = 0;

        /// <summary>
        /// 限日期
        /// </summary>
        public const byte DateTime = 1;

        /// <summary>
        /// 限次数
        /// </summary>
        public const byte Count = 2;
    }
}
