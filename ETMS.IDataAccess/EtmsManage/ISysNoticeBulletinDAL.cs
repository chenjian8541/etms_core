using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysNoticeBulletinDAL
    {
        Task<Tuple<IEnumerable<SysNoticeBulletin>, int>> GetPaging(AgentPagingBase request);

        Task AddSysNoticeBulletin(SysNoticeBulletin entity);

        Task DelSysNoticeBulletin(long id);

        Task<SysNoticeBulletin> GetUsableLog();

        void SetUserRead(int id, int tenantId, long userId);

        Task<bool> GetUserIsRead(int id, int tenantId, long userId);
    }
}
