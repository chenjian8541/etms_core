using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess
{
    public class StatisticsFinanceIncomeDAL : DataAccessBase, IStatisticsFinanceIncomeDAL
    {
        public StatisticsFinanceIncomeDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task UpdateStatisticsFinanceIncome(DateTime date)
        {
            var otDesc = date.EtmsToDateString();
            var statisticsSql = $"SELECT [Type],[ProjectType],[PayType],SUM([Sum]) AS TotalSum FROM EtIncomeLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmIncomeLogStatus.Normal} AND  Ot = '{otDesc}' GROUP BY [Type],[ProjectType],[PayType]";
            var hisData = await _dbWrapper.ExecuteObject<StatisticsFinanceIncomeView>(statisticsSql);
            await _dbWrapper.ExecuteScalar($"DELETE EtStatisticsFinanceIncome WHERE TenantId = {_tenantId} AND Ot = '{otDesc}'");
            var newEtStatisticsFinanceIncome = new List<EtStatisticsFinanceIncome>();
            foreach (var p in hisData)
            {
                newEtStatisticsFinanceIncome.Add(new EtStatisticsFinanceIncome()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = date,
                    PayType = p.PayType,
                    ProjectType = p.ProjectType,
                    TenantId = _tenantId,
                    TotalSum = p.TotalSum,
                    Type = p.Type
                });
            }
            if (newEtStatisticsFinanceIncome.Any())
            {
                await _dbWrapper.InsertRangeAsync(newEtStatisticsFinanceIncome);
            }
        }

        public async Task<List<EtStatisticsFinanceIncome>> GetStatisticsFinanceIncome(DateTime startTime, DateTime endTime, byte type)
        {
            return await _dbWrapper.FindList<EtStatisticsFinanceIncome>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime && p.Type == type);
        }
    }
}
