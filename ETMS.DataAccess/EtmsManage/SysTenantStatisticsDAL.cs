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

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantStatisticsDAL : BaseCacheDAL<SysTenantStatisticsBucket>, ISysTenantStatisticsDAL, IEtmsManage
    {
        public SysTenantStatisticsDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysTenantStatisticsBucket> GetDb(params object[] keys)
        {
            var myTenantStatistics = await this.Find<SysTenantStatistics>(p => p.TenantId == keys[0].ToInt() && p.IsDeleted == EmIsDeleted.Normal);
            if (myTenantStatistics == null)
            {
                return null;
            }
            return new SysTenantStatisticsBucket()
            {
                SysTenantStatistics = myTenantStatistics
            };
        }

        public async Task<SysTenantStatistics> GetSysTenantStatistics(int tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.SysTenantStatistics;
        }

        public async Task SaveSysTenantStatistics(SysTenantStatistics entity)
        {
            if (entity.Id > 0)
            {
                await this.Update(entity);
            }
            else
            {
                await this.Insert(entity);
            }
        }
    }
}
