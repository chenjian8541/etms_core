using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 费用
    /// </summary>
    [Table("EtCost")]
    public class EtCost : Entity<long>
    {
        /// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }

        /// <summary>
        /// 零售单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public int SaleQuantity { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmCostStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 创建人
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
