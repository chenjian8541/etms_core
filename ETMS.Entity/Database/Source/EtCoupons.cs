using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 优惠券 
    /// </summary>
    [Table("EtCoupons")]
    public class EtCoupons : Entity<long>
    {
        /// <summary>
		/// 类型  <see cref="ETMS.Entity.Enum.EmCouponsType"/>
		/// </summary>
		public byte Type { get; set; }

        /// <summary>
        /// 优惠额度
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 优惠券标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 最低消费
        /// </summary>
        public decimal? MinLimit { get; set; }

        /// <summary>
        /// 总量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 领券数量
        /// </summary>
        public int GetCount { get; set; }

        /// <summary>
        /// 核销数量
        /// </summary>
        public int UsedCount { get; set; }

        /// <summary>
        /// 使用说明
        /// </summary>
        public string UseExplain { get; set; }

        /// <summary>
        /// 过期类型   <see cref="ETMS.Entity.Enum.EmCouponsExpiredType"/>
        /// </summary>
        public byte ExpiredType { get; set; }

        /// <summary>
        /// 开始时间
        /// 标注此券允许开始使用的日期，如果为null，则自动填入派发日期
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 截至时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 截止有效期偏移天数  
        /// 有效期，0表示无限制
        /// </summary>
        public int EndOffset { get; set; }

        /// <summary>
        /// 单人单日限领
        /// 0表示不限制
        /// </summary>
        public int LimitGetSingle { get; set; }

        /// <summary>
        /// 单人限领
        /// 0表示不限制
        /// </summary>
        public int LimitGetAll { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmCouponsStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
