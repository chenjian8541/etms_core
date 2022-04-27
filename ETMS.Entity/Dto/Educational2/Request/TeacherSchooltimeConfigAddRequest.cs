using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational2.Request
{
    public class TeacherSchooltimeConfigAddRequest : RequestBase
    {
        public long TeacherId { get; set; }

        public long? CourseId { get; set; }

        public List<byte> Weeks { get; set; }

        public bool IsJumpHoliday { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public override string Validate()
        {
            if (TeacherId <= 0)
            {
                return "请选择老师";
            }
            if (Weeks == null || Weeks.Count == 0)
            {
                return "请选择周几上课";
            }
            if (StartTime == 0 || EndTime == 0)
            {
                return "请选择上课时间";
            }
            return base.Validate();
        }
    }
}
