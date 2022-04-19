using ETMS.IDataAccess.EtmsManage.Statistics;
using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage.Statistics
{
    public class SysTenantStatisticsMonthDAL : BaseCacheDAL<SysTenantStatisticsMonthBucket>, ISysTenantStatisticsMonthDAL, IEtmsManage
    {
        public SysTenantStatisticsMonthDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected async override Task<SysTenantStatisticsMonthBucket> GetDb(params object[] keys)
        {
            var sysTenantStatisticsMonth = await this.Find<SysTenantStatisticsMonth>(p => p.TenantId == keys[0].ToInt() && p.IsDeleted == EmIsDeleted.Normal);
            if (sysTenantStatisticsMonth == null)
            {
                return null;
            }
            return new SysTenantStatisticsMonthBucket()
            {
                SysTenantStatisticsMonth = sysTenantStatisticsMonth
            };
        }

        public async Task<SysTenantStatisticsMonth> GetSysTenantStatisticsMonth(int tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.SysTenantStatisticsMonth;
        }

        public async Task SaveSysTenantStatisticsMonth(SysTenantStatisticsMonth entity)
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
