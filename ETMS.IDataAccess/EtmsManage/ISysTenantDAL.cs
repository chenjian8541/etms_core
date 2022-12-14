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

        Task<Tuple<IEnumerable<SysTenant>, int>> GetAllTenant(int pageSize, int pageCurrent);

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

        Task UpdateTenantLastOpTime(int id, DateTime lastOpTime);

        Task UpdateTenantCloudStorage(int id, decimal newValueMB, decimal newValueGB);

        Task<SysTenant> TenantGetByPhone(string phone);

        Task UpdateTenantLastRenewalTime(int id, DateTime? lastRenewalTime);

        Task UpdateTenantSetPayUnionType(int id, int newPayUnionType);

        Task UpdateTenantIpAddress(int id,string province,string city,string district,string ipAddress,DateTime upDate);

        Task UpdateLoginPhone(int id, string loginPhone);
    }
}
