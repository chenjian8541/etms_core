using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentAccountRechargeGetRequest : RequestBase
    {
        public long Id { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "请求不合法";
            }
            return base.Validate();
        }
    }
}
