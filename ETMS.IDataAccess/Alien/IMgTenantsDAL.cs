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
        Task<bool> ExistTenant(int tenantId);

        Task AddMgTenant(MgTenants entity);

        Task<List<MgTenants>> GetMgTenants();

        Task DelMgTenant(int tenantId);
    }
}
