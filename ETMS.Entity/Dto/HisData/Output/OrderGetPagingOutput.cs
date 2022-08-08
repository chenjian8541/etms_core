using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class OrderGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public int OrderType { get; set; }

        public string OrderTypeDesc { get; set; }

        /// <summary>
        /// 订单来源  <see cref="ETMS.Entity.Enum.EmOrderSource"/>
        /// </summary>
        public int OrderSource { get; set; }

        public string OrderSourceDesc { get; set; }

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

        public string BuyOther { get; set; }

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

        /// <summary>
        /// 业绩归属人
        /// </summary>
        public string CommissionUser { get; set; }

        public string CommissionUserDesc { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EmOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal Sum2 { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal AptSum2 { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PaySum2 { get; set; }
    }
}
