using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 订单
    /// </summary>
    [Table("EtOrder")]
    public class EtOrder : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 充值账户ID
        /// </summary>
        public long? StudentAccountRechargeId { get; set; }

        /// <summary>
        /// 订单类型   <see cref="ETMS.Entity.Enum.EmOrderType"/>
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 签约类型 <see cref="ETMS.Entity.Enum.EmOrderBuyType">
        /// </summary>
        public byte BuyType { get; set; }

        /// <summary>
        /// 支出类型 <see cref="ETMS.Entity.Enum.EmOrderInOutType"/>
        /// </summary>
        public byte InOutType { get; set; }

        public string CouponsIds { get; set; }

        /// <summary>
        /// 核销的学员优惠券Id
        /// </summary>
        public string CouponsStudentGetIds { get; set; }

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
        /// 其它
        /// </summary>
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
        /// 支付金额
        /// </summary>
        public decimal PaySum { get; set; }

        /// <summary>
        /// 欠费金额
        /// </summary>
        public decimal ArrearsSum { get; set; }

        /// <summary>
        /// 充值账户抵扣 (实充)
        /// </summary>
        public decimal PayAccountRechargeReal { get; set; }

        /// <summary>
        /// 充值账户抵扣 (赠送)
        /// </summary>
        public decimal PayAccountRechargeGive { get; set; }

        /// <summary>
        /// 充值账户Id
        /// </summary>
        public long? PayAccountRechargeId { get; set; }

        /// <summary>
        /// 经办人
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateOt { get; set; }

        /// <summary>
        /// 业绩归属人
        /// </summary>
        public string CommissionUser { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EmOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 关联订单(退货用)
        /// </summary>
        public long? UnionOrderId { get; set; }

        /// <summary>
        /// 关联订单(退货用)
        /// </summary>
        public string UnionOrderNo { get; set; }

        /// <summary>
        /// 关联订单(转课用)
        /// </summary>
        public string UnionTransferOrderIds { get; set; }

        /// <summary>
        /// 是否有退单记录
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsReturn { get; set; }

        /// <summary>
        /// 是否有转课记录
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsTransferCourse { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
