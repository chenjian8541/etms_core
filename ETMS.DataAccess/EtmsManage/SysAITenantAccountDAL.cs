using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysAITenantAccountDAL : BaseCacheDAL<SysAITenantAccountBucket>, ISysAITenantAccountDAL, IEtmsManage
    {
        public SysAITenantAccountDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysAITenantAccountBucket> GetDb(params object[] keys)
        {
            var log = await this.Find<SysAITenantAccount>(p => p.IsDeleted == EmIsDeleted.Normal && p.Id == keys[0].ToLong());
            if (log == null)
            {
                return null;
            }
            return new SysAITenantAccountBucket()
            {
                SysAITenantAccount = log
            };
        }

        public async Task<SysAITenantAccount> GetSysAITenantAccount(int id)
        {
            var bucket = await GetCache(id);
            return bucket?.SysAITenantAccount;
        }

        public async Task<int> AddSysAITenantAccount(SysAITenantAccount entity)
        {
            await this.Insert(entity);
            await UpdateCache(entity.Id);
            return entity.Id;
        }

        public async Task<bool> EditSysAITenantAccount(SysAITenantAccount entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.Id);
            return true;
        }

        public async Task<List<SysAITenantAccount>> GetSysAITenantAccountSystem()
        {
            return await this.FindList<SysAITenantAccount>(p => p.IsDeleted == EmIsDeleted.Normal
            && p.Type == EmSysAIInterfaceType.System);
        }

        public async Task<List<SysAITenantAccount>> GetSysAITenantAccount()
        {
            return await this.FindList<SysAITenantAccount>(p => p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> ExistAITenantAccount(string secretId, int id = 0)
        {
            var log = await this.Find<SysAITenantAccount>(p => p.IsDeleted == EmIsDeleted.Normal
            && p.SecretId == secretId && p.Id != id);
            return log != null;
        }

        public async Task<bool> IsCanNotDel(int id)
        {
            var useLog = await this.Find<SysTenant>(p => p.IsDeleted == EmIsDeleted.Normal &&
            p.AICloudType == EmSysTenantAICloudType.TencentCloud && p.TencentCloudId == id);
            return useLog != null;
        }

        public async Task<Tuple<IEnumerable<SysAITenantAccount>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysAITenantAccount>("SysAITenantAccount", "*", request.PageSize, request.PageCurrent, "Type DESC,Id DESC", request.ToString());
        }
    }
}
