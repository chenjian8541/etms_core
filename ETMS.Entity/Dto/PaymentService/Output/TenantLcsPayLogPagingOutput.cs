using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class TenantLcsPayLogPagingOutput
    {

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 订单类型
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogOrderType"/>
        /// </summary>
        public int OrderType { get; set; }

        public string OrderTypeDesc { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单来源
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogOrderSource"/>
        /// </summary>
        public int OrderSource { get; set; }

        public string OrderSourceDesc { get; set; }

        /// <summary>
        /// 订单描述
        /// </summary>
        public string OrderDesc { get; set; }

        /// <summary>
        /// 利楚唯一订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 支付方式
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayType"/>
        /// </summary>
        public string PayType { get; set; }

        public string PayTypeDesc { get; set; }

        /// <summary>
        /// 金额，单位元
        /// </summary>
        public string TotalFeeDesc { get; set; }

        /// <summary>
        /// 利楚唯一退款单号
        /// </summary>
        public string OutRefundNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateOt { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        public DateTime? PayFinishOt { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime? RefundOt { get; set; }

        /// <summary>
        /// 状态
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogStatus"/>
        /// </summary>
        public int Status { get; set; }

        public string StatusDesc { get; set; }

        public bool IsCanRefund { get; set; }
    }
}
