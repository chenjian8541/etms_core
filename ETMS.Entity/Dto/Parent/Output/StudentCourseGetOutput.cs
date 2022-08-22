using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentCourseGetOutput
    {
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        /// <summary>
        /// 类型   <see cref="ETMS.Entity.Enum.EmCourseType"/>
        /// </summary>
        public byte Type { get; set; }

        public long CourseId { get; set; }

        public string CourseColor { get; set; }

        public string CourseName { get; set; }

        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public string SurplusQuantityDesc { get; set; }

        public string ExpireDateDesc { get; set; }

        //public ParentDeTypeClassTimes DeTypeClassTimes { get; set; }

        //public ParentDeTypeDay DeTypeDay { get; set; }

        public List<ParentStudentClass> StudentClass { get; set; }

        public List<StudentCourseDetailOutput> StudentCourseDetail { get; set; }
    }

    /// <summary>
    /// 按课时
    /// </summary>
    public class ParentDeTypeClassTimes
    {
        public string SurplusQuantityDesc { get; set; }
    }

    /// <summary>
    /// 按天消耗
    /// </summary>
    public class ParentDeTypeDay
    {
        public string SurplusQuantityDesc { get; set; }
    }

    public class ParentStudentClass
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class StudentCourseDetailOutput
    {
        public long CId { get; set; }

        public string OrderNo { get; set; }

        public long OrderId { get; set; }

        public byte DeType { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public string BuyQuantityDesc { get; set; }

        public string GiveQuantityDesc { get; set; }

        public string UseQuantityDesc { get; set; }

        public string SurplusQuantityDesc { get; set; }

        public string ExpirationDate { get; set; }

        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public string EndCourseRemark { get; set; }

        /// <summary>
        /// 剩余数量（课时/月）
        /// </summary>
        public decimal SurplusQuantity { get; set; }

        /// <summary>
        /// 剩余数量(天)
        /// </summary>
        public decimal SurplusSmallQuantity { get; set; }

        /// <summary>
        /// 截至时间
        /// </summary>
        public string EndTime { get; set; }
    }
}
