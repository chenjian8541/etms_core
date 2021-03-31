using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StatisticsSalesUserDAL : DataAccessBase, IStatisticsSalesUserDAL
    {
        public StatisticsSalesUserDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<EtStatisticsSalesUser> GetStatisticsSalesUser(long userId, DateTime ot)
        {
            return await _dbWrapper.Find<EtStatisticsSalesUser>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal &&
            p.UserId == userId && p.Ot == ot);
        }

        public async Task SaveStatisticsSalesUser(EtStatisticsSalesUser entity)
        {
            if (entity.Id > 0)
            {
                await _dbWrapper.Update(entity);
            }
            else
            {
                await _dbWrapper.Insert(entity);
            }
        }

        public async Task<IEnumerable<StatisticsSalesUserView>> GetStatisticsSalesUser(DateTime startTime, DateTime endTime)
        {
            var sql = $"SELECT UserId,SUM(OrderNewCount) AS TotalOrderNewCount,SUM(OrderRenewCount) AS TotalOrderRenewCount,SUM(OrderBuyCount) AS TotalOrderBuyCount,SUM(OrderNewSum) AS TotalOrderNewSum,SUM(OrderRenewSum) AS TotalOrderRenewSum,SUM(OrderTransferOutSum) AS TotalOrderTransferOutSum,SUM(OrderReturnSum) AS TotalOrderReturnSum,SUM(OrderSum) AS TotalOrderSum FROM EtStatisticsSalesUser WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}' GROUP BY UserId";
            return await _dbWrapper.ExecuteObject<StatisticsSalesUserView>(sql);
        }
    }
}
