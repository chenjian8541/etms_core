using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class OrderPaymentRequest : RequestBase
    {
        public long OrderId { get; set; }

        public decimal PayWechat { get; set; }

        public decimal PayAlipay { get; set; }

        public decimal PayCash { get; set; }

        public decimal PayBank { get; set; }

        public decimal PayPos { get; set; }

        public decimal PayOther { get; set; }

        public decimal PayLcsBarcodePay { get; set; }

        public string Remark { get; set; }

        public DateTime PayOt { get; set; }

        public decimal PayAccountRechargeReal { get; set; }

        public decimal PayAccountRechargeGive { get; set; }

        public long? PayAccountRechargeId { get; set; }

        public override string Validate()
        {
            if (OrderId <= 0)
            {
                return "请求数据格式错误";
            }
            var payPayAccount = 0M;
            if (this.PayAccountRechargeId != null && (this.PayAccountRechargeReal > 0 || this.PayAccountRechargeGive > 0))
            {
                payPayAccount = this.PayAccountRechargeReal + this.PayAccountRechargeGive;
            }
            if ((PayWechat + PayAlipay + PayCash + PayBank + PayPos + PayOther + PayLcsBarcodePay + payPayAccount) <= 0)
            {
                return "请输入支付金额";
            }
            return string.Empty;
        }
    }
}