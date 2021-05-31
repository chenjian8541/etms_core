using ETMS.Event.DataContract;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using ETMS.Entity.Database.Manage;
using System.Linq;
using ETMS.Utility;

namespace ETMS.Manage.Jobs
{
    public class SysTenantPeopleAnalysisJob : BaseJob
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IEventPublisher _eventPublisher;

        private const int _pageSize = 100;

        public SysTenantPeopleAnalysisJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
            var pageCurrent = 1;
            var getTenantsEffectiveResult = await _sysTenantDAL.GetTenantsEffective(_pageSize, pageCurrent);
            if (getTenantsEffectiveResult.Item2 == 0)
            {
                return;
            }
            HandleTenantList(getTenantsEffectiveResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(getTenantsEffectiveResult.Item2, _pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                getTenantsEffectiveResult = await _sysTenantDAL.GetTenantsEffective(_pageSize, pageCurrent);
                HandleTenantList(getTenantsEffectiveResult.Item1);
                pageCurrent++;
            }
        }

        private void HandleTenantList(IEnumerable<SysTenant> tenantList)
        {
            if (tenantList == null || !tenantList.Any())
            {
                return;
            }
            foreach (var tenant in tenantList)
            {
                _eventPublisher.Publish(new SysTenantUserAnalysisEvent(tenant.Id));
                _eventPublisher.Publish(new SysTenantStudentAnalysisEvent(tenant.Id));
                _eventPublisher.Publish(new TenantAgentStatisticsEvent(tenant.Id));
            }
        }
    }
}
