using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class SendToClassNoticeRequest : RequestBase
    {
        public List<long> StudentIds { get; set; }

        public override string Validate()
        {
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "请选择学员";
            }
            return string.Empty;
        }
    }
}
