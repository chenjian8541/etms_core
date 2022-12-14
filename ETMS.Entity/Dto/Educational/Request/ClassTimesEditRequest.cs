using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Utility;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesEditRequest : RequestBase
    {
        public long ClassTimesId { get; set; }

        public DateTime ClassOt { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public List<long> TeacherIds { get; set; }

        public List<long> CourseIds { get; set; }

        public List<long> ClassRoomIds { get; set; }

        public string ClassContent { get; set; }

        /// <summary>
        /// 预约类型  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        public override string Validate()
        {
            if (ClassTimesId <= 0)
            {
                return "请求数据格式错误";
            }
            if (!ClassOt.IsEffectiveDate())
            {
                return "请选择上课日期";
            }
            if (StartTime <= 0 || EndTime <= 0)
            {
                return "请选择上课时间";
            }
            if (StartTime >= EndTime)
            {
                return "上课时间格式不正确";
            }
            if (TeacherIds == null || !TeacherIds.Any())
            {
                return "请选择上课老师";
            }
            if (CourseIds == null || !CourseIds.Any())
            {
                return "请选择授课课程";
            }
            return string.Empty;
        }
    }
}
