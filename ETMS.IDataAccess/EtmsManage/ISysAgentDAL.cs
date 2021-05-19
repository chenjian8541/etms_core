using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysAgentDAL
    {
        Task<SysAgent> ExistSysAgentByPhone(string phone, int notId = 0);

        Task<SysAgent> ExistSysAgentByCode(string code, int notId = 0);

        Task<SysAgentBucket> GetAgent(int id);

        Task<bool> AddAgent(SysAgent entity, long userId);

        Task<bool> IsCanNotDelete(int agentId);

        Task UpdateAgentLastLoginTime(long agentId, DateTime lastLoginTime);

        Task<bool> EditAgent(SysAgent entity);

        Task<bool> DelAgent(int id);

        Task<bool> SmsCountAdd(int agentId, int count);

        Task<bool> SmsCountDeduction(int agentId, int count);

        Task<bool> EtmsAccountAdd(int agentId, int versionId, int count);

        Task<bool> EtmsAccountDeduction(int agentId, int versionId, int count);

        Task<Tuple<IEnumerable<SysAgent>, int>> GetPaging(AgentPagingBase request);

        Task<bool> EditAgentUser(List<int> agentIds, long userId);
    }
}
