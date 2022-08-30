using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseConsumeLogGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public long CourseId { get; set; }

        public string CourseName { get; set; }

        /// <summary>
        /// 数据来源 <see cref="ETMS.Entity.Enum.EmStudentCourseConsumeSourceType"/>
        /// </summary>
        public int SourceType { get; set; }

        public string SourceTypeDesc { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 扣课时规则 <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        public string DeTypeDesc { get; set; }

        /// <summary>
        /// 扣的课时
        /// </summary>
        public string DeClassTimes { get; set; }

        public string DeClassTimesSmall { get; set; }

        public string DeClassTimesDesc { get; set; }

        public string SurplusCourseDesc { get; set; }

        public decimal DeSum { get; set; }

        public string Remark { get; set; }
    }
}
