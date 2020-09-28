using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp.Request
{
    public class TryCalssNoticeTrackUserRequest
    {
        public long StudentId { get; set; }

        public DateTime ClassOt { get; set; }

        public byte StudentCheckStatus { get; set; }
    }
}
