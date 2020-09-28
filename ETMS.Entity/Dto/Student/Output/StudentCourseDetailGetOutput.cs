using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseDetailGetOutput
    {
        public long CourseId { get; set; }

        public string CourseColor { get; set; }

        public string CourseName { get; set; }

        public byte Status { get; set; }

        public string StopTimeDesc { get; set; }

        public string RestoreTimeDesc { get; set; }

        public int ExceedTotalClassTimes { get; set; }

        public DeTypeClassTimes DeTypeClassTimes { get; set; }

        public DeTypeDay DeTypeDay { get; set; }

        public List<StudentClass> StudentClass { get; set; }

        public List<StudentCourseDetail> StudentCourseDetail { get; set; }

        public List<StopLog> StopLogs { get; set; }
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
    }

    public class StudentClass
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// 按课时
    /// </summary>
    public class DeTypeClassTimes
    {
        public string SurplusQuantityDesc { get; set; }
    }

    /// <summary>
    /// 按天消耗
    /// </summary>
    public class DeTypeDay
    {
        public string SurplusQuantityDesc { get; set; }
    }

    public class StopLog
    {
        public string StopTimeDesc { get; set; }

        public string RestoreTimeDesc { get; set; }

        public int StopDay { get; set; }

        public string Remark { get; set; }
    }
}
