using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITenantConfigDAL : IBaseDAL
    {
        Task<TenantConfig> GetTenantConfig();

        Task<bool> SaveTenantConfig(TenantConfig config);
    }
}
