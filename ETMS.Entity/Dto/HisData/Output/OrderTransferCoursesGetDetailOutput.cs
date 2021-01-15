using ETMS.Entity.Dto.Common.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class OrderTransferCoursesGetDetailOutput
    {
        public OrderTransferCoursesGetDetailBascInfo BascInfo { get; set; }

        public List<OrderTransferCoursesGetDetailOut> OutList { get; set; }

        public List<OrderTransferCoursesGetDetailIn> InList { get; set; }

        public List<OrderGetDetailIncomeLog> OrderGetDetailIncomeLogs { get; set; }
    }

    public class OrderTransferCoursesGetDetailBascInfo
    {
        public long CId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public int OrderType { get; set; }

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
    }

    public class OrderTransferCoursesGetDetailOut
    {
        public long CId { get; set; }

        public long UnionOrderId { get; set; }

        public string UnionOrderNo { get; set; }

        public string ProductName { get; set; }

        public string OutQuantity { get; set; }

        public string OutQuantityDesc { get; set; }

        public decimal ItemAptSum { get; set; }
    }

    public class OrderTransferCoursesGetDetailIn
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
}
