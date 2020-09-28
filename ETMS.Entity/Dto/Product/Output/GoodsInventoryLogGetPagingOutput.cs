using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class GoodsInventoryLogGetPagingOutput
    {
        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmGoodsInventoryType"/>
        /// </summary>
        public int Type { get; set; }

        public string TypeDesc { get; set; }

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
        public string OtDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string GoodsName { get; set; }

        public string UserName { get; set; }
    }
}
