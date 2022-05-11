using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent5.Request
{
    public class StudentReservation1v1LessonsSubmitRequest : ParentRequestBase
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public long TeacherId { get; set; }

        public long ClassId { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public DateTime? ClassOt { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请求数据格式错误";
            }
            if (CourseId <= 0)
            {
                return "请选择课程";
            }
            if (TeacherId <= 0)
            {
                return "请选择老师";
            }
            if (ClassId <= 0)
            {
                return "请求数据格式错误";
            }
            if (ClassOt == null)
            {
                return "请求数据格式错误";
            }
            if (StartTime <= 0 || EndTime <= 0)
            {
                return "请选择预约时间";
            }
            return base.Validate();
        }
    }
}
