using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 课消记录
    /// </summary>
    [Table("EtStudentCourseConsumeLog")]
    public class EtStudentCourseConsumeLog : Entity<long>
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
        /// 订单ID
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 数据来源 <see cref="ETMS.Entity.Enum.EmStudentCourseConsumeSourceType"/>
        /// </summary>
        public int SourceType { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 扣课时规则 <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        /// <summary>
        /// 扣的课时
        /// </summary>
        public decimal DeClassTimes { get; set; }

        public decimal DeClassTimesSmall { get; set; }

        public string SurplusCourseDesc { get; set; }
    }
}
