using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 物品
    /// </summary>
    [Table("EtGoods")]
    public class EtGoods : Entity<long>
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
        /// 库存数量
        /// </summary>
        public int InventoryQuantity { get; set; }

        /// <summary>
        /// 库存预警数
        /// </summary>
        public int? LimitQuantity { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmGoodsStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 赠送积分
        /// </summary>
        public int Points { get; set; }

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
