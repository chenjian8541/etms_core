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
    public class SysTenantOtherInfoDAL : BaseCacheDAL<SysTenantOtherInfoBucket>, ISysTenantOtherInfoDAL, IEtmsManage
    {
        public SysTenantOtherInfoDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysTenantOtherInfoBucket> GetDb(params object[] keys)
        {
            var log = await this.Find<SysTenantOtherInfo>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == keys[0].ToInt());
            if (log == null)
            {
                return null;
            }
            return new SysTenantOtherInfoBucket()
            {
                SysTenantOtherInfo = log
            };
        }

        public async Task<SysTenantOtherInfo> GetSysTenantOtherInfo(int tenantId)
        {
            var bucket = await GetCache(tenantId);
            return bucket?.SysTenantOtherInfo;
        }

        public async Task<bool> SaveTenantOtherInfo(SysTenantOtherInfo entity)
        {
            var log = await this.Find<SysTenantOtherInfo>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == entity.TenantId);
            if (log == null)
            {
                await this.Insert(entity);
            }
            else
            {
                log.HomeLogo1 = entity.HomeLogo1;
                log.HomeLogo2 = entity.HomeLogo2;
                log.LoginLogo1 = entity.LoginLogo1;
                log.LoginBg = entity.LoginBg;
                log.IsHideKeFu = entity.IsHideKeFu;
                log.WebSiteTitle = entity.WebSiteTitle;
                log.KefuMobile = entity.KefuMobile;
                await this.Update(log);
            }
            await UpdateCache(entity.TenantId);
            return true;
        }
    }
}
