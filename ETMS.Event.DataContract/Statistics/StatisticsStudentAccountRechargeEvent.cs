using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract.Statistics
{
    public class StatisticsStudentAccountRechargeEvent : Event
    {
        public StatisticsStudentAccountRechargeEvent(int tenantId) : base(tenantId)
        { }
    }
}

