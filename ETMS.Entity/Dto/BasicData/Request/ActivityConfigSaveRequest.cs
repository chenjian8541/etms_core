using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class ActivityConfigSaveRequest: RequestBase
    {
        public bool IsAutoRefund { get; set; }

        public string PayTp { get; set; }
    }
}
