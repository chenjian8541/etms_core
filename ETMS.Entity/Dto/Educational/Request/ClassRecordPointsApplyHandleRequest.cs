using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassRecordPointsApplyHandleRequest : RequestBase
    {
        public long ClassRecordPointsApplyLogId { get; set; }

        public byte NewHandleStatus { get; set; }

        public string HandleContent { get; set; }

        public override string Validate()
        {
            if (ClassRecordPointsApplyLogId <= 0)
            {
                return "请求数据不合法";
            }
            if (NewHandleStatus <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}
