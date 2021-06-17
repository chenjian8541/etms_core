using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class TryCalssApplyRequest : Open2Base
    {
        public string Phone { get; set; }

        public string StuNo { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Phone))
            {
                return "请填写手机号码";
            }
            return base.Validate();
        }
    }
}
