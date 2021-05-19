using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
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
        public async Task AddSysTenantEtmsAccountLog(SysTenantEtmsAccountLog entity, long userId)
        {
            entity.UserId = userId;
            await this.Insert(entity);
        }

        public async Task AddSysTenantSmsLog(SysTenantSmsLog entity, long userId)
        {
            entity.UserId = userId;
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

        public async Task<List<SysTenantEtmsAccountLog>> GetTenantEtmsAccountLog(int tenantId, int agentId, int versionId)
        {
            return await this.FindList<SysTenantEtmsAccountLog>(p => p.TenantId == tenantId && p.AgentId == agentId
            && p.VersionId == versionId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task AddSysTenantExDateLog(SysTenantExDateLog entity)
        {
            await this.Insert(entity);
        }

        public async Task<Tuple<IEnumerable<SysTenantExDateLog>, int>> GetSysTenantExDateLogPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysTenantExDateLog>("SysTenantExDateLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
