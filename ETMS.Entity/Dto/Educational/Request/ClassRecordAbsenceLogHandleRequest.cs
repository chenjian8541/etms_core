using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassRecordAbsenceLogHandleRequest : RequestBase
    {
        public long ClassRecordAbsenceLogId { get; set; }

        public string HandleContent { get; set; }

        public override string Validate()
        {
            if (ClassRecordAbsenceLogId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
