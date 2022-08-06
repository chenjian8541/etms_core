using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class ParentMenusConfigDAL : DataAccessBase<ParentMenusConfigBucket>, IParentMenusConfigDAL
    {
        private readonly ITenantConfigDAL _tenantConfigDAL;

        public ParentMenusConfigDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider, ITenantConfigDAL tenantConfigDAL) : base(dbWrapper, cacheProvider)
        {
            this._tenantConfigDAL = tenantConfigDAL;
        }

        public override void InitTenantId(int tenantId)
        {
            base.InitTenantId(tenantId);
            this._tenantConfigDAL.InitTenantId(tenantId);
        }

        protected override async Task<ParentMenusConfigBucket> GetDb(params object[] keys)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            var myParentMenuConfig = config.ParentSetConfig.ParentMenus;
            var parentMenus = new List<ParentMenuConfigOutput>();
            foreach (var p in ParentMenuConfig.AllConfig)
            {
                if (myParentMenuConfig != null && myParentMenuConfig.Count > 0)
                {
                    var setConfig = myParentMenuConfig.FirstOrDefault(j => j.Id == p.Id);
                    if (setConfig != null)
                    {
                        parentMenus.Add(new ParentMenuConfigOutput()
                        {
                            IcoKey = setConfig.IcoKey,
                            IcoUrl = AliyunOssUtil.GetAccessUrlHttps(setConfig.IcoKey),
                            Id = p.Id,
                            IsShow = setConfig.IsShow,
                            OrderIndex = p.OrderIndex,
                            Title = setConfig.Title
                        });
                        continue;
                    }
                }
                parentMenus.Add(new ParentMenuConfigOutput()
                {
                    Id = p.Id,
                    IsShow = true,
                    OrderIndex = p.OrderIndex,
                    Title = p.Title,
                    IcoKey = p.IcoKey,
                    IcoUrl = AliyunOssUtil.GetAccessUrlHttps(p.IcoKey)
                });
            }

            return new ParentMenusConfigBucket()
            {
                ParentMenus = parentMenus.OrderBy(p => p.OrderIndex).ToList()
            };
        }

        public async Task<List<ParentMenuConfigOutput>> GetParentMenuConfig()
        {
            var bucket = await GetCache(_tenantId);
            return bucket.ParentMenus;
        }

        public async Task UpdateParentMenuConfig()
        {
            await UpdateCache(_tenantId);
        }

        public void ClearMenuConfig()
        {
            RemoveCache(_tenantId);
        }
    }
}
