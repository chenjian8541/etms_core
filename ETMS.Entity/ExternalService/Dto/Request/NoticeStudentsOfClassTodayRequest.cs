using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentsOfClassTodayRequest
    {
        public string ClassRoom { get; set; }

        public string ClassTimeDesc { get; set; }

        public List<NoticeStudentsOfClassTodayStudent> Students { get; set; }
    }

    public class NoticeStudentsOfClassTodayStudent
    {
        public string Phone { get; set; }

        public string StudentName { get; set; }

        public string CourseName { get; set; }
    }
}
