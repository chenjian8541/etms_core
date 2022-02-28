using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantCloudStorageDAL
    {
        Task SaveCloudStorage(int tenantId, List<SysTenantCloudStorage> entityList);

        Task<List<SysTenantCloudStorage>> GetCloudStorage(int tenantId);
    }
}
