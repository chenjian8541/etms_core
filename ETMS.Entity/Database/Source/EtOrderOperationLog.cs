using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 订单操作日志
    /// </summary>
    [Table("EtOrderOperationLog")]
    public class EtOrderOperationLog : Entity<long>
    {
        /// <summary>
		/// 订单ID
		/// </summary>
		public long OrderId { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 操作类型  <see cref="ETMS.Entity.Enum.EmOrderOperationLogType"/>
        /// </summary>
        public int OpType { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string OpContent { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
