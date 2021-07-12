using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class NoticeConfigDAL : DataAccessBase<NoticeConfigBucket>, INoticeConfigDAL
    {
        public NoticeConfigDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<NoticeConfigBucket> GetDb(params object[] keys)
        {
            var logs = await _dbWrapper.FindList<EtNoticeConfig>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            return new NoticeConfigBucket()
            {
                NoticeConfigs = logs
            };
        }

        public async Task SaveNoticeConfig(EtNoticeConfig entity)
        {
            var log = await _dbWrapper.Find<EtNoticeConfig>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal &&
            p.Type == entity.Type && p.PeopleType == entity.PeopleType && p.ScenesType == entity.ScenesType);
            if (log == null)
            {
                await _dbWrapper.Insert(entity);
            }
            else
            {
                log.ExType = entity.ExType;
                log.ConfigValue = entity.ConfigValue;
                await _dbWrapper.Update(log);
            }
            await UpdateCache(_tenantId);
        }

        public async Task<List<EtNoticeConfig>> GetNoticeConfig(int type)
        {
            var bucket = await GetCache(_tenantId);
            if (bucket == null || bucket.NoticeConfigs == null || bucket.NoticeConfigs.Count == 0)
            {
                return null;
            }
            return bucket.NoticeConfigs.Where(p => p.Type == type).ToList();
        }

        public async Task<EtNoticeConfig> GetNoticeConfig(int type, byte peopleType, int scenesType)
        {
            var bucket = await GetCache(_tenantId);
            if (bucket == null || bucket.NoticeConfigs == null || bucket.NoticeConfigs.Count == 0)
            {
                return null;
            }
            return bucket.NoticeConfigs.FirstOrDefault(p => p.Type == type && p.PeopleType == peopleType && p.ScenesType == scenesType);
        }
    }
}
