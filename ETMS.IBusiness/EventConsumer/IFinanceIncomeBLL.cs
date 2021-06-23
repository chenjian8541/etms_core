using ETMS.Event.DataContract.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EventConsumer
{
    public interface IFinanceIncomeBLL : IBaseBLL
    {
        Task StatisticsFinanceIncomeMonthConsumerEvent(StatisticsFinanceIncomeMonthEvent request);
    }
}
