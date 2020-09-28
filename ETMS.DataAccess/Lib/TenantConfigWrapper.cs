﻿using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.View;
using ETMS.IOC;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.Entity.Enum;
using ETMS.Utility;

namespace ETMS.DataAccess.Lib
{
    /// <summary>
    /// 机构配置信息
    /// </summary>
    public class TenantConfigWrapper : BaseCacheDAL<SysTenantConfigBucket>, ITenantConfigWrapper
    {
        public TenantConfigWrapper(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysTenantConfigBucket> GetDb(params object[] keys)
        {
            var tenantConfig = await DapperProvider.ExecuteObject<ViewTenantConfig>(CustomServiceLocator.GetInstance<IAppConfigurtaionServices>().AppSettings.DatabseConfig.EtmsManageConnectionString,
                 $"SELECT SysTenant.Id AS Id,SysConnectionString.Value AS ConnectionString FROM SysTenant INNER JOIN SysConnectionString ON SysTenant.ConnectionId = SysConnectionString.Id WHERE SysTenant.IsDeleted = {EmIsDeleted.Normal}");
            return new SysTenantConfigBucket()
            {
                TenantConfigs = tenantConfig.ToList()
            };
        }

        public async Task<string> GetTenantConnectionString(int tenantId)
        {
            var tenantConfigBucket = await base.GetCache();
            return CryptogramHelper.Decrypt3DES(tenantConfigBucket.TenantConfigs.First(p => p.Id == tenantId).ConnectionString, SystemConfig.CryptogramConfig.Key);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public async Task TenantConnectionUpdate()
        {
            await base.UpdateCache();
        }
    }
}
