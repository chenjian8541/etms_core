using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.EtmsManage.Common;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysUserDAL : BaseCacheDAL<SysUserBucket>, ISysUserDAL, IEtmsManage
    {
        public SysUserDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysUserBucket> GetDb(params object[] keys)
        {
            var id = keys[0].ToInt();
            var user = await this.Find<SysUser>(p => p.IsDeleted == EmIsDeleted.Normal && p.Id == id);
            if (user == null)
            {
                return null;
            }
            return new SysUserBucket()
            {
                SysUser = user
            };
        }

        public async Task<SysUser> ExistSysUserByPhone(int agentId, string phone, long notId = 0)
        {
            if (notId > 0)
            {
                return await this.Find<SysUser>(p => p.Phone == phone && p.AgentId == agentId && p.IsDeleted == EmIsDeleted.Normal && p.Id != notId);
            }
            else
            {
                return await this.Find<SysUser>(p => p.Phone == phone && p.AgentId == agentId && p.IsDeleted == EmIsDeleted.Normal);
            }
        }

        public async Task<SysUser> GetUser(long id)
        {
            var bucket = await GetCache(id);
            return bucket?.SysUser;
        }

        public async Task<bool> AddUser(SysUser entity)
        {
            await this.Insert(entity);
            await UpdateCache(entity.Id);
            return true;
        }

        public async Task UpdateUserLastLoginTime(long userId, DateTime lastLoginTime)
        {
            await this.Execute($"UPDATE SysUser SET LastLoginOt = '{lastLoginTime.EtmsToString()}' WHERE Id = {userId}");
            await UpdateCache(userId);
        }

        public async Task<bool> EditUser(SysUser entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.Id);
            return true;
        }

        public async Task<bool> DelUser(long id)
        {
            await this.Execute($"UPDATE SysUser SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id} ");
            RemoveCache(id);
            return true;
        }

        public async Task<Tuple<IEnumerable<SysUser>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysUser>("SysUser", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
