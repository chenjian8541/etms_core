using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysUserDAL
    {
        Task<SysUser> ExistSysUserByPhone(int agentId, string phone, long notId = 0);

        Task<SysUser> GetUser(long id);

        Task<bool> AddUser(SysUser entity);

        Task UpdateUserLastLoginTime(long userId, DateTime lastLoginTime);

        Task<bool> EditUser(SysUser entity);

        Task<bool> DelUser(long id);

        Task<Tuple<IEnumerable<SysUser>, int>> GetPaging(AgentPagingBase request);
    }
}
