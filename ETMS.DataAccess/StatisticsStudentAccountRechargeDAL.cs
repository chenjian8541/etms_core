using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.ICache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IDataAccess;

namespace ETMS.DataAccess
{
    public class StatisticsStudentAccountRechargeDAL : DataAccessBase<StatisticsStudentAccountRechargeBucket>,
        IStatisticsStudentAccountRechargeDAL
    {
        public StatisticsStudentAccountRechargeDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StatisticsStudentAccountRechargeBucket> GetDb(params object[] keys)
        {
            var log = await this._dbWrapper.Find<EtStatisticsStudentAccountRecharge>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new StatisticsStudentAccountRechargeBucket()
            {
                StatisticsStudentAccountRecharge = log
            };
        }

        public async Task UpdateStatisticsStudentAccountRecharge()
        {
            var sql = $"SELECT COUNT(0) AS AccountCount,SUM(BalanceSum) AS BalanceSum,SUM(BalanceReal) AS BalanceReal,SUM(BalanceGive) AS BalanceGive,SUM(RechargeSum) AS RechargeSum,SUM(RechargeGiveSum) AS RechargeGiveSum FROM EtStudentAccountRecharge WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} ";
            var statisticsData = await this._dbWrapper.ExecuteObject<StatisticsStudentAccountRechargeView>(sql);
            var myData = statisticsData.FirstOrDefault();
            if (myData != null)
            {
                var hisData = await this._dbWrapper.Find<EtStatisticsStudentAccountRecharge>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
                if (hisData != null)
                {
                    hisData.AccountCount = myData.AccountCount;
                    hisData.BalanceSum = myData.BalanceSum;
                    hisData.BalanceReal = myData.BalanceReal;
                    hisData.BalanceGive = myData.BalanceGive;
                    hisData.RechargeSum = myData.RechargeSum;
                    hisData.RechargeGiveSum = myData.RechargeGiveSum;
                    await this._dbWrapper.Update(hisData);
                }
                else
                {
                    await this._dbWrapper.Insert(new EtStatisticsStudentAccountRecharge()
                    {
                        AccountCount = myData.AccountCount,
                        BalanceGive = myData.BalanceGive,
                        BalanceReal = myData.BalanceReal,
                        BalanceSum = myData.BalanceSum,
                        RechargeGiveSum = myData.RechargeGiveSum,
                        RechargeSum = myData.RechargeSum,
                        TenantId = _tenantId,
                        IsDeleted = EmIsDeleted.Normal
                    });
                }
            }
            await UpdateCache(_tenantId);
        }

        public async Task<EtStatisticsStudentAccountRecharge> GetStatisticsStudentAccountRecharge()
        {
            var bucket = await GetCache(_tenantId);
            return bucket?.StatisticsStudentAccountRecharge;
        }
    }
}
