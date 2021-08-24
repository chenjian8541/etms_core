using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.TeacherSalary;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.TeacherSalary
{
    public class TeacherSalaryMonthStatisticsDAL : DataAccessBase<TeacherSalaryMonthStatisticsBucket>, ITeacherSalaryMonthStatisticsDAL
    {
        public TeacherSalaryMonthStatisticsDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TeacherSalaryMonthStatisticsBucket> GetDb(params object[] keys)
        {
            var userId = keys[1].ToLong();
            var date = Convert.ToDateTime(keys[2]);
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1);
            var totalPayItemSum = 0M;
            var obj = await _dbWrapper.ExecuteScalar($"SELECT SUM(PayItemSum) FROM EtTeacherSalaryPayrollUser WHERE TenantId = {_tenantId} AND UserId = {userId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmTeacherSalaryPayrollStatus.IsOK} AND PayDate >= '{startDate.EtmsToDateString()}' AND PayDate < '{endDate.EtmsToDateString()}'");
            if (obj != null)
            {
                totalPayItemSum = Convert.ToDecimal(obj);
            }
            return new TeacherSalaryMonthStatisticsBucket()
            {
                TotalPayItemSum = totalPayItemSum
            };
        }

        public async Task<decimal> GetTeacherSalaryMonthStatistics(long userId, int year, int month)
        {
            var bucket = await GetCache(_tenantId, userId, new DateTime(year, month, 1));
            if (bucket == null)
            {
                return 0;
            }
            return bucket.TotalPayItemSum;
        }

        public async Task UpdateTeacherSalaryMonthStatistics(long userId, int year, int month)
        {
            await UpdateCache(_tenantId, userId, new DateTime(year, month, 1));
        }
    }
}
