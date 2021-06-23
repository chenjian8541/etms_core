using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Event.DataContract.Statistics
{
    public class StatisticsFinanceIncomeMonthEvent : Event
    {
        public StatisticsFinanceIncomeMonthEvent(int tenantId) : base(tenantId)
        { }

        public DateTime Time { get; set; }
    }
}

