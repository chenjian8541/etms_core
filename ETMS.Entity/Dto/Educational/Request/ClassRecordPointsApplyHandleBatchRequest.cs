using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassRecordPointsApplyHandleBatchRequest : RequestBase
    {
        public List<long> ClassRecordPointsApplyLogIds { get; set; }

        public byte NewHandleStatus { get; set; }

        public string HandleContent { get; set; }

        public override string Validate()
        {
            if (ClassRecordPointsApplyLogIds == null || ClassRecordPointsApplyLogIds.Count == 0)
            {
                return "请求数据格式错误";
            }
            if (NewHandleStatus <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
