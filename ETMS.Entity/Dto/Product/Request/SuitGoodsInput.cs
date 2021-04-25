using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Request
{
    public class SuitGoodsInput : IValidate
    {
        public long GoodsId { get; set; }

        public int BuyQuantity { get; set; }

        /// <summary>
        /// 折扣  <see cref="ETMS.Entity.Enum.EmDiscountType"/>
        /// </summary>
        public byte DiscountType { get; set; }

        /// <summary>
        /// 优惠值
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// 应收金额（优惠完的金额）
        /// </summary>
        public decimal ItemAptSum { get; set; }

        public string Validate()
        {
            if (GoodsId <= 0)
            {
                return "请求数据格式错误";
            }
            if (BuyQuantity <= 0)
            {
                return "购买数量必须大于0";
            }
            return string.Empty;
        }
    }
}
