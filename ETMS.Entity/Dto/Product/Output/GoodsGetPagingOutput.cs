using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class GoodsGetPagingOutput
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
        /// 销售数量
        /// </summary>
        public int SaleQuantity { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int InventoryQuantity { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmGoodsStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public string LimitQuantityDesc { get; set; }

        public int Points { get; set; }
    }
}
