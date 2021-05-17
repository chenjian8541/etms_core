using ETMS.Event.DataContract;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using System.Threading.Tasks;
using System;

namespace ETMS.Manage.Jobs
{
    public class SysTenantPeopleAnalysisJob : BaseJob
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IEventPublisher _eventPublisher;

        public SysTenantPeopleAnalysisJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
            var tenantList = await _sysTenantDAL.GetTenantsNormal();
            foreach (var tenant in tenantList)
            {
                _eventPublisher.Publish(new SysTenantUserAnalysisEvent(tenant.Id));
                _eventPublisher.Publish(new SysTenantStudentAnalysisEvent(tenant.Id));
                _eventPublisher.Publish(new TenantAgentStatisticsEvent(tenant.Id));
            }
        }
    }
}
