using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassRecordStudentGetSimpleOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        public string GenderDesc { get; set; }

        /// <summary>
        /// 消耗课程ID
        /// </summary>
        public long CourseId { get; set; }

        public string CourseName { get; set; }

        /// <summary>
        /// 课次剩余描述
        /// </summary>
        public string CourseSurplusDesc { get; set; }
    }
}
