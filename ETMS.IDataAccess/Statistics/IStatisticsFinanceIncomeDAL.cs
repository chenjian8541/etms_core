using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsFinanceIncomeDAL: IBaseDAL
    {
        Task UpdateStatisticsFinanceIncome(DateTime date);

        Task UpdateStatisticsFinanceIncomeMonth(DateTime date);

        Task<List<EtStatisticsFinanceIncome>> GetStatisticsFinanceIncome(DateTime startTime, DateTime endTime, byte type);
    }
}
