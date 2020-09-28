using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeClassCheckSignRequest
    {
        public string ClassTimeDesc { get; set; }

        public List<NoticeClassCheckSignStudent> Students { get; set; }
    }

    public class NoticeClassCheckSignStudent
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string CourseName { get; set; }

        public string StudentCheckStatusDesc { get; set; }

        public string DeClassTimesDesc { get; set; }

        public string SurplusClassTimesDesc { get; set; }
    }
}
