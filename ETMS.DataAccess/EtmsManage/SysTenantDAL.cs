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

        public async Task<Tuple<IEnumerable<SysTenant>, int>> GetTenantsEffective(int pageSize, int pageCurrent)
        {
            var minDate = DateTime.Now.AddDays(-30).Date; //分析包含过期一个月以内的机构
            return await this.ExecutePage<SysTenant>("SysTenant", "*", pageSize, pageCurrent, "Id DESC", $" IsDeleted = {EmIsDeleted.Normal} AND ExDate >= '{minDate.EtmsToDateString()}' ");
        }

        public async Task<Tuple<IEnumerable<SysTenant>, int>> GetAllTenant(int pageSize, int pageCurrent)
        {
            return await this.ExecutePage<SysTenant>("SysTenant", "*", pageSize, pageCurrent, "Id DESC", $" IsDeleted = {EmIsDeleted.Normal} ");
        }

        public async Task<int> AddTenant(SysTenant sysTenant, long userId)
        {
            sysTenant.UserId = userId;
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

        public async Task EditTenantCode(int id, string newTenantCode)
        {
            await this.Execute($"UPDATE SysTenant SET TenantCode = '{newTenantCode}' WHERE Id = {id} ");
            await UpdateCache(id);
        }

        public async Task<bool> DelTenant(SysTenant sysTenant)
        {
            sysTenant.SmsCount = 0;
            sysTenant.IsDeleted = EmIsDeleted.Deleted;
            sysTenant.TenantCode = $"{sysTenant.TenantCode}_del_{DateTime.Now.ToString("ddHHmm")}";
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
            var oldLog = await this.Find<SysTenant>(p => p.TenantCode == tenantCode);
            return oldLog != null;
        }

        public async Task<bool> TenantSmsDeduction(int id, int deSmsCount)
        {
            await this.Execute($"UPDATE SysTenant SET SmsCount = SmsCount - {deSmsCount} WHERE Id = {id}");
            await UpdateCache(id);
            return true;
        }

        public async Task<bool> EditTenantUserId(List<int> tenantIds, long userId)
        {
            if (tenantIds.Count == 1)
            {
                await this.Execute($"UPDATE SysTenant SET UserId = {userId} WHERE Id = {tenantIds[0]} ");
            }
            else
            {
                await this.Execute($"UPDATE SysTenant SET UserId = {userId} WHERE Id IN ({string.Join(',', tenantIds)})");
            }
            foreach (var p in tenantIds)
            {
                await UpdateCache(p);
            }
            return true;
        }

        public async Task UpdateTenantLcswInfo(int id, int newLcswApplyStatus, byte newLcswOpenStatus)
        {
            await this.Execute($"UPDATE SysTenant SET AgtPayType = {EmAgtPayType.Lcsw} ,LcswApplyStatus = {newLcswApplyStatus} ,LcswOpenStatus = {newLcswOpenStatus} WHERE Id = {id} ");
            await UpdateCache(id);
        }

        public async Task UpdateTenantLastOpTime(int id, DateTime lastOpTime)
        {
            await this.Execute($"UPDATE SysTenant SET LastOpTime = '{lastOpTime.EtmsToString()}' WHERE Id = {id} ");
            RemoveCache(id);
        }

        public async Task UpdateTenantCloudStorage(int id, decimal newValueMB, decimal newValueGB)
        {
            await this.Execute($"UPDATE SysTenant SET CloudStorageValueMB = '{newValueMB}',CloudStorageValueGB = '{newValueGB}' WHERE Id = {id} ");
            await UpdateCache(id);
        }

        public async Task<SysTenant> TenantGetByPhone(string phone)
        {
            return await this.Find<SysTenant>(p => p.IsDeleted == EmIsDeleted.Normal && p.Phone == phone);
        }

        public async Task UpdateTenantLastRenewalTime(int id, DateTime? lastRenewalTime)
        {
            if (lastRenewalTime == null)
            {
                await this.Execute($"UPDATE SysTenant SET LastRenewalTime = null WHERE Id = {id} ");
                await UpdateCache(id);
            }
            else
            {
                await this.Execute($"UPDATE SysTenant SET LastRenewalTime = {lastRenewalTime.EtmsToString()} WHERE Id = {id} ");
                await UpdateCache(id);
            }
        }

        public async Task UpdateTenantSetPayUnionType(int id, int newPayUnionType)
        {
            await this.Execute($"UPDATE SysTenant SET PayUnionType = {newPayUnionType} WHERE Id = {id} ");
            await UpdateCache(id);
        }

        public async Task UpdateTenantIpAddress(int id, string province, string city, string district, string ipAddress, DateTime upDate)
        {
            await this.Execute($"UPDATE SysTenant SET Province = '{province}',City = '{city}',District = '{district}',IpAddress='{ipAddress}',IpUpdateOt = '{upDate.EtmsToString()}' WHERE Id = {id}");
            await UpdateCache(id);
        }
    }
}
