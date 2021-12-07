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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class TenantFubeiAccountDAL : BaseCacheDAL<SysTenantFubeiAccountBucket>, ITenantFubeiAccountDAL, IEtmsManage
    {
        public TenantFubeiAccountDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }
        protected override async Task<SysTenantFubeiAccountBucket> GetDb(params object[] keys)
        {
            var data = await this.Find<SysTenantFubeiAccount>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == keys[0].ToInt());
            if (data == null)
            {
                return null;
            }
            return new SysTenantFubeiAccountBucket()
            {
                TenantFubeiAccount = data
            };
        }

        public async Task<SysTenantFubeiAccount> GetTenantFubeiAccount(long tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.TenantFubeiAccount;
        }

        public async Task AddTenantFubeiAccount(SysTenantFubeiAccount entity)
        {
            await this.Insert(entity);
            await UpdateCache(entity.Id);
        }

        public async Task EditTenantFubeiAccount(SysTenantFubeiAccount entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.Id);
        }
    }
}
