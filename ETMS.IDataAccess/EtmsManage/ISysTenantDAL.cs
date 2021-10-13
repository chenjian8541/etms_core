using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantDAL
    {
        Task<SysTenant> GetTenant(string tenantCode);

        Task<SysTenant> GetTenant(int id);

        Task<Tuple<IEnumerable<SysTenant>, int>> GetTenantsEffective(int pageSize, int pageCurrent);

        Task<int> AddTenant(SysTenant sysTenant, long userId);

        Task<bool> EditTenant(SysTenant sysTenant);

        Task EditTenantCode(int id, string newTenantCode);

        Task<bool> DelTenant(SysTenant sysTenant);

        Task<Tuple<IEnumerable<SysTenantView>, int>> GetPaging(AgentPagingBase request);

        Task<bool> ExistPhone(string phone, int id = 0);

        Task<bool> ExistTenantCode(string tenantCode);

        Task<bool> TenantSmsDeduction(int id, int deSmsCount);

        Task<bool> EditTenantUserId(List<int> tenantIds, long userId);

        Task UpdateTenantLcswInfo(int id, int newLcswApplyStatus, byte newLcswOpenStatus);
    }
}
