using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseDetailGetOutput
    {
        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public long CourseId { get; set; }

        public string CourseColor { get; set; }

        public string CourseName { get; set; }

        /// <summary>
        /// 类型   <see cref="ETMS.Entity.Enum.EmCourseType"/>
        /// </summary>
        public byte Type { get; set; }

        public byte Status { get; set; }

        public string StopTimeDesc { get; set; }

        public string RestoreTimeDesc { get; set; }

        public string ExceedTotalClassTimes { get; set; }

        public string SurplusQuantityDesc { get; set; }

        public List<StudentClass> StudentClass { get; set; }

        public List<StudentCourseDetail> StudentCourseDetail { get; set; }

        public List<OpLog> OpLogs { get; set; }
    }

    public class StudentCourseDetail
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

    public class StudentClass
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class OpLog
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public string CourseName { get; set; }

        public string OpContent { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmStudentCourseOpLogType"/>
        /// </summary>
        public int OpType { get; set; }

        public string OpTypeDesc { get; set; }

        public DateTime OpTime { get; set; }

        public long OpUser { get; set; }

        public string OpUserName { get; set; }

        public string Remark { get; set; }
    }
}
