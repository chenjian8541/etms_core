using ETMS.Entity.Config.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysVersionMenuConfigDAL
    {
        Task Update(int versionId);

        Task<List<MenuConfig>> GetTenantMenuConfig(int versionId);
    }
}
