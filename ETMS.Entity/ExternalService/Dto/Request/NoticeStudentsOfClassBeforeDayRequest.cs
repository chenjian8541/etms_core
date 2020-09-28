using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentsOfClassBeforeDayRequest
    {
        public string ClassRoom { get; set; }

        public string ClassTimeDesc { get; set; }

        public List<NoticeStudentsOfClassBeforeDayStudent> Students { get; set; }
    }

    public class NoticeStudentsOfClassBeforeDayStudent
    {
        public string Phone { get; set; }

        public string StudentName { get; set; }

        public string CourseName { get; set; }
    }
}
