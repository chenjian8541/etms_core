using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysDangerousIpDAL
    {
        Task AddSysDangerousIp(SysDangerousIp entity);

        Task<Tuple<IEnumerable<SysDangerousIp>, int>> GetPaging(AgentPagingBase request);
    }
}
