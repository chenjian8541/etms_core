using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IUserNoticeDAL : IBaseDAL
    {
        Task<bool> AddUserNotice(List<EtUserNotice> entitys);

        Task<EtUserNotice> GetUserNotice(long id);

        Task<bool> EditUserNotice(EtUserNotice entity);

        Task<List<EtUserNotice>> GetUnreadNotice(long userId);
    }
}
