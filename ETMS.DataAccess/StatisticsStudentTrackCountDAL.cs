using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StatisticsStudentTrackCountDAL : DataAccessBase, IStatisticsStudentTrackCountDAL
    {
        public StatisticsStudentTrackCountDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task AddStudentTrackCount(DateTime time, int addCount)
        {
            var hisData = await this._dbWrapper.Find<EtStatisticsStudentTrackCount>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == time);
            if (hisData != null)
            {
                hisData.TrackCount += addCount;
                await this._dbWrapper.Update(hisData);
            }
            else
            {
                await this._dbWrapper.Insert(new EtStatisticsStudentTrackCount()
                {
                    TrackCount = addCount,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = time,
                    TenantId = _tenantId
                });
            }
        }

        public async Task DeductionStudentTrackCount(DateTime time, int deductionCount)
        {
            var hisData = await this._dbWrapper.Find<EtStatisticsStudentTrackCount>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == time);
            if (hisData != null)
            {
                hisData.TrackCount -= deductionCount;
                await this._dbWrapper.Update(hisData);
            }
        }

        public async Task<List<EtStatisticsStudentTrackCount>> GetStatisticsStudentTrackCount(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsStudentTrackCount>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }
    }
}
