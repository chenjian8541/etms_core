using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class GoodsGetOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 零售单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 库存预警数
        /// </summary>
        public string LimitQuantity { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int InventoryQuantity { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public byte Status { get; set; }
    }
}
