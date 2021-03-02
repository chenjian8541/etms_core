using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentAccountRechargeGetByPhoneRequest : RequestBase
    {
        public string Phone { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Phone))
            {
                return "请输入手机号码";
            }
            return base.Validate();
        }
    }
}
