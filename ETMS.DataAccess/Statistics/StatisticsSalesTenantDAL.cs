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

namespace ETMS.DataAccess.Statistics
{
    public class StatisticsSalesTenantDAL : DataAccessBase, IStatisticsSalesTenantDAL
    {
        public StatisticsSalesTenantDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<EtStatisticsSalesTenant> GetStatisticsSalesTenant(DateTime ot)
        {
            return await this._dbWrapper.Find<EtStatisticsSalesTenant>(p => p.TenantId == _tenantId
            && p.IsDeleted == EmIsDeleted.Normal && p.Ot == ot);
        }

        public async Task SaveStatisticsSalesTenant(EtStatisticsSalesTenant entity)
        {
            if (entity.Id > 0)
            {
                await this._dbWrapper.Update(entity);
            }
            else
            {
                await this._dbWrapper.Insert(entity);
            }
        }

        public async Task<IEnumerable<StatisticsSalesTenantView>> GetStatisticsSalesTenant(DateTime startTime, DateTime endTime)
        {
            var sql = $"SELECT SUM(OrderNewCount) AS TotalOrderNewCount,SUM(OrderRenewCount) AS TotalOrderRenewCount,SUM(OrderBuyCount) AS TotalOrderBuyCount,SUM(OrderNewSum) AS TotalOrderNewSum,SUM(OrderRenewSum) AS TotalOrderRenewSum,SUM(OrderTransferOutSum) AS TotalOrderTransferOutSum,SUM(OrderReturnSum) AS TotalOrderReturnSum,SUM(OrderSum) AS TotalOrderSum FROM EtStatisticsSalesTenant WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTime.EtmsToDateString()}' AND Ot <= '{endTime.EtmsToDateString()}'";
            return await _dbWrapper.ExecuteObject<StatisticsSalesTenantView>(sql);
        }
    }
}