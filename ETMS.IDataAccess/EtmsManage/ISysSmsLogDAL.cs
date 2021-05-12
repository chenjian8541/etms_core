using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysSmsLogDAL
    {
        Task AddSysSmsLog(List<SysSmsLog> smsLogs);

        Task<Tuple<IEnumerable<SysSmsLog>, int>> GetPaging(AgentPagingBase request);
    }
}
