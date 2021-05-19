using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysUserRoleDAL
    {
        Task<SysUserRole> GetRole(int id);

        Task<List<SysUserRole>> GetMyRoles(int agentId);

       Task<bool> AddRole(SysUserRole entity);

        Task<bool> EditRole(SysUserRole entity);

        Task<bool> IsCanNotDel(int roleId);

        Task<bool> DelRole(int id);

        Task<Tuple<IEnumerable<SysUserRole>, int>> GetPaging(AgentPagingBase request);
    }
}
