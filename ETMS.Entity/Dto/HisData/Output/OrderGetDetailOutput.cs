using ETMS.Entity.Dto.Common.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class OrderGetDetailOutput
    {
        public OrderGetDetailBascInfo BascInfo { get; set; }

        public List<OrderGetDetailCoupons> OrderGetDetailCoupons { get; set; }

        public List<OrderGetDetailProductInfo> OrderGetDetailProducts { get; set; }

        public List<OrderGetDetailIncomeLog> OrderGetDetailIncomeLogs { get; set; }
    }

    public class OrderGetDetailBascInfo
    {
        public long CId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentCardNo { get; set; }

        public int OrderType { get; set; }

        public string OrderTypeDesc { get; set; }

        /// <summary>
        /// 支出类型 <see cref="ETMS.Entity.Enum.EmOrderInOutType"/>
        /// </summary>
        public byte InOutType { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 购买的课程以”,”隔开
        /// </summary>
        public string BuyCourse { get; set; }

        /// <summary>
        /// 购买的商品
        /// </summary>
        public string BuyGoods { get; set; }

        public string BuyOther { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public string BuyCost { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal AptSum { get; set; }

        /// <summary>
        /// 直减/折扣
        /// </summary>
        public decimal GiveSum { get; set; }

        /// <summary>
        /// 获得积分
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public string TotalPointsDesc { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PaySum { get; set; }

        /// <summary>
        /// 欠费金额
        /// </summary>
        public decimal ArrearsSum { get; set; }

        /// <summary>
        /// 经办人
        /// </summary>
        public long UserId { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public string OtDesc { get; set; }

        public DateTime CreateOt { get; set; }

        /// <summary>
        /// 业绩归属人
        /// </summary>
        public string CommissionUser { get; set; }

        public string CommissionUserDesc { get; set; }

        public List<MultiSelectValueRequest> CommissionUserIds { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EmOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string UnionOrderId { get; set; }

        public string UnionOrderNo { get; set; }

        public byte IsReturn { get; set; }

        public byte IsTransferCourse { get; set; }

        public bool IsHasCourse { get; set; }

        public bool IsOnlyOneToOneCourse { get; set; }
    }

    public class OrderGetDetailCoupons
    {
        public long CId { get; set; }
        public string CouponsTitle { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmCouponsType"/>
        /// </summary>
        public byte CouponsType { get; set; }

        public string CouponsTypeDesc { get; set; }

        public decimal CouponsValue { get; set; }

        public string CouponsValueDesc { get; set; }

        public decimal? CouponsMinLimit { get; set; }

        public string MinLimitDesc { get; set; }
    }

    public class OrderGetDetailProductInfo
    {
        public long CId { get; set; }

        public string ProductTypeDesc { get; set; }

        public string ProductName { get; set; }

        public string PriceRule { get; set; }

        public int BuyQuantity { get; set; }

        public byte BugUnit { get; set; }

        public string BuyQuantityDesc { get; set; }

        public string GiveQuantityDesc { get; set; }

        public decimal ItemSum { get; set; }

        public decimal ItemAptSum { get; set; }

        public string DiscountDesc { get; set; }

        /// <summary>
        /// 退货数量/或者转课数量
        /// </summary>
        public decimal OutQuantity { get; set; }

        public string OutQuantityDesc { get; set; }
    }

    public class OrderGetDetailIncomeLog
    {
        public long CId { get; set; }

        public long ProjectType { get; set; }

        public string ProjectTypeName { get; set; }

        public string PayOt { get; set; }

        public string UserName { get; set; }

        public int PayType { get; set; }

        public string PayTypeDesc { get; set; }

        public decimal Sum { get; set; }
    }
}
