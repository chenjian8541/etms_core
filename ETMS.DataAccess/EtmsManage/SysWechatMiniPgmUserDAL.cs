using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
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
            var id = keys[0].ToLong();
            var log = await this.Find<SysWechatMiniPgmUser>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new SysWechatMiniPgmUserBucket()
            {
                WechatMiniPgmUser = log
            };
        }

        public async Task<SysWechatMiniPgmUser> GetWechatMiniPgmUser(long id)
        {
            var bucket = await GetCache(id);
            return bucket?.WechatMiniPgmUser;
        }

        public async Task AddWechatMiniPgmUser(SysWechatMiniPgmUser entity)
        {
            await this.Insert(entity);
        }

        public async Task EditWechatMiniPgmUser(SysWechatMiniPgmUser entity)
        {
            await this.Update(entity);
            RemoveCache(entity.Id);
        }

        public async Task<SysWechatMiniPgmUser> GetWechatMiniPgmUser(string openId)
        {
            return await this.Find<SysWechatMiniPgmUser>(p => p.OpenId == openId && p.IsDeleted == EmIsDeleted.Normal);
        }
    }
}
