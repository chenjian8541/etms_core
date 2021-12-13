using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTryApplyLogDAL
    {
        Task AddSysTryApplyLog(SysTryApplyLog entity);

        Task<Tuple<IEnumerable<SysTryApplyLog>, int>> GetPaging(AgentPagingBase request);
    }
}
