using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.View.Persistence;

namespace ETMS.DataAccess
{
    public class StatisticsStudentDAL : DataAccessBase<StatisticsStudentBucket>, IStatisticsStudentDAL
    {
        public StatisticsStudentDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StatisticsStudentBucket> GetDb(params object[] keys)
        {
            var statisticsStudents = await _dbWrapper.FindList<EtStatisticsStudent>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            return new StatisticsStudentBucket()
            {
                StatisticsStudents = statisticsStudents
            };
        }

        public async Task SaveStatisticsStudent(int type, string contentData)
        {
            var hisData = await _dbWrapper.Find<EtStatisticsStudent>(p => p.Type == type && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            if (hisData != null)
            {
                hisData.ContentData = contentData;
                await _dbWrapper.Update(hisData);
            }
            else
            {
                await _dbWrapper.Insert(new EtStatisticsStudent()
                {
                    ContentData = contentData,
                    IsDeleted = EmIsDeleted.Normal,
                    TenantId = _tenantId,
                    Type = type
                });
            }
            await UpdateCache(_tenantId);
        }

        public async Task<List<EtStatisticsStudent>> GetStatisticsStudent()
        {
            var bucket = await GetCache(_tenantId);
            return bucket?.StatisticsStudents;
        }

        public async Task<EtStatisticsStudent> GetStatisticsStudent(int type)
        {
            var statisticsStudents = await GetStatisticsStudent();
            if (statisticsStudents != null && statisticsStudents.Any())
            {
                return statisticsStudents.FirstOrDefault(p => p.Type == type);
            }
            return null;
        }

        public async Task<IEnumerable<StatisticsStudentType>> GetStatisticsStudentType()
        {
            var sql = $"SELECT StudentType,COUNT(0) AS TotalCount FROM EtStudent WHERE TenantId  = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} GROUP BY StudentType";
            return await _dbWrapper.ExecuteObject<StatisticsStudentType>(sql);
        }
    }
}
