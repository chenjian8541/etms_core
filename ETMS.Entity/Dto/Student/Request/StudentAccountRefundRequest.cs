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

        public DateTime Ot { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (StudentAccountRechargeId <= 0)
            {
                return "数据格式错误";
            }
            if (ReturnReal <= 0)
            {
                return "请输入实充余额退款";
            }
            if (ReturnServiceCharge > ReturnReal)
            {
                return "手续费不能大于实充余额退款";
            }
            return base.Validate();
        }
    }
}
