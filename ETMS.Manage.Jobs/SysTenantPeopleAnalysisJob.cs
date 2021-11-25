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
    public class SysTenantPeopleAnalysisJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        public SysTenantPeopleAnalysisJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher)
            :base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _eventPublisher.Publish(new SysTenantUserAnalysisEvent(tenant.Id));
            _eventPublisher.Publish(new SysTenantStudentAnalysisEvent(tenant.Id));
            _eventPublisher.Publish(new TenantAgentStatisticsEvent(tenant.Id));
        }
    }
}
