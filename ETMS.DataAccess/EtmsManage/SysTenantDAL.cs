using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Config;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using ETMS.Utility;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantDAL : BaseCacheDAL<SysTenantBucket>, ISysTenantDAL, IEtmsManage
    {
        private readonly ITenantConfigWrapper _tenantConfigWrapper;
        public SysTenantDAL(ICacheProvider cacheProvider, ITenantConfigWrapper tenantConfigWrapper) : base(cacheProvider)
        {
            this._tenantConfigWrapper = tenantConfigWrapper;
        }
        protected override async Task<SysTenantBucket> GetDb(params object[] keys)
        {
            var id = keys[0].ToInt();
            var sysTenant = await this.Find<SysTenant>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
            return new SysTenantBucket()
            {
                SysTenant = sysTenant
            };
        }

        public async Task<SysTenant> GetTenant(string tenantCode)
        {
            return await this.Find<SysTenant>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantCode == tenantCode);
        }

        public async Task<SysTenant> GetTenant(int id)
        {
            var bucket = await base.GetCache(id);
            return bucket?.SysTenant;
        }

        public async Task<List<SysTenant>> GetTenants()
        {
            return await this.FindList<SysTenant>(p => p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<List<SysTenant>> GetTenantsNormal()
        {
            return await this.FindList<SysTenant>(p => p.IsDeleted == EmIsDeleted.Normal && p.Status == EmSysTenantStatus.Normal && p.ExDate >= DateTime.Now.Date);
        }

        public async Task<int> AddTenant(SysTenant sysTenant)
        {
            await this.Insert(sysTenant);
            await UpdateCache(sysTenant.Id);
            await _tenantConfigWrapper.TenantConnectionUpdate();
            return sysTenant.Id;
        }

        public async Task<bool> EditTenant(SysTenant sysTenant)
        {
            await this.Update(sysTenant);
            await UpdateCache(sysTenant.Id);
            await _tenantConfigWrapper.TenantConnectionUpdate();
            return true;
        }

        public async Task<bool> DelTenant(SysTenant sysTenant)
        {
            sysTenant.SmsCount = 0;
            sysTenant.IsDeleted = EmIsDeleted.Deleted;
            sysTenant.TenantCode = $"{sysTenant.TenantCode}_del";
            await this.Update(sysTenant);
            RemoveCache(sysTenant.Id);
            return true;
        }

        public async Task<Tuple<IEnumerable<SysTenantView>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysTenantView>("SysTenantView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> ExistPhone(string phone, int id = 0)
        {
            var oldLog = await this.Find<SysTenant>(p => p.Phone == phone && p.IsDeleted == EmIsDeleted.Normal && p.Id != id);
            return oldLog != null;
        }

        public async Task<bool> ExistTenantCode(string tenantCode)
        {
            var oldLog = await this.Find<SysTenant>(p => p.TenantCode == tenantCode && p.IsDeleted == EmIsDeleted.Normal);
            return oldLog != null;
        }

        public async Task<bool> TenantSmsDeduction(int id, int deSmsCount)
        {
            await this.Execute($"UPDATE SysTenant SET SmsCount = SmsCount - {deSmsCount} WHERE Id = {id}");
            await UpdateCache(id);
            return true;
        }
    }
}
