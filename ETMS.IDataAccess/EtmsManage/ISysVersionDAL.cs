using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysVersionDAL
    {
        Task<SysVersion> GetVersion(int id);

        Task<List<SysVersion>> GetVersions();

        Task<bool> AddVersion(SysVersion entity);

        Task<bool> EditVersion(SysVersion entity);

        Task<bool> IsCanNotDelete(int id);

        Task<bool> DelVersion(int id);
    }
}
