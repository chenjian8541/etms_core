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
    public class MgHeadDAL : DataAccessBaseAlien<MgHeadBucket>, IMgHeadDAL
    {
        public MgHeadDAL(IDbWrapperAlien dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<MgHeadBucket> GetDb(params object[] keys)
        {
            var id = keys[0].ToInt();
            var log = await _dbWrapper.Find<MgHead>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
            return new MgHeadBucket()
            {
                MyMgHead = log
            };
        }

        public async Task<bool> ExistHeadCode(string headCode, int id = 0)
        {
            var log = await _dbWrapper.Find<MgHead>(p => p.IsDeleted == EmIsDeleted.Normal && p.HeadCode == headCode && p.Id != id);
            return log != null;
        }

        public async Task AddMgHead(MgHead entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(entity.Id);
        }

        public async Task EditMgHead(MgHead entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(entity.Id);
        }

        public async Task<MgHead> GetMgHead(int id)
        {
            var bucket = await GetCache(id);
            return bucket?.MyMgHead;
        }

        public async Task<MgHead> GetMgHead(string headCode)
        {
            return await _dbWrapper.Find<MgHead>(p => p.IsDeleted == EmIsDeleted.Normal && p.HeadCode == headCode);
        }

        public async Task DelMgHead(int id)
        {
            await _dbWrapper.Execute($"UPDATE MgHead SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id}");
            RemoveCache(id);
        }

        public async Task<Tuple<IEnumerable<MgHead>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<MgHead>("MgHead", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task UpdateTenantCount(int id)
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT COUNT(1) FROM MgTenants WHERE HeadId = {id} AND IsDeleted = {EmIsDeleted.Normal}");
            await _dbWrapper.Execute($"UPDATE MgHead SET TenantCount = {obj.ToInt()} WHERE Id = {id}");
            await UpdateCache(id);
        }
    }
}
