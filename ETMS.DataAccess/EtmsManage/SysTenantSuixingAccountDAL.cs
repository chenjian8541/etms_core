﻿using ETMS.DataAccess.Core;
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
    public class SysTenantSuixingAccountDAL : BaseCacheDAL<SysTenantSuixingAccountBucket>, ISysTenantSuixingAccountDAL, IEtmsManage
    {
        public SysTenantSuixingAccountDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysTenantSuixingAccountBucket> GetDb(params object[] keys)
        {
            var data = await this.Find<SysTenantSuixingAccount>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == keys[0].ToInt());
            if (data == null)
            {
                return null;
            }
            return new SysTenantSuixingAccountBucket()
            {
                TenantSuixingAccount = data
            };
        }

        public async Task<SysTenantSuixingAccount> GetTenantSuixingAccount(long tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.TenantSuixingAccount;
        }

        public async Task<SysTenantSuixingAccount> GetTenantSuixingAccount(string mno)
        {
            return await this.Find<SysTenantSuixingAccount>(p => p.IsDeleted == EmIsDeleted.Normal && p.MblNo == mno);
        }

        public async Task AddTenantSuixingAccount(SysTenantSuixingAccount entity)
        {
            var oldEntity = await GetTenantSuixingAccount(entity.TenantId);
            if (oldEntity != null)
            {
                throw new EtmsErrorException("机构已绑定随行付账户");
            }
            await this.Insert(entity);
        }

        public async Task EditTenantSuixingAccount(SysTenantSuixingAccount entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.TenantId);
        }
    }
}
