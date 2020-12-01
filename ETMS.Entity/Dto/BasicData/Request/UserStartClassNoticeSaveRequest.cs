using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class UserStartClassNoticeSaveRequest : RequestBase
    {
        public int StartClassBeforeMinuteValue { get; set; }
    }
}
