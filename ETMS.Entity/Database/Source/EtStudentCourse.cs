using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员课程信息
    /// </summary>
    [Table("EtStudentCourse")]
    public class EtStudentCourse : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public long CourseId { get; set; }

        /// <summary>
        /// 课消方式 <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int BuyQuantity { get; set; }

        /// <summary>
        /// 购买单位   <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte BugUnit { get; set; }

        /// <summary>
        /// 赠送数量(课时/月)
        /// </summary>
        public int GiveQuantity { get; set; }

        /// <summary>
        /// 赠送数量 (天)
        /// </summary>
        public int GiveSmallQuantity { get; set; }

        /// <summary>
        /// 消耗数量
        /// </summary>
        public decimal UseQuantity { get; set; }

        /// <summary>
        /// 消耗单位  <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte UseUnit { get; set; }

        /// <summary>
        /// 剩余数量(课时/月)
        /// </summary>
        public decimal SurplusQuantity { get; set; }

        /// <summary>
        /// 剩余数量(天)
        /// </summary>
        public decimal SurplusSmallQuantity { get; set; }

        /// <summary>
        /// 课程分析JOB最后执行时间
        /// </summary>
        public DateTime LastJobProcessTime { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmStudentCourseStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 停课时间
        /// </summary>
        public DateTime? StopTime { get; set; }

        /// <summary>
        /// 复课时间
        /// </summary>
        public DateTime? RestoreTime { get; set; }

        /// <summary>
        /// 超上课时
        /// </summary>
        public decimal ExceedTotalClassTimes { get; set; }
    }
}
