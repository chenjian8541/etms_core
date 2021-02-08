using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantStudentDAL
    {
        Task ResetTenantAllStudent(int tenantId);

        Task AddTenantStudent(int tenantId, long StudentId, string phone, bool isRefreshCache);

        Task RemoveTenantStudent(int tenantId, string phone);

        Task UpdateTenantStudentOpTime(int tenantId, string phone, DateTime opTime);

        Task<List<SysTenantStudent>> GetTenantStudent(string phone);
    }
}
