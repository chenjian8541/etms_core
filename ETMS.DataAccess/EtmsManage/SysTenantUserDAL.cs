using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantUserDAL : BaseCacheDAL<SysTenantUserBucket>, ISysTenantUserDAL, IEtmsManage
    {
        public SysTenantUserDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysTenantUserBucket> GetDb(params object[] keys)
        {
            var phone = keys[0].ToString();
            var myTenants = await this.FindList<SysTenantUser>(p => p.Phone == phone
            && p.Status == EmSysTenantPeopleStatus.Normal && p.IsDeleted == EmIsDeleted.Normal);
            return new SysTenantUserBucket()
            {
                SysTenantUsers = myTenants
            };
        }

        public async Task ResetTenantAllUser(int tenantId)
        {
            await this.Execute($"UPDATE SysTenantUser SET [Status] = {EmSysTenantPeopleStatus.Lock} WHERE TenantId = {tenantId} ");
        }

        public async Task AddTenantUser(int tenantId, long userId, string phone, bool isRefreshCache)
        {
            var log = await this.Find<SysTenantUser>(p => p.TenantId == tenantId && p.Phone == phone && p.IsDeleted == EmIsDeleted.Normal);
            if (log != null)
            {
                log.Status = EmSysTenantPeopleStatus.Normal;
                await this.Update(log);
            }
            else
            {
                await this.Insert(new SysTenantUser()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    LastOpTime = null,
                    Phone = phone,
                    Remark = string.Empty,
                    Status = EmSysTenantPeopleStatus.Normal,
                    TenantId = tenantId,
                    UserId = userId
                });
            }
            if (isRefreshCache)
            {
                await UpdateCache(phone);
            }
        }

        public async Task RemoveTenantUser(int tenantId, string phone)
        {
            await this.Execute($"DELETE SysTenantUser WHERE TenantId = {tenantId} AND Phone = '{phone}' ");
            await UpdateCache(phone);
        }

        public async Task UpdateTenantUserOpTime(int tenantId, string phone, DateTime opTime)
        {
            await this.Execute($"UPDATE SysTenantUser SET LastOpTime = '{opTime.EtmsToString()}' WHERE TenantId = {tenantId} AND Phone = '{phone}' ");
            await UpdateCache(phone);
        }

        public async Task<List<SysTenantUser>> GetTenantUser(string phone)
        {
            var bucket = await GetCache(phone);
            return bucket?.SysTenantUsers;
        }
    }
}
