using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员课程详情
    /// </summary>
    [Table("EtStudentCourseDetail")]
    public class EtStudentCourseDetail : Entity<long>
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
        public long OrderId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmStudentCourseStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 扣课时类型  <see cref=" ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        /// <summary>
        /// 课程单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 课程总价
        /// </summary>
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int BuyQuantity { get; set; }

        /// <summary>
        /// 购买单位  <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte BugUnit { get; set; }

        /// <summary>
        /// 赠送数量
        /// </summary>
        public int GiveQuantity { get; set; }

        /// <summary>
        /// 赠送单位 <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte GiveUnit { get; set; }

        /// <summary>
        /// 消耗数量
        /// </summary>
        public decimal UseQuantity { get; set; }

        /// <summary>
        /// 消耗单位  <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte UseUnit { get; set; }

        /// <summary>
        /// 剩余数量（课时/月）
        /// </summary>
        public decimal SurplusQuantity { get; set; }

        /// <summary>
        /// 剩余数量(天)
        /// </summary>
        public decimal SurplusSmallQuantity { get; set; }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 截至时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 课程分析JOB最后执行时间
        /// </summary>
        public DateTime? LastJobProcessTime { get; set; }

        /// <summary>
        /// 结课时间
        /// </summary>
        public DateTime? EndCourseTime { get; set; }

        /// <summary>
        /// 结课人
        /// </summary>
        public long? EndCourseUser { get; set; }

        /// <summary>
        /// 结课备注
        /// </summary>
        public string EndCourseRemark { get; set; }

        public long? PriceRuleId { get; set; }
    }
}
