using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 收支明细
    /// </summary>
    [Table("EtIncomeLog")]
    public class EtIncomeLog : Entity<long>
    {
        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmIncomeLogType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 关联单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 关联订单
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// 关联ID
        /// </summary>
        public long? RelationId { get; set; }

        /// <summary>
        /// 项目名称  <see cref=" ETMS.Entity.Enum.EmIncomeLogProjectType"/>
        /// </summary>
        public long ProjectType { get; set; }

        /// <summary>
        /// 支付方式 <see cref="ETMS.Entity.Enum.EmPayType"/>
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 收支金额
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string AccountNo { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmIncomeLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 经办人
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 作废时间
        /// </summary>
        public DateTime? RepealOt { get; set; }

        /// <summary>
        /// 作废人
        /// </summary>
        public long? RepealUserId { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateOt { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
