using ETMS.Entity.Database.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Alien
{
    public interface IMgRoleDAL: IBaseAlienDAL
    {
        Task<MgRole> GetRole(int id);

        Task<List<MgRole>> GetRoles();

        Task<bool> AddRole(MgRole entity);

        Task<bool> EditRole(MgRole entity);

        Task<bool> IsCanNotDel(int roleId);

        Task<bool> DelRole(int id);
    }
}
