using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysClientUpgradeDAL
    {
        Task<SysClientUpgrade> SysClientUpgradeGet(int id);

        Task SysClientUpgradeAdd(SysClientUpgrade entity);

        Task SysClientUpgradeEdit(SysClientUpgrade entity);

        Task SysClientUpgradeDel(SysClientUpgrade entity);

        Task<SysClientUpgrade> SysClientUpgradeLatestGet(int clientType);

        Task<Tuple<IEnumerable<SysClientUpgrade>, int>> GetPaging(AgentPagingBase request);
    }
}
