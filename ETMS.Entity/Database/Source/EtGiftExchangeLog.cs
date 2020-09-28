using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 礼品兑换记录
    /// </summary>
    [Table("EtGiftExchangeLog")]
    public class EtGiftExchangeLog : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 兑换单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 兑换类型  <see cref="ETMS.Entity.Enum.EmExchangeType"/>
        /// </summary>
        public byte ExchangeType { get; set; }

        /// <summary>
        /// 总积分
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 兑换时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmGiftExchangeLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
