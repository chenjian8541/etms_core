using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysRoleDAL : BaseCacheDAL<SysRoleBucket>, ISysRoleDAL, IEtmsManage
    {
        public SysRoleDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysRoleBucket> GetDb(params object[] keys)
        {
            var roles = await this.FindList<SysRole>(p => p.IsDeleted == EmIsDeleted.Normal);
            return new SysRoleBucket()
            {
                SysRoles = roles
            };
        }

        public async Task<SysRole> GetRole(int id)
        {
            var roles = await GetRoles();
            return roles.FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<SysRole>> GetRoles()
        {
            var bucket = await GetCache();
            if (bucket == null || bucket.SysRoles == null)
            {
                return new List<SysRole>();
            }
            return bucket.SysRoles;
        }

        public async Task<bool> AddRole(SysRole entity)
        {
            await this.Insert(entity);
            await UpdateCache();
            return true;
        }

        public async Task<bool> EditRole(SysRole entity)
        {
            await this.Update(entity);
            await UpdateCache();
            return true;
        }

        public async Task<bool> IsCanNotDel(int roleId)
        {
            var log = await this.Find<SysAgent>(p => p.RoleId == roleId && p.IsDeleted == EmIsDeleted.Normal);
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
