using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseAboutFastDeClassTimesGet2
    {
        /// <summary>
        /// 课消方式 <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        public string DeTypeDesc { get; set; }

        public long CourseId { get; set; }

        public string CourseName { get; set; }

        public string SurplusQuantityDesc { get; set; }
    }
}
