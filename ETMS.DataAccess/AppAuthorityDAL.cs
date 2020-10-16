using ETMS.Authority;
using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Config.Router;
using ETMS.Entity.Database.Manage;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.LOG;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class AppAuthorityDAL : IAppAuthorityDAL, IEtmsManage
    {
        private readonly ISysVersionMenuConfigDAL _sysVersionMenuConfigDAL;

        private readonly ISysVersionRouteConfigDAL _sysVersionRouteConfigDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        public AppAuthorityDAL(ISysTenantDAL sysTenantDAL, ISysVersionMenuConfigDAL sysVersionMenuConfigDAL, ISysVersionRouteConfigDAL sysVersionRouteConfigDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._sysVersionMenuConfigDAL = sysVersionMenuConfigDAL;
            this._sysVersionRouteConfigDAL = sysVersionRouteConfigDAL;
        }

        public async Task<List<MenuConfig>> GetTenantMenuConfig(int tenantId)
        {
            var tenant = await _sysTenantDAL.GetTenant(tenantId);
            var myMenuConfigs = await _sysVersionMenuConfigDAL.GetTenantMenuConfig(tenant.VersionId);
            if (myMenuConfigs == null)
            {
                return new List<MenuConfig>();
            }
            return myMenuConfigs;
        }

        public async Task<List<RouteConfig>> GetTenantRouteConfig(int tenantId)
        {
            var tenant = await _sysTenantDAL.GetTenant(tenantId);
            var myRouteConfigs = await _sysVersionRouteConfigDAL.GetTenantRouteConfig(tenant.VersionId);
            if (myRouteConfigs == null)
            {
                return new List<RouteConfig>();
            }
            return myRouteConfigs;
        }
    }
}
