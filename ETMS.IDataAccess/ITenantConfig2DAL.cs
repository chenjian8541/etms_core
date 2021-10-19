using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITenantConfig2DAL : IBaseDAL
    {
        Task<TenantConfig2> GetTenantConfig();

        Task<bool> SaveTenantConfig(TenantConfig2 config);
    }
}
