using ETMS.Authority;
using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.User.Output;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Config.Menu;

namespace ETMS.DataAccess
{
    public class RoleDAL : DataAccessBase<RoleBucket>, IRoleDAL
    {
        public RoleDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<RoleBucket> GetDb(params object[] keys)
        {
            var role = await _dbWrapper.Find<EtRole>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            return new RoleBucket()
            {
                Role = role
            };
        }

        public async Task<EtRole> GetRole(long id)
        {
            var roleBucket = await base.GetCache(_tenantId, id);
            return roleBucket?.Role;
        }

        public async Task<List<EtRole>> GetRole()
        {
            return await _dbWrapper.FindList<EtRole>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> AddRole(EtRole role)
        {
            return await _dbWrapper.Insert(role, async () => { await UpdateCache(_tenantId, role.Id); });
        }

        public async Task<bool> EditRole(EtRole role)
        {
            return await _dbWrapper.Update(role, async () => { await UpdateCache(_tenantId, role.Id); });
        }

        public async Task<bool> DelRole(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtRole SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id}");
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<RoleBucket> GetRoleBucket(long id)
        {
            return await GetCache(_tenantId, id);
        }
    }
}
