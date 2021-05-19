using ETMS.Authority;
using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Database.Manage;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.LOG;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysVersionMenuConfigDAL : BaseCacheDAL<SysVersionMenuConfigBucket>, ISysVersionMenuConfigDAL, IEtmsManage
    {
        public SysVersionMenuConfigDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysVersionMenuConfigBucket> GetDb(params object[] keys)
        {
            var versionId = keys[0].ToInt();
            Log.Warn($"[SysVersionMenuConfigDAL]从数据库中获取系统版本菜单信息:versionId:{versionId}", this.GetType());
            var sysVersion = await this.Find<SysVersion>(p => p.Id == versionId);
            if (sysVersion == null)
            {
                Log.Error($"[SysVersionMenuConfigDAL]获取系统版本失败,versionId:{versionId}", this.GetType());
                return null;
            }
            var myAuthorityValueMenu = sysVersion.EtmsAuthorityValue.Split('|');
            var pageWeight = myAuthorityValueMenu[2].ToBigInteger();
            var actionWeight = myAuthorityValueMenu[1].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            var authorityCoreAction = new AuthorityCore(actionWeight);
            var allMenuConfig = EtmsHelper.DeepCopy(PermissionData.MenuConfigs);
            return new SysVersionMenuConfigBucket()
            {
                MenuConfigs = PermissionData.GetChildMenus(authorityCorePage, authorityCoreAction, allMenuConfig)
            };
        }

        

        public async Task<List<MenuConfig>> GetTenantMenuConfig(int versionId)
        {
            var bucket = await GetCache(versionId);
            return bucket?.MenuConfigs;
        }

        public async Task Update(int versionId)
        {
            await UpdateCache(versionId);
        }
    }
}
