using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class NoticeStudentCourseNotEnoughBatchEvent : Event
    {
        public NoticeStudentCourseNotEnoughBatchEvent(int tenantId) : base(tenantId)
        { }
        public List<SendStudentCourseNotEnoughBatchItem> StudentCourses { get; set; }
    }
}
