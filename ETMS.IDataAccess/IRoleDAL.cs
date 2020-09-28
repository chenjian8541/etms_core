using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IRoleDAL : IBaseDAL
    {
        Task<EtRole> GetRole(long id);

        Task<List<EtRole>> GetRole();

        Task<bool> AddRole(EtRole role);

        Task<bool> EditRole(EtRole role);

        Task<bool> DelRole(long id);

        Task<RoleBucket> GetRoleBucket(long id);
    }
}
