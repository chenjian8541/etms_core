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
using ETMS.DataAccess.Alien.Core;

namespace ETMS.DataAccess.Alien
{
    public class MgRoleDAL : DataAccessBaseAlien<MgRoleBucket>, IMgRoleDAL
    {
        public MgRoleDAL(IDbWrapperAlien dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<MgRoleBucket> GetDb(params object[] keys)
        {
            var logs = await _dbWrapper.FindList<MgRole>(p => p.HeadId == _headId && p.IsDeleted == EmIsDeleted.Normal);
            return new MgRoleBucket()
            {
                MyMgRoles = logs
            };
        }

        public async Task<MgRole> GetRole(int id)
        {
            var roles = await GetRoles();
            if (roles == null || !roles.Any())
            {
                return null;
            }
            return roles.FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<MgRole>> GetRoles()
        {
            var bucket = await GetCache(_headId);
            return bucket?.MyMgRoles;
        }

        public async Task<bool> AddRole(MgRole entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_headId);
            return true;
        }

        public async Task<bool> EditRole(MgRole entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_headId);
            return true;
        }

        public async Task<bool> IsCanNotDel(int roleId)
        {
            var log = await _dbWrapper.Find<MgUser>(p => p.MgRoleId == roleId && p.IsDeleted == EmIsDeleted.Normal);
            return log != null;
        }

        public async Task<bool> DelRole(int id)
        {
            var role = await this.GetRole(id);
            role.IsDeleted = EmIsDeleted.Deleted;
            return await EditRole(role);
        }
    }
}
