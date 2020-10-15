using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysConnectionStringDAL
    {
        Task<List<SysConnectionString>> GetSysConnectionString();
    }
}
