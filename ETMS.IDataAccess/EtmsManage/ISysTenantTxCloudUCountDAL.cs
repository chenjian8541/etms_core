using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantTxCloudUCountDAL
    {
        Task AddTenantTxCloudUCount(int tenantId, DateTime ot, byte type, int addCount);
    }
}
