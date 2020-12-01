using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class UserWechatDAL : DataAccessBase<UserWechatBucket>, IUserWechatDAL
    {
        public UserWechatDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<UserWechatBucket> GetDb(params object[] keys)
        {
            var userId = keys[1].ToLong();
            var log = await _dbWrapper.Find<EtUserWechat>(p => p.TenantId == _tenantId && p.UserId == userId && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new UserWechatBucket()
            {
                UserWechat = log
            };
        }

        public async Task<EtUserWechat> GetUserWechat(long userId)
        {
            var bucket = await GetCache(_tenantId, userId);
            return bucket?.UserWechat;
        }

        public async Task<bool> SaveUserWechat(EtUserWechat userWechat)
        {
            var old = await GetUserWechat(userWechat.UserId);
            if (old == null)
            {
                await _dbWrapper.Insert(userWechat);
            }
            else
            {
                old.Phone = userWechat.Phone;
                old.WechatUnionid = userWechat.WechatUnionid;
                old.WechatOpenid = userWechat.WechatOpenid;
                old.Nickname = userWechat.Nickname;
                old.Headimgurl = userWechat.Headimgurl;
                old.Remark = userWechat.Remark;
                await _dbWrapper.Update(old);
            }
            await UpdateCache(_tenantId, userWechat.UserId);
            return true;
        }
    }
}
