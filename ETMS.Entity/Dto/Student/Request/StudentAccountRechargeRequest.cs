using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentAccountRechargeRequest : RequestBase
    {
        public long StudentAccountRechargeId { get; set; }

        public decimal RechargeReal { get; set; }

        public decimal RechargeGive { get; set; }

        public DateTime Ot { get; set; }

        public List<long> CommissionUser { get; set; }

        public string Remark { get; set; }

        public int TotalPoints { get; set; }

        public StudentAccountRechargePayInfo PayInfo { get; set; }

        public override string Validate()
        {
            if (StudentAccountRechargeId <= 0)
            {
                return "数据不合法";
            }
            if (RechargeReal <= 0)
            {
                return "实充金额必须大于0";
            }
            if (PayInfo == null)
            {
                return "请输入支付信息";
            }
            if (PayInfo.PaySum != RechargeReal)
            {
                return "实收金额需要等于应收金额";
            }
            if (!Ot.IsEffectiveDate())
            {
                return "请选择正确的经办日期";
            }
            return base.Validate();
        }
    }

    public class StudentAccountRechargePayInfo : IValidate
    {
        public decimal PayWechat { get; set; }

        public decimal PayAlipay { get; set; }

        public decimal PayCash { get; set; }

        public decimal PayBank { get; set; }

        public decimal PayPos { get; set; }

        public decimal PaySum
        {
            get
            {
                return PayWechat + PayAlipay + PayCash + PayBank + PayPos;
            }
        }

        public string Validate()
        {
            return string.Empty;
        }
    }
}
