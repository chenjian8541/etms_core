using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class OrderGetDetailPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 经办人
        /// </summary>
        public long UserId { get; set; }

        public string UserName { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

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

        public string InOutTypeDesc { get; set; }

        public string OrderTypeDesc { get; set; }

        public int OrderType { get; set; }

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
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmProductType"/>
        /// </summary>
        public byte ProductType { get; set; }

        public string ProductTypeDesc { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public long ProductId { get; set; }

        public string ProductName { get; set; }

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


        public string BuyQuantityDesc { get; set; }

        public string GiveQuantityDesc { get; set; }

        /// <summary>
        /// 单项总金额
        /// </summary>
        public decimal ItemSum { get; set; }

        /// <summary>
        /// 单项折后额
        /// </summary>
        public decimal ItemAptSum { get; set; }

        /// <summary>
        /// 折扣类别  <see cref="ETMS.Entity.Enum.EmDiscountType"/>
        /// </summary>
        public byte DiscountType { get; set; }

        /// <summary>
        /// 折扣值
        /// </summary>
        public decimal DiscountValue { get; set; }
        public string DiscountDesc { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public string OtDesc { get; set; }

        /// <summary>
        /// 签约类型 <see cref="ETMS.Entity.Enum.EmOrderBuyType">
        /// </summary>
        public byte BuyType { get; set; }

        public string BuyTypeDesc { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
