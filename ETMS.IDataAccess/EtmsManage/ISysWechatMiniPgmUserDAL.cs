using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysWechatMiniPgmUserDAL
    {
        Task<SysWechatMiniPgmUser> GetWechatMiniPgmUser(long id);

        Task AddWechatMiniPgmUser(SysWechatMiniPgmUser entity);

        Task EditWechatMiniPgmUser(SysWechatMiniPgmUser entity);

        Task<SysWechatMiniPgmUser> GetWechatMiniPgmUser(string openId);
    }
}
