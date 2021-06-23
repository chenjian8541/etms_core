using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class FinanceIncomeBLL: IFinanceIncomeBLL
    {
        private readonly IStatisticsFinanceIncomeDAL _statisticsFinanceIncomeDAL;

        public FinanceIncomeBLL(IStatisticsFinanceIncomeDAL statisticsFinanceIncomeDAL)
        {
            this._statisticsFinanceIncomeDAL = statisticsFinanceIncomeDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsFinanceIncomeDAL);
        }

        public async Task StatisticsFinanceIncomeMonthConsumerEvent(StatisticsFinanceIncomeMonthEvent request)
        {
            await this._statisticsFinanceIncomeDAL.UpdateStatisticsFinanceIncomeMonth(request.Time);
        }
    }
}
