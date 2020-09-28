using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class CostGetPagingOutput
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
        /// 状态  <see cref="ETMS.Entity.Enum.EmGoodsStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatusDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
