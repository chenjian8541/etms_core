using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 订单详情
    /// </summary>
    [Table("EtOrderDetail")]
    public class EtOrderDetail : Entity<long>
    {
        /// <summary>
        /// 经办人
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 支出类型 <see cref="ETMS.Entity.Enum.EmOrderInOutType"/>
        /// </summary>
        public byte InOutType { get; set; }

        /// <summary>
        /// 退货数量/或者转课数量
        /// </summary>
        public decimal OutQuantity { get; set; }

        /// <summary>
        /// 退 订单ID
        /// </summary>
        public long? OutOrderId { get; set; }

        /// <summary>
        /// 退 订单号
        /// </summary>
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmOrderProductType"/>
        /// </summary>
        public byte ProductType { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 定价标准
        /// </summary>
        public string PriceRule { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int BuyQuantity { get; set; }

        /// <summary>
        /// 购买单位
        /// </summary>
        public byte BugUnit { get; set; }

        /// <summary>
        /// 赠送数量
        /// </summary>
        public int GiveQuantity { get; set; }

        /// <summary>
        /// 赠送单位
        /// </summary>
        public byte GiveUnit { get; set; }

        /// <summary>
        /// 单项总金额
        /// </summary>
        public decimal ItemSum { get; set; }

        /// <summary>
        /// 单项折后额
        /// </summary>
        public decimal ItemAptSum { get; set; }

        /// <summary>
        /// 折扣类别  <see cref="ETMS.Entity.Enum.EmOrderDiscountType"/>
        /// </summary>
        public byte DiscountType { get; set; }

        /// <summary>
        /// 折扣值
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// 订单时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
