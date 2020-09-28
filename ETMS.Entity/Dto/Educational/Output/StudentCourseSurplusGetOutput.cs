using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class StudentCourseSurplusGetOutput
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 消耗课程ID
        /// </summary>
        public long CourseId { get; set; }

        public string CourseName { get; set; }

        /// <summary>
        /// 课次剩余描述
        /// </summary>
        public string CourseSurplusDesc { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmClassStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        public string StudentTypeDesc { get; set; }
    }
}
