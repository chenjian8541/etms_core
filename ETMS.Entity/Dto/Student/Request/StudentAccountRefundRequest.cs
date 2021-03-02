using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentAccountRefundRequest : RequestBase
    {
        public long StudentAccountRechargeId { get; set; }

        public decimal ReturnReal { get; set; }

        public decimal ReturnGive { get; set; }

        public decimal ReturnServiceCharge { get; set; }

        /// <summary>
        /// 支付类型 <see cref="ETMS.Entity.Enum.EmPayType"/>
        /// </summary>
        public byte PayType { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal PaySum { get; set; }

        public DateTime Ot { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (StudentAccountRechargeId <= 0)
            {
                return "数据不合法";
            }
            if (ReturnReal <= 0)
            {
                return "请输入实充余额退款";
            }
            return base.Validate();
        }
    }
}
