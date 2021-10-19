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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class TenantConfig2DAL : DataAccessBase<TenantConfig2Bucket>, ITenantConfig2DAL
    {
        public TenantConfig2DAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TenantConfig2Bucket> GetDb(params object[] keys)
        {
            var appConfig = await this._dbWrapper.Find<EtAppConfig>(p => p.TenantId == _tenantId && p.Type == EmAppConfigType.TenantConfig2 && p.IsDeleted == EmIsDeleted.Normal);
            if (appConfig == null)
            {
                return new TenantConfig2Bucket()
                {
                    TenantConfig2 = new TenantConfig2()
                };
            }
            return new TenantConfig2Bucket()
            {
                TenantConfig2 = JsonConvert.DeserializeObject<TenantConfig2>(appConfig.ConfigValue)
            };
        }

        public async Task<TenantConfig2> GetTenantConfig()
        {
            var bucket = await GetCache(_tenantId);
            return bucket.TenantConfig2;
        }

        public async Task<bool> SaveTenantConfig(TenantConfig2 config)
        {
            var configValue = JsonConvert.SerializeObject(config);
            var appConfig = await this._dbWrapper.Find<EtAppConfig>(p => p.TenantId == _tenantId && p.Type == EmAppConfigType.TenantConfig2 && p.IsDeleted == EmIsDeleted.Normal);
            if (appConfig == null)
            {
                await _dbWrapper.Insert(new EtAppConfig()
                {
                    ConfigValue = configValue,
                    IsDeleted = EmIsDeleted.Normal,
                    Remark = string.Empty,
                    TenantId = _tenantId,
                    Type = EmAppConfigType.TenantConfig2
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
