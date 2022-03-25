using ETMS.DataAccess.Core;
using ETMS.DataAccess.Core.Alien;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.Alien;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.Alien;
using ETMS.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess.Alien
{
    public class MgTenantsDAL : DataAccessBaseAlien<MgTenantsBucket>, IMgTenantsDAL
    {
        public MgTenantsDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<MgTenantsBucket> GetDb(params object[] keys)
        {
            var logs = await _dbWrapper.FindList<MgTenants>(p => p.HeadId == _headId && p.IsDeleted == EmIsDeleted.Normal);
            return new MgTenantsBucket()
            {
                TenantsList = logs
            };
        }

        public async Task<bool> ExistTenant(int tenantId)
        {
            var allMgTenants = await GetMgTenants();
            if (allMgTenants == null || !allMgTenants.Any())
            {
                return false;
            }
            return allMgTenants.Exists(j => j.TenantId == tenantId);
        }

        public async Task AddMgTenant(MgTenants entity)
        {
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_headId);
        }

        public async Task<List<MgTenants>> GetMgTenants()
        {
            var bucket = await GetCache(_headId);
            return bucket?.TenantsList;
        }

        public async Task DelMgTenant(int tenantId)
        {
            await _dbWrapper.Execute($"UPDATE [MgTenants] SET IsDeleted = {EmIsDeleted.Deleted} WHERE HeadId = {_headId} AND TenantId = {tenantId}");
            await UpdateCache(_headId);
        }
    }
}
