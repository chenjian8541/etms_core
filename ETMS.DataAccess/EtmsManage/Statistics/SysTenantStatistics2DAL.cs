using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.IDataAccess.EtmsManage.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Utility;
using ETMS.ICache;

namespace ETMS.DataAccess.EtmsManage.Statistics
{
    public class SysTenantStatistics2DAL : BaseCacheDAL<SysTenantStatistics2Bucket>, ISysTenantStatistics2DAL, IEtmsManage
    {
        public SysTenantStatistics2DAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected async override Task<SysTenantStatistics2Bucket> GetDb(params object[] keys)
        {
            var myTenantStatistics2 = await this.Find<SysTenantStatistics2>(p => p.TenantId == keys[0].ToInt() && p.IsDeleted == EmIsDeleted.Normal);
            if (myTenantStatistics2 == null)
            {
                return null;
            }
            return new SysTenantStatistics2Bucket()
            {
                SysTenantStatistics2 = myTenantStatistics2
            };
        }

        public async Task<SysTenantStatistics2> GetSysTenantStatistics(int tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.SysTenantStatistics2;
        }

        public async Task SaveSysTenantStatistics2(SysTenantStatistics2 entity)
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
