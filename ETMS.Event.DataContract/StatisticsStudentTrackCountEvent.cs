using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsStudentTrackCountEvent : Event
    {
        public StatisticsStudentTrackCountEvent(int tenantId) : base(tenantId)
        { }

        public StatisticsStudentTrackCountOpType OpType { get; set; }

        public DateTime Time { get; set; }

        public int ChangeCount { get; set; }
    }

    public enum StatisticsStudentTrackCountOpType
    {
        Add = 0,

        Deduction = 1
    }
}
