using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class SuitGetPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 赠送积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 是否启用 <see cref="ETMS.Entity.Enum.EmProductStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }
    }
}
