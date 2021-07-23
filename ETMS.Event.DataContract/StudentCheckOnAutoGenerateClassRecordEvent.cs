using System;

namespace ETMS.Event.DataContract
{
    public class StudentCheckOnAutoGenerateClassRecordEvent : Event
    {
        public StudentCheckOnAutoGenerateClassRecordEvent(int tenantId) : base(tenantId)
        { }

        public DateTime AnalyzeDate { get; set; }
    }
}
