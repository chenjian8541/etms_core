using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.EtmsManage.Common;


namespace ETMS.DataAccess.EtmsManage
{
    public class SysExternalConfigDAL : BaseCacheDAL<SysExternalConfigBucket>, ISysExternalConfigDAL, IEtmsManage
    {
        public SysExternalConfigDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysExternalConfigBucket> GetDb(params object[] keys)
        {
            var data = await this.FindList<SysExternalConfig>(p => p.IsDeleted == EmIsDeleted.Normal);
            return new SysExternalConfigBucket()
            {
                SysExternalConfigs = data
            };
        }

        public async Task AddSysExternalConfig(SysExternalConfig entity)
        {
            await this.Insert(entity);
            await UpdateCache();
        }

        public async Task EditSysExternalConfig(SysExternalConfig entity)
        {
            await this.Update(entity);
            await UpdateCache();
        }

        public async Task<SysExternalConfig> GetSysExternalConfigById(int id)
        {
            return await this.Find<SysExternalConfig>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<SysExternalConfig> GetSysExternalConfigByType(int type)
        {
            return await this.Find<SysExternalConfig>(p => p.Type == type && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task DelSysExternalConfig(SysExternalConfig entity)
        {
            await this.Execute($"DELETE [SysExternalConfig] WHERE Id = {entity.Id} ");
            await UpdateCache();
        }

        public async Task<Tuple<IEnumerable<SysExternalConfig>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysExternalConfig>("SysExternalConfig", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<List<SysExternalConfig>> GetSysExternalConfigs()
        {
            var bucket = await GetCache();
            return bucket?.SysExternalConfigs;
        }
    }
}
