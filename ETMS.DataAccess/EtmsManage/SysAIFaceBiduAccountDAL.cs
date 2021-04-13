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
    public class SysAIFaceBiduAccountDAL : BaseCacheDAL<SysAIFaceBiduAccountBucket>, ISysAIFaceBiduAccountDAL, IEtmsManage
    {
        public SysAIFaceBiduAccountDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysAIFaceBiduAccountBucket> GetDb(params object[] keys)
        {
            var log = await this.Find<SysAIFaceBiduAccount>(p => p.IsDeleted == EmIsDeleted.Normal && p.Id == keys[0].ToLong());
            if (log == null)
            {
                return new SysAIFaceBiduAccountBucket();
            }
            return new SysAIFaceBiduAccountBucket()
            {
                SysAIFaceBiduAccount = log
            };
        }

        public async Task<SysAIFaceBiduAccount> GetSysAIFaceBiduAccount(int id)
        {
            var bucket = await GetCache(id);
            return bucket?.SysAIFaceBiduAccount;
        }

        public async Task<int> AddSysAIFaceBiduAccount(SysAIFaceBiduAccount entity)
        {
            await this.Insert(entity);
            await UpdateCache(entity.Id);
            return entity.Id;
        }

        public async Task<bool> EditSysAIFaceBiduAccount(SysAIFaceBiduAccount entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.Id);
            return true;
        }

        public async Task<List<SysAIFaceBiduAccount>> GetSysAIFaceBiduAccountSystem()
        {
            return await this.FindList<SysAIFaceBiduAccount>(p => p.IsDeleted == EmIsDeleted.Normal
            && p.Type == EmSysAIInterfaceType.System);
        }

        public async Task<List<SysAIFaceBiduAccount>> GetSysAIFaceBiduAccount()
        {
            return await this.FindList<SysAIFaceBiduAccount>(p => p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> ExistAIFaceBiduAccount(string appid, int id = 0)
        {
            var log = await this.Find<SysAIFaceBiduAccount>(p => p.IsDeleted == EmIsDeleted.Normal
            && p.Appid == appid && p.Id != id);
            return log != null;
        }

        public async Task<bool> IsCanNotDel(int id)
        {
            var useLog = await this.Find<SysTenant>(p => p.IsDeleted == EmIsDeleted.Normal &&
            p.AICloudType == EmSysTenantAICloudType.BaiduCloud && p.BaiduCloudId == id);
            return useLog != null;
        }

        public async Task<Tuple<IEnumerable<SysAIFaceBiduAccount>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysAIFaceBiduAccount>("SysAIFaceBiduAccount", "*", request.PageSize, request.PageCurrent, "Type DESC,Id DESC", request.ToString());
        }
    }
}
