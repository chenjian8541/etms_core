using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysRoleDAL
    {
        Task<SysRole> GetRole(int id);

        Task<List<SysRole>> GetRoles();

        Task<bool> AddRole(SysRole entity);

        Task<bool> EditRole(SysRole entity);

        Task<bool> IsCanNotDel(int roleId);

        Task<bool> DelRole(int id);
    }
}
