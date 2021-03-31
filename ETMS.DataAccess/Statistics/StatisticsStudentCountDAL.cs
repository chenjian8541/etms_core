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
    public class StatisticsStudentCountDAL : DataAccessBase, IStatisticsStudentCountDAL
    {
        public StatisticsStudentCountDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task AddStudentCount(DateTime time, int addCount)
        {
            var hisData = await this._dbWrapper.Find<EtStatisticsStudentCount>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == time);
            if (hisData != null)
            {
                hisData.AddStudentCount += addCount;
                await this._dbWrapper.Update(hisData);
            }
            else
            {
                await this._dbWrapper.Insert(new EtStatisticsStudentCount()
                {
                    AddStudentCount = addCount,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = time,
                    TenantId = _tenantId
                });
            }
        }

        public async Task DeductionStudentCount(DateTime time, int deductionCount)
        {
            var hisData = await this._dbWrapper.Find<EtStatisticsStudentCount>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot == time);
            if (hisData != null)
            {
                hisData.AddStudentCount -= deductionCount;
                await this._dbWrapper.Update(hisData);
            }
        }

        public async Task<List<EtStatisticsStudentCount>> GetStatisticsStudentCount(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtStatisticsStudentCount>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }
    }
}
