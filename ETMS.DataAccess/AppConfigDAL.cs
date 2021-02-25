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

namespace ETMS.DataAccess
{
    public class AppConfigDAL : DataAccessBase<AppConfigBucket>, IAppConfigDAL
    {
        public AppConfigDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<AppConfigBucket> GetDb(params object[] keys)
        {
            var type = keys[1].ToInt();
            var log = await _dbWrapper.Find<EtAppConfig>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Type == type);
            if (log == null)
            {
                return null;
            }
            return new AppConfigBucket()
            {
                AppConfig = log
            };
        }

        public async Task SaveAppConfig(EtAppConfig entity)
        {
            var log = await _dbWrapper.Find<EtAppConfig>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Type == entity.Type);
            if (log != null)
            {
                log.ConfigValue = entity.ConfigValue;
                log.Remark = entity.Remark;
                await _dbWrapper.Update(log);
            }
            else
            {
                await _dbWrapper.Insert(entity);
            }
            await UpdateCache(_tenantId, entity.Type);
        }

        public async Task<EtAppConfig> GetAppConfig(byte type)
        {
            var bucket = await GetCache(_tenantId, type);
            return bucket?.AppConfig;
        }
    }
}
