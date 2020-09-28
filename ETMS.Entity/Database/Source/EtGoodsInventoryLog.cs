using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 商品库存变动
    /// </summary>
    [Table("EtGoodsInventoryLog")]
    public class EtGoodsInventoryLog : Entity<long>
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public long GoodsId { get; set; }

        /// <summary>
        /// 经办人
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmGoodsInventoryType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 变动数量
        /// </summary>
        public int ChangeQuantity { get; set; }

        /// <summary>
        /// 出入库单价
        /// </summary>
        public decimal Prince { get; set; }

        /// <summary>
        /// 出入口总价
        /// </summary>
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
