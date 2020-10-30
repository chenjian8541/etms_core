using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.Wechart;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ETMS.DataAccess.Wechart
{
    public class SysWechartAuthorizerTokenDAL : BaseCacheDAL<SysWechartAuthorizerTokenBucket>, ISysWechartAuthorizerTokenDAL, IEtmsManage
    {
        public SysWechartAuthorizerTokenDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysWechartAuthorizerTokenBucket> GetDb(params object[] keys)
        {
            var authorizerAppid = keys[0].ToString();
            var db = await this.Find<SysWechartAuthorizerToken>(p => p.AuthorizerAppid == authorizerAppid && p.IsDeleted == EmIsDeleted.Normal);
            if (db == null)
            {
                return null;
            }
            return new SysWechartAuthorizerTokenBucket()
            {
                SysWechartAuthorizerToken = db
            };
        }

        public async Task<SysWechartAuthorizerToken> GetSysWechartAuthorizerToken(string authorizerAppid)
        {
            var bucket = await GetCache(authorizerAppid);
            return bucket?.SysWechartAuthorizerToken;
        }

        public async Task<bool> SaveSysWechartAuthorizerToken(SysWechartAuthorizerToken entity)
        {
            if (entity.Id > 0)
            {
                await this.Update(entity);
            }
            else
            {
                await this.Insert(entity);
            }
            await UpdateCache(entity.AuthorizerAppid);
            return true;
        }
    }
}
