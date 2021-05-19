using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysAgentLogDAL
    {
        Task AddSysAgentOpLog(SysAgentOpLog sysAgentOpLog,long userId);

        Task AddSysAgentOpLog(AgentRequestBase request, string opContent, int type);

        Task AddSysAgentEtmsAccountLog(SysAgentEtmsAccountLog sysAgentEtmsAccountLog, long userId);

        Task AddSysAgentSmsLog(SysAgentSmsLog sysAgentSmsLog, long userId);

        Task<Tuple<IEnumerable<SysAgentOpLogView>, int>> GetPagingOpLog(AgentPagingBase request);

        Task<Tuple<IEnumerable<SysAgentEtmsAccountLogView>, int>> GetPagingEtmsAccountLog(AgentPagingBase request);

        Task<Tuple<IEnumerable<SysAgentSmsLogView>, int>> GetPagingSmsLog(AgentPagingBase request);
    }
}
