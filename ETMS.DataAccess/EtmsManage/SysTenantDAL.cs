using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Config;
using ETMS.IDataAccess.EtmsManage;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantDAL : BaseCacheDAL<SysTenantBucket>, ISysTenantDAL, IEtmsManage
    {
        private readonly ITenantConfigWrapper _tenantConfigWrapper;
        public SysTenantDAL(ICacheProvider cacheProvider, ITenantConfigWrapper tenantConfigWrapper) : base(cacheProvider)
        {
            this._tenantConfigWrapper = tenantConfigWrapper;
        }
        protected override async Task<SysTenantBucket> GetDb(params object[] keys)
        {
            var sysTenants = await this.FindList<SysTenant>(p => p.IsDeleted == EmIsDeleted.Normal);
            return new SysTenantBucket()
            {
                SysTenants = sysTenants
            };
        }

        public async Task<SysTenant> GetTenant(string tenantCode)
        {
            var sysTenantBucket = await base.GetCache();
            return sysTenantBucket.SysTenants.FirstOrDefault(p => p.TenantCode == tenantCode);
        }

        public async Task<SysTenant> GetTenant(int id)
        {
            var sysTenantBucket = await base.GetCache();
            return sysTenantBucket.SysTenants.FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<SysTenant>> GetTenants()
        {
            var sysTenantBucket = await base.GetCache();
            return sysTenantBucket.SysTenants;
        }

        public async Task<int> AddTenant(SysTenant sysTenant)
        {
            await this.Insert(sysTenant);
            await UpdateCache(new SysTenantBucket().GetKeyFormat());
            await _tenantConfigWrapper.TenantConnectionUpdate();
            return sysTenant.Id;
        }
    }
}
