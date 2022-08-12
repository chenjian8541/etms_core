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
    public class SysTenantWechartAuthDAL : BaseCacheDAL<SysTenantWechartAuthBucket>, ISysTenantWechartAuthDAL, IEtmsManage
    {
        public SysTenantWechartAuthDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysTenantWechartAuthBucket> GetDb(params object[] keys)
        {
            var tenantId = keys[0].ToInt();
            var db = await this.Find<SysTenantWechartAuth>(p => p.TenantId == tenantId && p.IsDeleted == EmIsDeleted.Normal);
            if (db == null)
            {
                return null;
            }
            return new SysTenantWechartAuthBucket()
            {
                SysTenantWechartAuth = db
            };
        }

        public async Task<SysTenantWechartAuth> GetSysTenantWechartAuth(int tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.SysTenantWechartAuth;
        }

        public async Task<bool> SaveSysTenantWechartAuth(SysTenantWechartAuth entity)
        {
            if (entity.Id > 0)
            {
                await this.Update(entity);
            }
            else
            {
                await this.Insert(entity);
            }
            await UpdateCache(entity.TenantId);
            return true;
        }

        public async Task<bool> OnUnauthorizeTenantWechart(string authorizerAppid)
        {
            var tenantWechartAuthList = await this.FindList<SysTenantWechartAuth>(p => p.AuthorizerAppid == authorizerAppid && p.IsDeleted == EmIsDeleted.Normal);
            foreach (var p in tenantWechartAuthList)
            {
                p.ModifyOt = DateTime.Now;
                p.AuthorizeState = EmSysTenantWechartAuthAuthorizeState.Unauthorized;
                await SaveSysTenantWechartAuth(p);
            }
            return true;
        }

        public async Task DelSysTenantWechartAuth(int tenantId)
        {
            await this.Execute($"UPDATE SysTenantWechartAuth SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {tenantId}");
            RemoveCache(tenantId);
        }
    }
}
