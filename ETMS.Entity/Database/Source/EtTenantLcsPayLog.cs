using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Source
{
    [Table("EtTenantLcsPayLog")]
    public class EtTenantLcsPayLog : Entity<long>
    {
        public int AgentId { get; set; }

        public long RelationId { get; set; }

        /// <summary>
        /// 聚合支付状态类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmAgtPayType"/>
        /// </summary>
        public int AgtPayType { get; set; }

        /// <summary>
        /// 订单类型
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogOrderType"/>
        /// </summary>
        public int OrderType { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单来源
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogOrderSource"/>
        /// </summary>
        public int OrderSource { get; set; }

        /// <summary>
        /// 订单描述
        /// </summary>
        public string OrderDesc { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantNo { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string TerminalId { get; set; }

        /// <summary>
        /// 商户类型
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsMerchanttype"/>
        /// </summary>
        public int MerchantType { get; set; }

        /// <summary>
        /// 终端流水号，填写商户系统的订单号（订单号）
        /// </summary>
        public string TerminalTrace { get; set; }

        /// <summary>
        /// 利楚唯一订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 支付方式
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayType"/>
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 金额，单位分
        /// </summary>
        public string TotalFee { get; set; }

        /// <summary>
        /// 金额，单位元
        /// </summary>
        public string TotalFeeDesc { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal TotalFeeValue { get; set; }

        /// <summary>
        /// 付款码
        /// </summary>
        public string AuthNo { get; set; }

        /// <summary>
        /// 公众号appid
        /// </summary>
        public string SubAppid { get; set; }

        /// <summary>
        /// 用户标识（微信openid，支付宝userid）
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 订单描述
        /// </summary>
        public string OrderBody { get; set; }

        /// <summary>
        /// 附加数据，原样返回
        /// </summary>
        public string Attach { get; set; }

        /// <summary>
        /// 利楚唯一退款单号
        /// </summary>
        public string OutRefundNo { get; set; }

        /// <summary>
        /// 退款金额，单位分
        /// </summary>
        public string RefundFee { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateOt { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        public DateTime? PayFinishOt { get; set; }

        /// <summary>
        /// 支付完成日期
        /// </summary>
        public DateTime? PayFinishDate { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime? RefundOt { get; set; }

        /// <summary>
        /// 退款日期
        /// </summary>
        public DateTime? RefundDate { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 状态
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogStatus"/>
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTenantLcsPayLogDataType"/>
        /// </summary>
        public byte DataType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
