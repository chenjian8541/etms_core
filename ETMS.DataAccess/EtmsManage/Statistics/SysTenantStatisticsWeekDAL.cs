using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.EtmsManage.Statistics;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage.Statistics
{
    public class SysTenantStatisticsWeekDAL : BaseCacheDAL<SysTenantStatisticsWeekBucket>, ISysTenantStatisticsWeekDAL, IEtmsManage
    {
        public SysTenantStatisticsWeekDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected async override Task<SysTenantStatisticsWeekBucket> GetDb(params object[] keys)
        {
            var sysTenantStatisticsWeek = await this.Find<SysTenantStatisticsWeek>(p => p.TenantId == keys[0].ToInt() && p.IsDeleted == EmIsDeleted.Normal);
            if (sysTenantStatisticsWeek == null)
            {
                return null;
            }
            return new SysTenantStatisticsWeekBucket()
            {
                SysTenantStatisticsWeek = sysTenantStatisticsWeek
            };
        }

        public async Task<SysTenantStatisticsWeek> GetSysTenantStatisticsWeek(int tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.SysTenantStatisticsWeek;
        }

        public async Task SaveSysTenantStatisticsWeek(SysTenantStatisticsWeek entity)
        {
            if (entity.Id > 0)
            {
                await this.Update(entity);
            }
            else
            {
                await this.Insert(entity);
            }
            await UpdateCache(entity.TenantId);
        }
    }
}
