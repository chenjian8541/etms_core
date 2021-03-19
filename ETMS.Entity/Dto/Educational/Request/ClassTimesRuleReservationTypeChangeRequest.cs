using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleReservationTypeChangeRequest : RequestBase
    {
        public long ClassRuleId { get; set; }

        public override string Validate()
        {
            if (ClassRuleId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}

