using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class TenantLcsAccountDAL : BaseCacheDAL<SysTenantLcsAccountBucket>, ITenantLcsAccountDAL, IEtmsManage
    {
        public TenantLcsAccountDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysTenantLcsAccountBucket> GetDb(params object[] keys)
        {
            var data = await this.Find<SysTenantLcsAccount>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == keys[0].ToInt());
            if (data == null)
            {
                return null;
            }
            return new SysTenantLcsAccountBucket()
            {
                TenantLcsAccount = data
            };
        }

        public async Task<SysTenantLcsAccount> GetTenantLcsAccount(long tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.TenantLcsAccount;
        }

        public async Task<SysTenantLcsAccount> GetTenantLcsAccount(string merchantNo)
        {
            return await this.Find<SysTenantLcsAccount>(p => p.IsDeleted == EmIsDeleted.Normal && p.MerchantNo == merchantNo);
        }

        public async Task AddTenantLcsAccount(SysTenantLcsAccount entity)
        {
            var oldEntity = await GetTenantLcsAccount(entity.TenantId);
            if (oldEntity != null)
            {
                throw new EtmsErrorException("机构已申请过支付账户信息");
            }
            await this.Insert(entity);
        }

        public async Task EditTenantLcsAccount(SysTenantLcsAccount entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.TenantId);
        }

        public async Task<long> AddTenantLcsPayLog(SysTenantLcsPayLog entity)
        {
            await this.Insert(entity);
            return entity.Id;
        }

        public async Task<SysTenantLcsPayLog> GetTenantLcsPayLog(long id)
        {
            return await this.Find<SysTenantLcsPayLog>(p => p.Id == id);
        }

        public async Task EditTenantLcsPayLog(SysTenantLcsPayLog entity)
        {
            await this.Update(entity);
        }

        public async Task<Tuple<IEnumerable<SysTenantLcsPayLog>, int>> GetTenantLcsPayLogPaging(IPagingRequest request)
        {
            return await this.ExecutePage<SysTenantLcsPayLog>("SysTenantLcsPayLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
