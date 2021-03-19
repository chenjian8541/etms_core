using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCheckChoiceClassRequest : RequestBase
    {
        public long ClassTimesId { get; set; }

        public long StudentCheckOnLogId { get; set; }

        public override string Validate()
        {
            if (ClassTimesId == 0 || StudentCheckOnLogId == 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}