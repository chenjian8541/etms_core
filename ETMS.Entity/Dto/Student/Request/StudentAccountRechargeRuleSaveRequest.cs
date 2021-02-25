using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentAccountRechargeRuleSaveRequest : RequestBase
    {
        public string ImgUrlKey { get; set; }

        public string Explain { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Explain))
            {
                return "请填写规则说明";
            }
            return base.Validate();
        }
    }
}
