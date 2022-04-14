using ETMS.Entity.Database.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Alien
{
    public interface IMgTenantsDAL : IBaseAlienDAL
    {
        Task<MgTenants> GetMgTenant(int id);

        Task<bool> ExistTenant(int tenantId);

        Task AddMgTenant(MgTenants entity);

        Task<List<MgTenants>> GetMgTenants();

        Task DelMgTenant(int tenantId);

        Task DelMgLog(int id);
    }
}
