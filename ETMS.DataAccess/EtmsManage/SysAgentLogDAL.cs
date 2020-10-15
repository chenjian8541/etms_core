using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysAgentLogDAL : ISysAgentLogDAL, IEtmsManage
    {
        public async Task AddSysAgentOpLog(SysAgentOpLog sysAgentOpLog)
        {
            await this.Insert(sysAgentOpLog);
        }

        public async Task AddSysAgentOpLog(AgentRequestBase request, string opContent, int type)
        {
            await this.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = opContent,
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = type
            });
        }

        public async Task AddSysAgentEtmsAccountLog(SysAgentEtmsAccountLog sysAgentEtmsAccountLog)
        {
            await this.Insert(sysAgentEtmsAccountLog);
        }

        public async Task AddSysAgentSmsLog(SysAgentSmsLog sysAgentSmsLog)
        {
            await this.Insert(sysAgentSmsLog);
        }

        public async Task<Tuple<IEnumerable<SysAgentOpLogView>, int>> GetPagingOpLog(AgentPagingBase request)
        {
            return await this.ExecutePage<SysAgentOpLogView>("SysAgentOpLogView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<SysAgentEtmsAccountLogView>, int>> GetPagingEtmsAccountLog(AgentPagingBase request)
        {
            return await this.ExecutePage<SysAgentEtmsAccountLogView>("SysAgentEtmsAccountLogView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<SysAgentSmsLogView>, int>> GetPagingSmsLog(AgentPagingBase request)
        {
            return await this.ExecutePage<SysAgentSmsLogView>("SysAgentSmsLogView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
