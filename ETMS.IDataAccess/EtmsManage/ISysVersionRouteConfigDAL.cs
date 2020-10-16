using ETMS.Entity.Config.Router;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysVersionRouteConfigDAL
    {
        Task Update(int versionId);

        Task<List<RouteConfig>> GetTenantRouteConfig(int versionId);
    }
}
