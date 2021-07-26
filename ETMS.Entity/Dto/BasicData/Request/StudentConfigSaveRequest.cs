using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentConfigSaveRequest : RequestBase
    {
        public string InitialPassword { get; set; }

        public override string Validate()
        {
            if (InitialPassword.Length > 20)
            {
                return "密码长度不能超过20";
            }
            return base.Validate();
        }
    }
}

