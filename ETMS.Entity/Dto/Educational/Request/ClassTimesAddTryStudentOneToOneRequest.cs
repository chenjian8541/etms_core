using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesAddTryStudentOneToOneRequest : RequestBase
    {
        public long StudentId { get; set; }

        public DateTime? ClassOt { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public string ClassContent { get; set; }

        public long CourseId { get; set; }

        public long TeacherId { get; set; }

        public List<long> ClassRoomIds { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            if (ClassOt == null)
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
            if (CourseId <= 0)
            {
                return "请选择课程";
            }
            if (TeacherId <= 0)
            {
                return "请选择老师";
            }
            return string.Empty;
        }
    }
}
