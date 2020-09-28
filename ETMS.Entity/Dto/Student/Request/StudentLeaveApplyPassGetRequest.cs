using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentLeaveApplyPassGetRequest : RequestBase
    {
        public DateTime? Ot { get; set; }

        public override string Validate()
        {
            if (Ot == null)
            {
                return "请选择上课日期";
            }
            return string.Empty;
        }
    }
}
