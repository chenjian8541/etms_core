using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsStudentCountEvent : Event
    {
        public StatisticsStudentCountEvent(int tenantId) : base(tenantId)
        { }

        public StatisticsStudentOpType OpType { get; set; }

        public DateTime Time { get; set; }

        public int ChangeCount { get; set; }
    }

    public enum StatisticsStudentOpType
    {
        Add = 0,

        Deduction = 1
    }
}
