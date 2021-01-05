using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 课程收费规则
    /// </summary>
    [Table("EtCoursePriceRule")]
    public class EtCoursePriceRule : Entity<long>
    {
        /// <summary>
		/// 课程ID
		/// </summary>
		public long CourseId { get; set; }

        /// <summary>
        /// 收费类型 <see cref="ETMS.Entity.Enum.EmCoursePriceType"/>
        /// </summary>
        public byte PriceType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 单价单位  <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte PriceUnit { get; set; }

        /// <summary>
        /// 赠送积分
        /// </summary>
        public int Points { get; set; }
    }
}
