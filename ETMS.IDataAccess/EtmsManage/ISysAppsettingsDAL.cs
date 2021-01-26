using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysAppsettingsDAL
    {
        Task<bool> SaveSysAppsettings(string data, int type);

        Task<SysAppsettings> GetAppsettings(int type);
    }
}
