using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket;
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
    public class SysClientUpgradeDAL : BaseCacheDAL<SysClientUpgradeBucket>, ISysClientUpgradeDAL, IEtmsManage
    {
        public SysClientUpgradeDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysClientUpgradeBucket> GetDb(params object[] keys)
        {
            var log = await this.ExecuteObject<SysClientUpgrade>(
                $"SELECT TOP 1 * FROM SysClientUpgrade WHERE ClientType = {keys[0].ToInt()} AND IsDeleted = {EmIsDeleted.Normal} ORDER BY Id DESC");
            if (!log.Any())
            {
                return null;
            }
            return new SysClientUpgradeBucket()
            {
                ClientUpgrade = log.First()
            };
        }

        public async Task<SysClientUpgrade> SysClientUpgradeGet(int id)
        {
            return await this.Find<SysClientUpgrade>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task SysClientUpgradeAdd(SysClientUpgrade entity)
        {
            await this.Insert(entity);
            await UpdateCache(entity.ClientType);
        }

        public async Task SysClientUpgradeEdit(SysClientUpgrade entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.ClientType);
        }

        public async Task<SysClientUpgrade> SysClientUpgradeLatestGet(int clientType)
        {
            var log = await GetCache(clientType);
            return log?.ClientUpgrade;
        }

        public async Task<Tuple<IEnumerable<SysClientUpgrade>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysClientUpgrade>("SysClientUpgrade", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
