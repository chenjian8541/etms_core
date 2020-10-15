using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantLogDAL : ISysTenantLogDAL, IEtmsManage
    {
        public async Task AddSysTenantEtmsAccountLog(SysTenantEtmsAccountLog entity)
        {
            await this.Insert(entity);
        }

        public async Task AddSysTenantSmsLog(SysTenantSmsLog entity)
        {
            await this.Insert(entity);
        }

        public async Task<Tuple<IEnumerable<SysTenantEtmsAccountLogView>, int>> GetTenantEtmsAccountLogPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysTenantEtmsAccountLogView>("SysTenantEtmsAccountLogView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<SysTenantSmsLogVew>, int>> GetTenantSmsLogPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysTenantSmsLogVew>("SysTenantSmsLogVew", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
