using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentAccountRechargeBinderRemoveRequest : RequestBase
    {
        public long StudentAccountRechargeId { get; set; }

        public long StudentAccountRechargeBinderId { get; set; }

        public long StudentId { get; set; }

        public override string Validate()
        {
            if (StudentAccountRechargeId <= 0 || StudentAccountRechargeBinderId <= 0 || StudentId <= 0)
            {
                return "数据格式错误";
            }
            return base.Validate();
        }
    }
}