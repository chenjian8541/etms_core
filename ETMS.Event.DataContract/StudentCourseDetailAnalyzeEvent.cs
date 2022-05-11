using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StudentCourseDetailAnalyzeEvent : Event
    {
        public StudentCourseDetailAnalyzeEvent(int tenantId) : base(tenantId)
        { }

        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public bool IsClassOfOneAutoOver { get; set; }

        public bool IsSendNoticeStudent { get; set; }

        public bool IsNeedCheckCourseIsNotEnough { get; set; }

        public bool IsJobExecute { get; set; }
    }
}
