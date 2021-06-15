using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StatisticsStudentCountDAL : DataAccessBase, IStatisticsStudentCountDAL
    {
        public StatisticsStudentCountDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task UpdateStatisticsStudentCountDay(DateTime time)
        {
            var startTimeDesc = time.Date.EtmsToDateString();
            var endTimeDesc = time.AddDays(1).Date.EtmsToDateString();
            var sql = $"SELECT COUNT(0) from EtStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTimeDesc}' AND  Ot < '{endTimeDesc}' ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            var totalCount = 0;
            if (obj != null)
            {
                totalCount = obj.ToInt();
            }
            var hisData = await this._dbWrapper.Find<EtStatisticsStudentCount>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == time);
            if (hisData != null)
            {
                hisData.AddStudentCount = totalCount;
                await _dbWrapper.Update(hisData);
            }
            else
            {
                await this._dbWrapper.Insert(new EtStatisticsStudentCount()
                {
                    AddStudentCount = totalCount,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = time,
                    TenantId = _tenantId
                });
            }
        }

        public async Task<List<EtStatisticsStudentCount>> GetStatisticsStudentCount(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsStudentCount>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<Tuple<IEnumerable<EtStatisticsStudentCount>, int>> GetStatisticsStudentCountPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsStudentCount>("EtStatisticsStudentCount", "*", request.PageSize, request.PageCurrent, "[Ot] DESC", request.ToString());
        }

        public async Task UpdateStatisticsStudentCountMonth(DateTime time)
        {
            var firstDate = new DateTime(time.Year, time.Month, 1);
            var startTimeDesc = firstDate.EtmsToDateString();
            var endTimeDesc = firstDate.AddMonths(1).EtmsToDateString();
            var sql = $"SELECT COUNT(0) from EtStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Ot >= '{startTimeDesc}' AND  Ot < '{endTimeDesc}' ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            var totalCount = 0;
            if (obj != null)
            {
                totalCount = obj.ToInt();
            }
            var log = await _dbWrapper.Find<EtStatisticsStudentCountMonth>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == firstDate);
            if (log != null)
            {
                log.AddStudentCount = totalCount;
                await _dbWrapper.Update(log);
            }
            else
            {
                await _dbWrapper.Insert(new EtStatisticsStudentCountMonth()
                {
                    AddStudentCount = totalCount,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = firstDate,
                    Month = firstDate.Month,
                    Year = firstDate.Year,
                    TenantId = _tenantId
                });
            }
        }

        public async Task<List<EtStatisticsStudentCountMonth>> GetStatisticsStudentCountMonth(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsStudentCountMonth>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<Tuple<IEnumerable<EtStatisticsStudentCountMonth>, int>> GetStatisticsStudentCountMonthPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStatisticsStudentCountMonth>("EtStatisticsStudentCountMonth", "*", request.PageSize, request.PageCurrent, "[Ot] DESC", request.ToString());
        }
    }
}
