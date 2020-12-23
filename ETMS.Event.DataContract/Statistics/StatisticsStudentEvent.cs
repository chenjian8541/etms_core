using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsStudentEvent : Event
    {
        public StatisticsStudentEvent(int tenantId) : base(tenantId)
        { }

        public EmStatisticsStudentType OpType { get; set; }

        public DateTime StatisticsDate { get; set; }
    }
}
