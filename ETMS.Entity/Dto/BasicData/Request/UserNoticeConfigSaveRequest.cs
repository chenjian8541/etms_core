using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class UserNoticeConfigSaveRequest : RequestBase
    {
        public bool StartClassWeChat { get; set; }

        public bool StartClassSms { get; set; }
    }
}
