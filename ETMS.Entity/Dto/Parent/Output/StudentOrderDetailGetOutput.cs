using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentOrderDetailGetOutput
    {
        public ParentOrderGetDetailBascInfo BascInfo { get; set; }

        public List<ParentOrderGetDetailCoupons> OrderGetDetailCoupons { get; set; }

        public List<ParentOrderGetDetailProductInfo> OrderGetDetailProducts { get; set; }

        public List<ParentOrderGetDetailIncomeLog> OrderGetDetailIncomeLogs { get; set; }
    }

    public class ParentOrderGetDetailBascInfo
    {
        public long Id { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public int OrderType { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal AptSum { get; set; }

        /// <summary>
        /// 获得积分
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PaySum { get; set; }

        /// <summary>
        /// 欠费金额
        /// </summary>
        public decimal ArrearsSum { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public string OtDesc { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EmOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }
    }

    public class ParentOrderGetDetailCoupons
    {
        public long Id { get; set; }
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

    public class ParentOrderGetDetailProductInfo
    {
        public long Id { get; set; }

        public byte ProductType { get; set; }

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
    }

    public class ParentOrderGetDetailIncomeLog
    {
        public long Id { get; set; }

        public long ProjectType { get; set; }

        public string ProjectTypeName { get; set; }

        public string PayOt { get; set; }

        public int PayType { get; set; }

        public string PayTypeDesc { get; set; }

        public decimal Sum { get; set; }
    }
}

