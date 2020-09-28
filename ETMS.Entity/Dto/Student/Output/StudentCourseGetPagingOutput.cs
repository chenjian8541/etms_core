using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseGetPagingOutput
    {
        public long CId { get; set; }

        public long StudentId { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// 课消方式 <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public string DeTypeDesc { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public string BuyQuantityDesc { get; set; }

        public string GiveQuantityDesc { get; set; }

        public string UseQuantityDesc { get; set; }

        public string SurplusQuantityDesc { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmStudentCourseStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 超上课时
        /// </summary>
        public int ExceedTotalClassTimes { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }
    }
}
