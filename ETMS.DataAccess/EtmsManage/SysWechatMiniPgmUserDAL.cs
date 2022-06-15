using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysWechatMiniPgmUserDAL : BaseCacheDAL<SysWechatMiniPgmUserBucket>, ISysWechatMiniPgmUserDAL, IEtmsManage
    {
        public SysWechatMiniPgmUserDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysWechatMiniPgmUserBucket> GetDb(params object[] keys)
        {
            var openId = keys[0].ToString();
            var log = await this.Find<SysWechatMiniPgmUser>(p => p.OpenId == openId && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new SysWechatMiniPgmUserBucket()
            {
                WechatMiniPgmUser = log
            };
        }

        public async Task SaveWechatMiniPgmUser(SysWechatMiniPgmUser entity)
        {
            var bucket = await GetCache(entity.OpenId);
            var log = bucket?.WechatMiniPgmUser;
            if (log == null)
            {
                await this.Insert(entity);
            }
            else
            {
                log.Phone = entity.Phone;
                log.NickName = entity.NickName;
                log.AvatarUrl = entity.AvatarUrl;
                log.UpdateTime = DateTime.Now;
                await this.Update(log);
            }
            await UpdateCache(entity.OpenId);
        }

        public async Task<SysWechatMiniPgmUser> GetWechatMiniPgmUser(string openId)
        {
            var bucket = await GetCache(openId);
            return bucket?.WechatMiniPgmUser;
        }
    }
}
