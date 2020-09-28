using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class TenantConfigDAL : DataAccessBase<TenantConfigBucket>, ITenantConfigDAL
    {
        public TenantConfigDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TenantConfigBucket> GetDb(params object[] keys)
        {
            var appConfig = await this._dbWrapper.Find<EtAppConfig>(p => p.TenantId == _tenantId && p.Type == EmAppConfigType.TenantConfig && p.IsDeleted == EmIsDeleted.Normal);
            if (appConfig == null)
            {
                return new TenantConfigBucket()
                {
                    TenantConfig = new TenantConfig()
                };
            }
            return new TenantConfigBucket()
            {
                TenantConfig = JsonConvert.DeserializeObject<TenantConfig>(appConfig.ConfigValue)
            };
        }

        public async Task<TenantConfig> GetTenantConfig()
        {
            var bucket = await GetCache(_tenantId);
            return bucket.TenantConfig;
        }

        public async Task<bool> SaveTenantConfig(TenantConfig config)
        {
            var configValue = JsonConvert.SerializeObject(config);
            var appConfig = await this._dbWrapper.Find<EtAppConfig>(p => p.TenantId == _tenantId && p.Type == EmAppConfigType.TenantConfig && p.IsDeleted == EmIsDeleted.Normal);
            if (appConfig == null)
            {
                await _dbWrapper.Insert(new EtAppConfig()
                {
                    ConfigValue = configValue,
                    IsDeleted = EmIsDeleted.Normal,
                    Remark = string.Empty,
                    TenantId = _tenantId,
                    Type = EmAppConfigType.TenantConfig
                });
            }
            else
            {
                appConfig.ConfigValue = configValue;
                await _dbWrapper.Update(appConfig);
            }
            await this.UpdateCache(_tenantId);
            return true;
        }
    }
}
