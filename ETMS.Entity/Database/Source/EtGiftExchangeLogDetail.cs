using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 礼品兑换详情
    /// </summary>
    [Table("EtGiftExchangeLogDetail")]
    public class EtGiftExchangeLogDetail : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 礼品ID
        /// </summary>
        public long GiftId { get; set; }

        /// <summary>
        /// 兑换ID
        /// </summary>
        public long GiftExchangeLogId { get; set; }

        /// <summary>
        /// 兑换单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmGiftExchangeLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 消耗积分
        /// </summary>
        public int ItemPoints { get; set; }
    }
}
