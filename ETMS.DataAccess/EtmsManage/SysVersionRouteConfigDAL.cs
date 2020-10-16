using ETMS.Authority;
using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Config.Router;
using ETMS.Entity.Database.Manage;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.LOG;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysVersionRouteConfigDAL : BaseCacheDAL<SysVersionRouteConfigBucket>, ISysVersionRouteConfigDAL, IEtmsManage
    {
        public SysVersionRouteConfigDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysVersionRouteConfigBucket> GetDb(params object[] keys)
        {
            var versionId = keys[0].ToInt();
            Log.Warn($"[SysVersionRouteConfigDAL]从数据库中获取系统版本路由信息:versionId:{versionId}", this.GetType());
            var sysVersion = await this.Find<SysVersion>(p => p.Id == versionId);
            if (sysVersion == null)
            {
                Log.Error($"[SysVersionRouteConfigDAL]获取系统版本失败,versionId:{versionId}", this.GetType());
                return null;
            }
            var pageWeight = sysVersion.EtmsAuthorityValue.Split('|')[2].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            var pageRoute = EtmsHelper.DeepCopy(PermissionData.RouteConfigs);
            PageRouteHandle(pageRoute, authorityCorePage);
            return new SysVersionRouteConfigBucket()
            {
                RouteConfigs = pageRoute
            };
        }

        private static void PageRouteHandle(List<RouteConfig> pageRoute, AuthorityCore authorityCorePage)
        {
            foreach (var p in pageRoute)
            {
                if (p.Id == 0 || !authorityCorePage.Validation(p.Id))
                {
                    p.Hidden = true;
                }
                if (p.Children != null && p.Children.Any())
                {
                    PageRouteHandle(p.Children, authorityCorePage);
                }
            }
        }

        public async Task<List<RouteConfig>> GetTenantRouteConfig(int versionId)
        {
            var bucket = await GetCache(versionId);
            return bucket?.RouteConfigs;
        }

        public async Task Update(int versionId)
        {
            await UpdateCache(versionId);
        }
    }
}
