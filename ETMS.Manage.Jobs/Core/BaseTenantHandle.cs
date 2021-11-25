using ETMS.Entity.Database.Manage;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Manage.Entity.Config;
using ETMS.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.Manage.Jobs
{
    public abstract class BaseTenantHandle : BaseJob
    {
        protected ISysTenantDAL _sysTenantDAL;

        private const int _pageSizeTenant = 100;

        public BaseTenantHandle(ISysTenantDAL sysTenantDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
        }

        public override async Task Process(JobExecutionContext context)
        {
            var pageCurrent = 1;
            var getTenantsEffectiveResult = await _sysTenantDAL.GetTenantsEffective(_pageSizeTenant, pageCurrent);
            if (getTenantsEffectiveResult.Item2 == 0)
            {
                return;
            }
            await HandleTenantList(getTenantsEffectiveResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(getTenantsEffectiveResult.Item2, _pageSizeTenant);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                getTenantsEffectiveResult = await _sysTenantDAL.GetTenantsEffective(_pageSizeTenant, pageCurrent);
                await HandleTenantList(getTenantsEffectiveResult.Item1);
                pageCurrent++;
            }
        }

        private async Task HandleTenantList(IEnumerable<SysTenant> tenantList)
        {
            if (tenantList == null || !tenantList.Any())
            {
                return;
            }
            foreach (var tenant in tenantList)
            {
                await this.ProcessTenant(tenant);
            }
        }

        public abstract Task ProcessTenant(SysTenant tenant);
    }
}
