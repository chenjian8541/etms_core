using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class NoticeStudentLeaveApplyRequest
    {
        public string TimeDesc { get; set; }

        public List<NoticeStudentLeaveApplyStudent> Students { get; set; }
    }

    public class NoticeStudentLeaveApplyStudent
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string HandleStatusDesc { get; set; }
    }
}
