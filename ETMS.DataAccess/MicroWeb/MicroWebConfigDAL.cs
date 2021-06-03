using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.MicroWeb;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.IDataAccess.MicroWeb;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.MicroWeb
{
    public class MicroWebConfigDAL : DataAccessBase<MicroWebConfigBucket>, IMicroWebConfigDAL
    {
        public MicroWebConfigDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<MicroWebConfigBucket> GetDb(params object[] keys)
        {
            var type = keys[1].ToInt();
            var log = await this._dbWrapper.Find<EtMicroWebConfig>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == _tenantId && p.Type == type);
            if (log == null)
            {
                return null;
            }
            return new MicroWebConfigBucket()
            {
                MicroWebConfig = log
            };
        }

        public async Task<bool> SaveMicroWebConfig(EtMicroWebConfig entity)
        {
            var log = await this._dbWrapper.Find<EtMicroWebConfig>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == entity.TenantId && p.Type == entity.Type);
            if (log != null)
            {
                log.ConfigValue = entity.ConfigValue;
                await _dbWrapper.Update(log);
            }
            else
            {
                await _dbWrapper.Insert(entity);
            }
            await this.UpdateCache(_tenantId, entity.Type);
            return true;
        }

        public async Task<EtMicroWebConfig> GetMicroWebConfig(int type)
        {
            var bucket = await GetCache(_tenantId, type);
            return bucket?.MicroWebConfig;
        }
    }
}
