using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysUpgradeMsgDAL
    {
        Task<Tuple<IEnumerable<SysUpgradeMsg>, int>> GetPaging(AgentPagingBase request);

        Task<bool> AddSysUpgradeMsg(SysUpgradeMsg entity);

        Task<SysUpgradeMsg> GetLastSysUpgradeMsg();

        Task<bool> SetRead(int upgradeMsgId, int tenantId, long userId);

        Task<bool> GetUserIsRead(int upgradeMsgId, int tenantId, long userId);
    }
}
