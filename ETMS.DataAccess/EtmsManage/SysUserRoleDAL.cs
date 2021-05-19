using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Config;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using ETMS.Utility;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysUserRoleDAL : BaseCacheDAL<SysUserRoleBucket>, ISysUserRoleDAL, IEtmsManage
    {
        public SysUserRoleDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysUserRoleBucket> GetDb(params object[] keys)
        {
            var id = keys[0].ToInt();
            var myRole = await this.Find<SysUserRole>(p => p.IsDeleted == EmIsDeleted.Normal && p.Id == id);
            if (myRole == null)
            {
                return null;
            }
            return new SysUserRoleBucket()
            {
                SysUserRole = myRole
            };
        }

        public async Task<SysUserRole> GetRole(int id)
        {
            var bucket = await GetCache(id);
            return bucket?.SysUserRole;
        }

        public async Task<List<SysUserRole>> GetMyRoles(int agentId)
        {
            return await this.FindList<SysUserRole>(p => p.IsDeleted == EmIsDeleted.Normal && p.AgentId == agentId);
        }

        public async Task<bool> AddRole(SysUserRole entity)
        {
            await this.Insert(entity);
            await UpdateCache(entity.Id);
            return true;
        }

        public async Task<bool> EditRole(SysUserRole entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.Id);
            return true;
        }

        public async Task<bool> IsCanNotDel(int roleId)
        {
            var log = await this.Find<SysUser>(p => p.UserRoleId == roleId && p.IsDeleted == EmIsDeleted.Normal);
            return log != null;
        }

        public async Task<bool> DelRole(int id)
        {
            await this.Execute($"UPDATE SysUserRole SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id} ");
            this.RemoveCache(id);
            return true;
        }

        public async Task<Tuple<IEnumerable<SysUserRole>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysUserRole>("SysUserRole", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
