using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class SysSafeSmsCheckRequest : RequestBase
    {
        public string SmsCode { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SmsCode))
            {
                return "校验码不能为空";
            }
            return base.Validate();
        }
    }
}
