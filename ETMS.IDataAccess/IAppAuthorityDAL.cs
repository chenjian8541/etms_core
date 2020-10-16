using ETMS.Entity.Config.Menu;
using ETMS.Entity.Config.Router;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IAppAuthorityDAL
    {
        Task<List<MenuConfig>> GetTenantMenuConfig(int tenantId);

        Task<List<RouteConfig>> GetTenantRouteConfig(int tenantId);
    }
}
