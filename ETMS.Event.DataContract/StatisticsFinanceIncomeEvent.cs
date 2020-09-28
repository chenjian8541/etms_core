using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsFinanceIncomeEvent : Event
    {
        public StatisticsFinanceIncomeEvent(int tenantId) : base(tenantId)
        { }

        public DateTime StatisticsDate { get; set; }
    }
}

