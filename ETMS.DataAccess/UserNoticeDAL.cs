using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess
{
    public class UserNoticeDAL : DataAccessBase<UserNoticeBucket>, IUserNoticeDAL
    {
        public UserNoticeDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<UserNoticeBucket> GetDb(params object[] keys)
        {
            var userId = keys[1].ToLong();
            var logs = await _dbWrapper.FindList<EtUserNotice>(p => p.TenantId == _tenantId && p.UserId == userId
            && p.IsDeleted == EmIsDeleted.Normal && p.Status == EmUserNoticeStatus.Unread);
            return new UserNoticeBucket()
            {
                UserNotices = logs
            };
        }

        public async Task<bool> AddUserNotice(List<EtUserNotice> entitys)
        {
            _dbWrapper.InsertRange(entitys);
            var users = entitys.Select(p => p.UserId);
            foreach (var userId in users)
            {
                await UpdateCache(_tenantId, userId);
            }
            return true;
        }

        public async Task<EtUserNotice> GetUserNotice(long id)
        {
            return await this._dbWrapper.Find<EtUserNotice>(id);
        }

        public async Task<bool> EditUserNotice(EtUserNotice entity)
        {
            await this._dbWrapper.Update(entity);
            return true;
        }

        public async Task<List<EtUserNotice>> GetUnreadNotice(long userId)
        {
            var bucket = await GetCache(_tenantId, userId);
            return bucket?.UserNotices;
        }
    }
}
