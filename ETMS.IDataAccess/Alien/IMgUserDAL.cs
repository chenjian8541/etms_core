using ETMS.Entity.Common;
using ETMS.Entity.Database.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Alien
{
    public interface IMgUserDAL : IBaseAlienDAL
    {
        Task<MgUser> ExistMgUserByPhone(string phone, long notId = 0);

        Task<MgUser> GetUser(long id);

        Task<MgUser> GetUser(string phone);

        Task<bool> AddUser(MgUser entity);

        Task<bool> EditUser(MgUser entity);

        Task UpdateUserLastLoginTime(long userId, DateTime lastLoginTime);

        Task<bool> DelUser(long id);

        Task<Tuple<IEnumerable<MgUser>, int>> GetPaging(IPagingRequest request);

        Task<bool> ExistRole(long roleId);
    }
}
