using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.Enum;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysUpgradeMsgDAL : BaseCacheDAL<SysUpgradeMsgBucket>, ISysUpgradeMsgDAL, IEtmsManage
    {
        public SysUpgradeMsgDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysUpgradeMsgBucket> GetDb(params object[] keys)
        {
            var lastDb = await this.ExecuteObject<SysUpgradeMsg>("select top 1 * from SysUpgradeMsg order by id desc");
            if (lastDb == null || !lastDb.Any())
            {
                return null;
            }
            return new SysUpgradeMsgBucket()
            {
                SysUpgradeMsg = lastDb.First()
            };
        }

        public async Task<Tuple<IEnumerable<SysUpgradeMsg>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysUpgradeMsg>("SysUpgradeMsg", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> AddSysUpgradeMsg(SysUpgradeMsg entity)
        {
            await this.Execute("DELETE SysUpgradeMsgRead ");
            await this.Insert(entity);
            await UpdateCache();
            return true;
        }

        public async Task<SysUpgradeMsg> GetLastSysUpgradeMsg()
        {
            var bucket = await GetCache();
            return bucket?.SysUpgradeMsg;
        }

        public async Task<bool> SetRead(int upgradeMsgId, int tenantId, long userId)
        {
            await this.Insert(new SysUpgradeMsgRead()
            {
                IsDeleted = EmIsDeleted.Normal,
                ReadTime = DateTime.Now,
                Remark = string.Empty,
                TenantId = tenantId,
                UpgradeMsgId = upgradeMsgId,
                UserId = userId
            });
            return true;
        }

        public async Task<bool> GetUserIsRead(int upgradeMsgId, int tenantId, long userId)
        {
            var log = await this.Find<SysUpgradeMsgRead>(p => p.UpgradeMsgId == upgradeMsgId && p.TenantId == tenantId
            && p.UserId == userId && p.IsDeleted == EmIsDeleted.Normal);
            return log != null;
        }
    }
}
