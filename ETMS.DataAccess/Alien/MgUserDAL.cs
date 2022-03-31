using ETMS.DataAccess.Alien.Core;
using ETMS.DataAccess.Core;
using ETMS.DataAccess.Core.Alien;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.Alien;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.Alien;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Alien
{
    public class MgUserDAL : DataAccessBaseAlien<MgUserBucket>, IMgUserDAL
    {
        public MgUserDAL(IDbWrapperAlien dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<MgUserBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToInt();
            var log = await _dbWrapper.Find<MgUser>(p => p.Id == id && p.HeadId == _headId && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new MgUserBucket()
            {
                MyMgUser = log
            };
        }

        public async Task<MgUser> ExistMgUserByPhone(string phone, long notId = 0)
        {
            if (notId > 0)
            {
                return await _dbWrapper.Find<MgUser>(p => p.Phone == phone && p.HeadId == _headId && p.IsDeleted == EmIsDeleted.Normal && p.Id != notId);
            }
            else
            {
                return await _dbWrapper.Find<MgUser>(p => p.Phone == phone && p.HeadId == _headId && p.IsDeleted == EmIsDeleted.Normal);
            }
        }

        public async Task<MgUser> GetUser(long id)
        {
            var bucket = await GetCache(_headId, id);
            return bucket?.MyMgUser;
        }

        public async Task<MgUser> GetUser(string phone)
        {
            return await _dbWrapper.Find<MgUser>(p => p.HeadId == _headId && p.Phone == phone && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> AddUser(MgUser entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_headId, entity.Id);
            return true;
        }

        public async Task<bool> EditUser(MgUser entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_headId, entity.Id);
            return true;
        }

        public async Task UpdateUserLastLoginTime(long userId, DateTime lastLoginTime)
        {
            await _dbWrapper.Execute($"UPDATE MgUser SET LastLoginOt = '{lastLoginTime.EtmsToString()}' WHERE Id = {userId}");
            await UpdateCache(_headId, userId);
        }

        public async Task<bool> DelUser(long id)
        {
            await _dbWrapper.Execute($"UPDATE MgUser SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id} ");
            RemoveCache(_headId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<MgUser>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<MgUser>("MgUser", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> ExistRole(long roleId)
        {
            var user = await _dbWrapper.Find<MgUser>(p => p.HeadId == _headId && p.MgRoleId == roleId && p.IsDeleted == EmIsDeleted.Normal);
            return user != null;
        }
    }
}
